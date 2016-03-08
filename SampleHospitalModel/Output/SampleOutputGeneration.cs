using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using SampleHospitalModel.Emergency;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.MathTool.Statistics;
using SimulationCore.ModelLog;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.Output
{
    /// <summary>
    /// Example how KPIs can be generated with the help of the generic output generator
    /// </summary>
    public class SampleOutputGeneration : GenericOutputGenereator
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, delegate dictionaries for event and control unit data collection
        /// methods are stored in dictionaries
        /// </summary>
        /// <param name="parentSimulationModel">Simulation model data is collected for</param>
        /// <param name="endOfWarmUpPeriod">Time after which data is collected</param>
        public SampleOutputGeneration(SimulationModel parentSimulationModel, DateTime endOfWarmUpPeriod) : base(parentSimulationModel, endOfWarmUpPeriod)
        {
            CollectingMethodsPerStandaloneEventType.Add(typeof(EventEmergencyPatientArrival), PatientArrivingAtEmergency);
            CollectingMethodsPerStandaloneEventType.Add(typeof(EventEmergencyPatientLeave), PatientLeavingAtEmergency);
            CollectingMethodsPerActivityStartEventType.Add(typeof(ActivityHealthCareAction<EmergencyActionTypeClass>), PatientFirstSeenByDoctor);
            CollectingMethodsPerControlUnitType.Add(typeof(ControlUnitEmergencyRegisterTriage), LogTriageRegisterQueueLength);
            CollectingMethodsPerControlUnitType.Add(typeof(ContorlUnitAssessmentTreatmentExample), LogAssessmentTreatmentQueueLength);
        } // end of

        #endregion

        //--------------------------------------------------------------------------------------------------
        // EventHandling Methods
        //--------------------------------------------------------------------------------------------------

        #region PatientArrivingAtEmergency

        /// <summary>
        /// Collects data for patient arriving events, in this example the arriving time
        /// per patient is stored
        /// </summary>
        /// <param name="ev">Arriving event of patient</param>
        /// <param name="time">Time event was triggered</param>
        public void PatientArrivingAtEmergency(Event ev, DateTime time)
        {
            EventEmergencyPatientArrival arrivingEvent = ev as EventEmergencyPatientArrival;

            if (!arrivingEvent.Patient.DataEntries.ContainsKey("PatientArrivalAtEmergency"))
                arrivingEvent.Patient.DataEntries.Add("PatientArrivalAtEmergency", time);

        } // end PatientArrivingAtEmergency

        #endregion

        #region PatientLeavingAtEmergency

        /// <summary>
        /// Collects data for patient leaving events, in this example the leaving time
        /// per patient is stored
        /// </summary>
        /// <param name="ev">LEaving event of patient</param>
        /// <param name="time">Time event was triggered</param>
        public void PatientLeavingAtEmergency(Event ev, DateTime time)
        {
            EventEmergencyPatientLeave leavingEvent = ev as EventEmergencyPatientLeave;

            leavingEvent.Patient.DataEntries.Add("PatientLeaveAtEmergency", time);

        } // end PatientLeavingAtEmergency

        #endregion

        #region PatientFirstSeenByDoctor

        /// <summary>
        /// Start event of first assessment activity of patients is stored here
        /// </summary>
        /// <param name="activity">Activity that is triggered</param>
        /// <param name="time">Time activity is triggered</param>
        public void PatientFirstSeenByDoctor(Activity activity, DateTime time)
        {
            if (activity.GetType() == typeof(ActivityHealthCareAction<EmergencyActionTypeClass>)
                && ((ActivityHealthCareAction<EmergencyActionTypeClass>)activity).ActionType.Type == "Assessment")
            {
                ((ActivityHealthCareAction<EmergencyActionTypeClass>)activity).Patient.DataEntries.Add("FirstAssessment", time);
            } // end if
        } // end of PatientFirstSeenByDoctor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Handling Methods
        //--------------------------------------------------------------------------------------------------

        #region LogTriageRegisterQueueLength

        /// <summary>
        /// Collects data for a triage and register control unit, stores queue lengths at time
        /// </summary>
        /// <param name="controlUnit">Triage and register control unit</param>
        /// <param name="time">Time data is collected</param>
        public void LogTriageRegisterQueueLength(ControlUnit controlUnit, DateTime time)
        {
            Dictionary<string, int> queueLengths = new Dictionary<string,int>();

            List<ActivityRequest> triageRequests = controlUnit.RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Triage").ToList();

            queueLengths.Add("Triage", triageRequests.Count);

            List<ActivityRequest> registerRequests = controlUnit.RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Register").ToList();

            queueLengths.Add("Register", registerRequests.Count);

            controlUnit.StateData.Add(time, queueLengths);

        } // end of LogCurrentQueueLength

        #endregion

        #region LogAssessmentTreatmentQueueLength

        /// <summary>
        /// Collects data for a assessment and treatment control unit, stores queue lengths at time
        /// </summary>
        /// <param name="controlUnit">Assessment and Treatment control unit</param>
        /// <param name="time">Time data is collected</param>
        public void LogAssessmentTreatmentQueueLength(ControlUnit controlUnit, DateTime time)
        {
            Dictionary<string, int> queueLengths = new Dictionary<string, int>();

            List<ActivityRequest> triageRequests = controlUnit.RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Assessment").ToList();

            queueLengths.Add("Assessment", triageRequests.Count);

            List<ActivityRequest> registerRequests = controlUnit.RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Treatment").ToList();

            queueLengths.Add("Treatment", registerRequests.Count);

            controlUnit.StateData.Add(time, queueLengths);

        } // end of LogCurrentQueueLength

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region CreateSimulationResult

        /// <summary>
        /// Creates a simulation result, computes statistic measures for LOS, time to be first seen by a doctor
        /// and average queuqe lengths
        /// </summary>
        public override void CreateSimulationResult()
        {
            #region LOS

            List<double> allLengthOfStays = new List<double>();

            foreach (EntityPatient patient in ParentSimulationModel.GetEntitiesOfType<EntityPatient>())
            {
                if (patient.DataEntries.ContainsKey("PatientArrivalAtEmergency") && patient.DataEntries.ContainsKey("PatientLeaveAtEmergency"))
                    allLengthOfStays.Add((((DateTime)patient.DataEntries["PatientLeaveAtEmergency"]) - ((DateTime)patient.DataEntries["PatientArrivalAtEmergency"])).TotalHours);
            } // end foreach

            #endregion

            #region TimeToBeSeenByDoctor

            List<double> allTimeToFirstAssessment = new List<double>();

            foreach (EntityPatient patient in ParentSimulationModel.GetEntitiesOfType<EntityPatient>())
            {
                if (patient.DataEntries.ContainsKey("PatientArrivalAtEmergency") && patient.DataEntries.ContainsKey("FirstAssessment"))
                    allTimeToFirstAssessment.Add((((DateTime)patient.DataEntries["FirstAssessment"]) - ((DateTime)patient.DataEntries["PatientArrivalAtEmergency"])).TotalHours);
             
            } // end foreach

            #endregion

            #region AverageQueueLengthTriageRegister
            
            #endregion

            StatisticsSample losStatistic = new StatisticsSample(allLengthOfStays, ConfidenceIntervalTypes.StandardDeviation);
            StatisticsSample timeToFirstAssessmentStatistic = new StatisticsSample(allTimeToFirstAssessment, ConfidenceIntervalTypes.StandardDeviation);

            
        } // end of CreateSimulationResult

        #endregion
        
    } // end of SampleOutputGeneration
}

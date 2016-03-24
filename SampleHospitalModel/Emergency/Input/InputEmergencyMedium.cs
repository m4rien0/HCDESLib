using GeneralHealthCareElements;
using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.Emergency
{
    /// <summary>
    /// Sample input for a emergncy control extending the general xml input 
    /// </summary>
    public class InputEmergency : GenericXMLDepartmentInput, IInputEmergency
    {

        //--------------------------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic contructor
        /// </summary>
        /// <param name="xmlInput">Xml input that defines most of input</param>
        public InputEmergency(XMLInputHealthCareDepartment xmlInput) : base(xmlInput)
        {

        } // end of InputEmergencyMedium

	    #endregion
        
        //--------------------------------------------------------------------------------------------------
        // Interface Methods 
        //--------------------------------------------------------------------------------------------------

        #region EmergencyPatientArrivalTime

        /// <summary>
        /// Defines arrival stream of patients, currently exponential inter-arrival times
        /// depending on the time of the day
        /// </summary>
        /// <param name="time">Current time</param>
        /// <returns>Time span to next arrival time from the current time</returns>
        public TimeSpan PatientArrivalTime(DateTime time)
        {
            if (time.TimeOfDay < TimeSpan.FromHours(20) && time.TimeOfDay >= TimeSpan.FromHours(8))
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(40));
            else
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(50));
        } // end of PatientArrivalTime


        #endregion

        #region PatientActionTime

        /// <summary>
        /// Activity duriations, here modeled with exponential distributions independent from the patient conditions,
        /// just dependent on the action performed
        /// </summary>
        /// <param name="patient">Patient participating action</param>
        /// <param name="resources">Resources involved in action</param>
        /// <param name="actionType">Type of action</param>
        /// <returns>Time span that represent the duration of the action</returns>
        public override TimeSpan PatientActionTime(EntityPatient patient,
            ResourceSet resources,
            ActionTypeClass actionType)
        {

            if (actionType.Type == "BedPlacement")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(1));
            if (actionType.Type == "Triage")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(5));
            if (actionType.Type == "Register")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(5));
            if (actionType.Type == "Assessment")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(10));
            if (actionType.Type == "Treatment")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(15));

            return TimeSpan.FromMinutes(Distributions.Instance.Exponential(15));
        } // end of PatientEmergencyTreatmentTime

        #endregion

        #region GetNextPatient

        /// <summary>
        /// The patient part of the arrival stream, defines patient category and other attributes of arriving
        /// patients
        /// </summary>
        /// <returns>The next patient to arrive</returns>
        public EntityPatient GetNextPatient()
        {
            PatientClass newPatientClass = (PatientClass)PatientClassPerXmlPatientClass[PatientClassDistribution.GetRandomValue()].Clone();

            return new EntityPatient(EntityPatient.RunningPatientID++, newPatientClass);
        } // end of PatientArrivalTime

        #endregion

        #region CreateEmergencyPath

        /// <summary>
        /// Method for patient path creation, uses core path method from base class
        /// and casts action list for emergency department
        /// </summary>
        /// <param name="patient">Patient the path is created for</param>
        /// <returns>Patient path</returns>
        public EmergencyPatientPath CreateEmergencyPath(EntityPatient patient)
        {
            List<ActionTypeClass> actions;
            Admission outpatientAdmission;
            Admission inpatientAdmission;

            GetCorePath(patient, null, out actions, out outpatientAdmission, out inpatientAdmission);

            return new EmergencyPatientPath(actions.Select(p=> new EmergencyActionTypeClass(p)).ToList(), inpatientAdmission, outpatientAdmission, patient);
          
        } // end of CreateEmergencyPath

        #endregion

    } // end of InputEmergency
}

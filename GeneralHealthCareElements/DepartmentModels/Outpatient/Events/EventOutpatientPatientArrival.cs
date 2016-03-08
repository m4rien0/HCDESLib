using SimulationCore.MathTool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimulationCore.HCCMElements;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.SimulationClasses;
using GeneralHealthCareElements.Activities;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Arrival event of patient to the actual outpatient clinic, walk in patient are handled with a
    /// different event type. This event is intented for returns of special services or
    /// from waiting list arrivals
    /// </summary>
    public class EventOutpatientArrival : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="parentControlUnit">Parent outpatient control</param>
        /// <param name="patient">Arriving patient</param>
        /// <param name="scheduledTime">Scheduled time of arrival</param>
        /// <param name="inputData">Corresponding input data</param>
        /// <param name="admission">Corresponding admission of arrival</param>
        public EventOutpatientArrival(ControlUnit parentControlUnit, 
            EntityPatient patient, 
            DateTime scheduledTime,
            IInputOutpatient inputData,
            Admission admission)
            : base(EventType.Standalone, parentControlUnit)
        {
            _scheduledTime = scheduledTime;
            _patient = patient;
            _inputData = inputData;
            _admission = admission;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// Acutal state change of arrival. If patient arrives from waiting list path is set, otherwise
        /// next action is taken.
        /// </summary>
        /// <param name="time">Time the patient arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {

            //--------------------------------------------------------------------------------------------------
            // In case Patient is returning from special service facility the path
            // has already been set and needs update to next action
            //--------------------------------------------------------------------------------------------------
            if (Patient.OutpatientTreatmentPath != null)
            {
                Patient.OutpatientTreatmentPath.UpdateNextAction();
            }
            else
            {
                Patient.OutpatientTreatmentPath = InputData.CreateOutpatientTreatmentPath(Patient, Admission, ScheduledTime, false);

                ParentControlUnit.AddEntity(Patient);
            } // end if

            if (Patient.OutpatientTreatmentPath.TakeNextAction(simEngine, this, time, ParentControlUnit))
            {
                if (Patient.OccupiedFacility == null || Patient.OccupiedFacility.ParentDepartmentControl != ParentControlUnit)
                {
                    SequentialEvents.Add(Patient.StartWaitingActivity(((ControlUnitOutpatient)ParentControlUnit).WaitingAreaPatientForNextActionType(Patient.OutpatientTreatmentPath.GetCurrentActionType())));
                }
                else
                {
                    ActivityWaitInFacility waitInFacility = new ActivityWaitInFacility(ParentControlUnit, Patient, Patient.OccupiedFacility);
                    SequentialEvents.Add(waitInFacility.StartEvent);
                } // end if
            } // end if

        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Arriving patient
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion

        #region AffectedEntites

        /// <summary>
        /// Affected entities include only patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { Patient };
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputOutpatient _inputData;

        /// <summary>
        /// Corresponding input data
        /// </summary>
        public IInputOutpatient InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion

        #region ScheduledTime

        private DateTime _scheduledTime;

        /// <summary>
        /// Scheduled time of arrival, may vary from actual arrival time
        /// </summary>
        public DateTime ScheduledTime
        {
            get
            {
                return _scheduledTime;
            }
        } // end of ScheduledTime

        #endregion                

        #region Admission

        private Admission _admission;

        /// <summary>
        /// Admission associated with arriving patient
        /// </summary>
        public Admission Admission
        {
            get
            {
                return _admission;
            }
            set
            {
                _admission = value;
            }
        } // end of Admission

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventOutpatientArrival";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventOutpatientArrival(ParentControlUnit, (EntityPatient)Patient.Clone(), ScheduledTime, InputData, Admission);
        } // end of Clone

        #endregion
    }
}

using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList
{
    /// <summary>
    /// Waiting activity for getting a slot assigned (e.g. if admissions are handled only at certain times)
    /// </summary>
    public class ActivityOutpatientWaitingListWaitToAssignSlot : Activity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent waiting list control unit</param>
        /// <param name="patient">Waiting patient</param>
        /// <param name="admissionType">Admission type for slot request</param>
        /// <param name="earliestTime">Earliest possible slot time, e.g. follow up not before a week passed</param>
        /// <param name="latestTime">Latest possible time for slot</param>
        public ActivityOutpatientWaitingListWaitToAssignSlot(ControlUnit parentControlUnit,
            EntityPatient patient,
            Admission admissionType,
            DateTime earliestTime,
            DateTime latestTime)
            : base(parentControlUnit, "ActivityOutpatientWaitingListWaitToAssignSlot", false)
        {
            _patient = patient;
            _admissionType = admissionType;
            _earliestTime = earliestTime;
        } // end of Activity

        #endregion Constructor

        #region Name

        public static string Name = "ActivityOutpatientWaitingListWaitToAssignSlot";

        #endregion Name

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Waiting patient
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion Patient

        #region AffectedEntites

        /// <summary>
        /// Affected entities include only patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return Patient.ToArray();
            }
        } // end of AffectedEntities

        #endregion AffectedEntites

        //--------------------------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------------------------

        #region EarliestTime

        private DateTime _earliestTime;

        /// <summary>
        /// Earliest possible time for slot to book
        /// </summary>
        public DateTime EarliestTime
        {
            get
            {
                return _earliestTime;
            }
        } // end of EarliestTime

        #endregion EarliestTime

        #region LatestTime

        private DateTime _latestTime;

        /// <summary>
        /// Latest possible time for slot to book
        /// </summary>
        public DateTime LatestTime
        {
            get
            {
                return _latestTime;
            }
        } // end of LatestTime

        #endregion LatestTime

        #region AdmissionType

        private Admission _admissionType;

        public Admission AdmissionType
        {
            get
            {
                return _admissionType;
            }
        } // end of AdmissionType

        #endregion AdmissionType

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// State changes of the activities start event. Request for slot assigning is filed
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            RequestOutpatientWaitingListPatientToAssignSlot requestAssign
                = new RequestOutpatientWaitingListPatientToAssignSlot(Patient,
                    time,
                    AdmissionType,
                    EarliestTime,
                    LatestTime);
            ParentControlUnit.AddRequest(requestAssign);
        } // end of TriggerStartEvent

        #endregion TriggerStartEvent

        #region TriggerEndEvent

        /// <summary>
        /// State changes of the activities end event. No functionality
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
        } // end of TriggerEndEvent

        #endregion TriggerEndEvent

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return Name;
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Activity Clone()
        {
            return new ActivityOutpatientWaitingListWaitToAssignSlot(ParentControlUnit, (EntityPatient)Patient.Clone(), AdmissionType, EarliestTime, LatestTime);
        } // end of Clone

        #endregion Clone
    }
}
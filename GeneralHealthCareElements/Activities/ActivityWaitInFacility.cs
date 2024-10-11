using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.Activities
{
    /// <summary>
    /// A waiting activity of patients where a treatment facility is used as a waiting area
    /// and remains occupied
    /// </summary>
    public class ActivityWaitInFacility : Activity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Control unit to host acitivity</param>
        /// <param name="patient">Patient that is waiting</param>
        /// <param name="facility">Facility occupied by waiting patient</param>
        public ActivityWaitInFacility(ControlUnit parentControlUnit,
            EntityPatient patient,
            EntityTreatmentFacility facility)
            : base(parentControlUnit, "ActivityWaitInFacility", false)
        {
            _patient = patient;
            _facility = facility;
        } // end of Activity

        #endregion Constructor

        #region Name

        public static string Name = "ActivityWaitInFacility";

        #endregion Name

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// No state change occur
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
        } // end of TriggerStartEvent

        #endregion TriggerStartEvent

        #region TriggerEndEvent

        /// <summary>
        /// No state change occur
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
        } // end of TriggerEndEvent

        #endregion TriggerEndEvent

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

        #region Facility

        private EntityTreatmentFacility _facility;

        /// <summary>
        /// Facility occupied by waiting patient
        /// </summary>
        public EntityTreatmentFacility Facility
        {
            get
            {
                return _facility;
            }
            set
            {
                _facility = value;
            }
        } // end of Facility

        #endregion Facility

        #region AffectedEntites

        /// <summary>
        /// Affected entities including patient and facility
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { Patient, Facility };
            }
        } // end of AffectedEntities

        #endregion AffectedEntites

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
            return new ActivityWaitInFacility(ParentControlUnit,
                (EntityPatient)Patient.Clone(),
                (EntityTreatmentFacility)Facility.Clone());
        } // end of Clone

        #endregion Clone
    } // end of ActivityWaitInFacility
}
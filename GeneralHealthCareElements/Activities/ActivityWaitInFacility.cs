using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        #endregion

        #region Name

        public static string Name = "ActivityWaitInFacility";

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// No state change occur
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            
        } // end of TriggerStartEvent

        #endregion

        #region TriggerEndEvent

        /// <summary>
        /// No state change occur
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
          
        } // end of TriggerEndEvent

        #endregion

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

        #endregion

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

        #endregion
        
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

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return Name;
        } // end of ToString

        #endregion

        #region Clone

        public override Activity Clone()
        {
            return new ActivityWaitInFacility(ParentControlUnit,
                (EntityPatient)Patient.Clone(),
                (EntityTreatmentFacility)Facility.Clone());
        } // end of Clone

        #endregion
        
        
    } // end of ActivityWaitInFacility
}

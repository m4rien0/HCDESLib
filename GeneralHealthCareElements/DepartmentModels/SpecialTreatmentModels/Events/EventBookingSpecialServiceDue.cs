using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Management;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Event that represents the due time of a slot for a service request
    /// </summary>
    public class EventBookingSpecialServiceDue : Event
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent control unit</param>
        /// <param name="patient">Patient requiring service</param>
        /// <param name="returnDelegate">Possible return delegate</param>
        /// <param name="availability">Availabity delegate associated</param>
        public EventBookingSpecialServiceDue(ControlUnit parentControlUnit,
            EntityPatient patient,
            RequestSpecialFacilitiyService returnDelegate,
            DelegateAvailabilitiesForRequest availability)
            : base(EventType.Standalone, parentControlUnit)
        {
            _patient = patient;
            _returnDelegate = returnDelegate;
            _availability = availability;
        } // end of Event

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// State change of event. Sends patient to the special service model
        /// </summary>
        /// <param name="time">Time event is triggered</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            ControlUnitManagement jointControl = (ControlUnitManagement)ParentControlUnit.FindSmallestJointControl(Availability.ServiceControl);

            if (Patient != null)
            {
                ActivityMove movePatientToSpecialTreatment
                    = new ActivityMove(jointControl,
                                       Patient,
                                       ParentControlUnit,
                                       Availability.ServiceControl,
                                       ReturnDelegate,
                                       jointControl.InputData.DurationMove(Patient,
                                           ParentControlUnit,
                                           Availability.ServiceControl));
                SequentialEvents.Add(movePatientToSpecialTreatment.StartEvent);
            } // end if
        } // end of Trigger

        #endregion Trigger

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient requiring service
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
                return new Entity[] { Patient };
            }
        } // end of AffectedEntities

        #endregion AffectedEntites

        //--------------------------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------------------------

        #region Availability

        private DelegateAvailabilitiesForRequest _availability;

        /// <summary>
        /// Availabity delegate associated
        /// </summary>
        public DelegateAvailabilitiesForRequest Availability
        {
            get
            {
                return _availability;
            }
        } // end of Availability

        #endregion Availability

        #region ReturnDelegate

        private RequestSpecialFacilitiyService _returnDelegate;

        /// <summary>
        /// Possible return delegate
        /// </summary>
        public RequestSpecialFacilitiyService ReturnDelegate
        {
            get
            {
                return _returnDelegate;
            }
        } // end of ReturnDelegate

        #endregion ReturnDelegate

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventBookingSpecialServiceDue";
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Event Clone()
        {
            return new EventBookingSpecialServiceDue(ParentControlUnit, (EntityPatient)Patient.Clone(), ReturnDelegate, Availability);
        } // end of Clone

        #endregion Clone
    } // end of EventBookingSpecialServiceDue
}
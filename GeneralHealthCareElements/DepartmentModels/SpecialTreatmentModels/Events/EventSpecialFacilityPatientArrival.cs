using Enums;
using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Event that is triggered when patient arrives at special service department
    /// </summary>
    public class EventSpecialFacilityPatientArrival : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent special service control</param>
        /// <param name="patient">Arriving patient</param>
        /// <param name="originalRequest">Service request associated with arrival</param>
        /// <param name="input">Special service input data</param>
        public EventSpecialFacilityPatientArrival(ControlUnit parentControlUnit, 
            EntityPatient patient,
            RequestSpecialFacilitiyService originalRequest,
            IInputSpecialFacility input)
            : base(EventType.Standalone, parentControlUnit)
        {
            _patient = patient;
            _originalRequest = originalRequest;
            _inputData = input;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// Overriden state change of the event. The patient path is set and the first action is taken.
        /// </summary>
        /// <param name="time">Time the patient arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            Patient.SpecialFacilityPath = InputData.CreatePatientPath(OriginalRequest.SpecialFacilityAdmissionTypes, Patient, OriginalRequest);

            if (Patient.SpecialFacilityPath.TakeNextAction(simEngine, this, time, ParentControlUnit))
            {
                ActivityWait waitPatient = new ActivityWait(ParentControlUnit,
                                        Patient,
                                        (((ControlUnitSpecialServiceModel)ParentControlUnit).WaitingAreaPatientForNextActionType(Patient.SpecialFacilityPath.GetCurrentActionType())));


                SequentialEvents.Add(waitPatient.StartEvent);
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
        /// Affected entities only consist of the patient
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

        private IInputSpecialFacility _inputData;

        /// <summary>
        /// Special service input data
        /// </summary>
        public IInputSpecialFacility InputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
            }
        } // end of InputData

        #endregion

        #region OriginalRequest

        private RequestSpecialFacilitiyService _originalRequest;

        /// <summary>
        /// Service request associated with arrival
        /// </summary>
        public RequestSpecialFacilitiyService OriginalRequest
        {
            get
            {
                return _originalRequest;
            }
        } // end of OriginalRequest

        #endregion
        
        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventDiagnosticsPatientArrival";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventSpecialFacilityPatientArrival(ParentControlUnit, (EntityPatient)Patient.Clone(), OriginalRequest, InputData);
        } // end of Clone

        #endregion

    } // end of EventDiagnosticsPatientArrival
}

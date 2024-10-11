using Enums;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Delegates;
using GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Events;
using GeneralHealthCareElements.Management;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;
using System;
using System.Linq;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Base class for outpatient department control units
    /// </summary>
    public abstract class ControlUnitOutpatient : ControlUnitHealthCareDepartment
    {
        //--------------------------------------------------------------------------------------------------
        // Constructing
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor of emergency department controls
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent management control unit</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Outpatient input data</param>
        /// <param name="handledOutpatientAdmissionTypes">Outpatient admission types that are handled by the control unit</param>
        /// <param name="waitinListControlUnit">Corresponding waiting list control unit</param>
        public ControlUnitOutpatient(string name,
            ControlUnit parentControlUnit,
            OutpatientAdmissionTypes[] handledOutpatientAdmissionTypes,
            SimulationModel parentSimulationModel,
            ControlUnit waitinListControlUnit,
            IInputOutpatient inputData)
            : base(ControlUnitType.Outpatient,
                   name,
                   parentControlUnit,
                   parentSimulationModel,
                   inputData)
        {
            if (handledOutpatientAdmissionTypes != null)
                _handledOutpatientAdmissionTypes = handledOutpatientAdmissionTypes.ToArray();
            else
                _handledOutpatientAdmissionTypes = Helpers<OutpatientAdmissionTypes>.EmptyArray();

            _waitingListControlUnit = waitinListControlUnit;
            _inputData = inputData;

            _delegateHandlingMethods.Add(typeof(DelegateAvailabilitiesForRequest), DefaultDelegateHandling.HandleImmediateSpecialServiceRequest);
        } // end of ControlUnitOutpatient

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Overrides basic initialiing method. Besides standard operations a possible walk in stream of
        /// patients is
        /// </summary>
        /// <param name="startTime">Time the simulation starts</param>
        /// <param name="simEngine">SimEngine that handles the simulation run</param>
        public override void Initialize(DateTime startTime, ISimulationEngine simEngine)
        {
            base.Initialize(startTime, simEngine);

            EntityPatient nextPatient = null;
            DateTime nextArrivalTime;

            if ((nextPatient = InputData.GetNextWalkInPatient(out nextArrivalTime, this, startTime)) != null)
            {
                EventOutpatientWalkInPatientArrival nextArrival = new EventOutpatientWalkInPatientArrival(this, InputData, nextPatient);

                simEngine.AddScheduledEvent(nextArrival, nextArrivalTime);
            } // end if
        } // end of CustomInitialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Handled Treatments and Admissions
        //--------------------------------------------------------------------------------------------------

        #region HandledOutpatientAdmissionTypes

        private OutpatientAdmissionTypes[] _handledOutpatientAdmissionTypes;

        /// <summary>
        /// Handled outpatient admission types by the control
        /// </summary>
        public override OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes
        {
            get
            {
                return _handledOutpatientAdmissionTypes;
            }
        } // end of HandledOutpatientAdmissionTypes

        #endregion HandledOutpatientAdmissionTypes

        #region HandledInpatientAdmissionTypes

        /// <summary>
        /// Overriden handled inpatient admissions, returns empty array as this
        /// is an outpatient control
        /// </summary>
        public override InpatientAdmissionTypes[] HandledInpatientAdmissionTypes
        {
            get
            {
                return Helpers<InpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledInpatientAdmissionTypes

        #endregion HandledInpatientAdmissionTypes

        #region HandledSpecialFacilityAdmissionTypes

        /// <summary>
        /// Overriden handled special service admissions, returns empty array as this
        /// is an outpatient control
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                return Helpers<SpecialServiceAdmissionTypes>.EmptyArray();
            }
        } // end of HandledSpecialFacilityAdmissionTypes

        #endregion HandledSpecialFacilityAdmissionTypes

        #region WaitingListControlUnit

        private ControlUnit _waitingListControlUnit;

        /// <summary>
        /// The corresponding waiting list control unit for the outpatient control
        /// </summary>
        public ControlUnit WaitingListControlUnit
        {
            get
            {
                return _waitingListControlUnit;
            }
        } // end of WaitingListControlUnits

        #endregion WaitingListControlUnit

        //--------------------------------------------------------------------------------------------------
        // Input
        //--------------------------------------------------------------------------------------------------

        #region InputData

        protected IInputOutpatient _inputData;

        /// <summary>
        /// Outpatient input data
        /// </summary>
        public new IInputOutpatient InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion InputData

        //--------------------------------------------------------------------------------------------------
        // Enter Leave
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            if (entity is EntityHealthCareStaff)
                return new EventControlUnitStaffEnterLeave(this, true, (EntityStaff)entity, originDelegate, WaitingRoomForStaff((EntityHealthCareStaff)entity));

            EntityPatient patient = (EntityPatient)entity;

            if (originDelegate is RequestSpecialFacilitiyService)
            {
                return new EventOutpatientArrival(this,
                    patient,
                    time,
                    InputData,
                    null);
            }
            else if (originDelegate is RequestMoveOutpatient)
            {
                RequestMoveOutpatient outDel = (RequestMoveOutpatient)originDelegate;
                return new EventOutpatientWaitingListPatientArrival(
                    WaitingListControlUnit,
                    this,
                    patient,
                    outDel.OutpatientAdmission,
                    InputData);
            } // end if

            return null;
        } // end of EntityEnterControlUnit

        #endregion EntityEnterControlUnit

        #region EntityLeaveControlUnit

        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
        } // end of EntityLeaveControlUnit

        #endregion EntityLeaveControlUnit
    } // end of ControlUnitOutpatient
}
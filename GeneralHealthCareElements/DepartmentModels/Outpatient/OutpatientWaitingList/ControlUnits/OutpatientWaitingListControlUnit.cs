using Enums;
using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList
{
    /// <summary>
    /// Base class for outpatient waiting list controls
    /// </summary>
    public abstract class OutpatientWaitingListControlUnit : ControlUnitHealthCare
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent outpatient control</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="input">Corresponding outpatient input data</param>
        /// <param name="assigningAtEvents">Flag if slots are assigned at events only, or immediately</param>
        public OutpatientWaitingListControlUnit(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            IInputOutpatient input,
                            bool assigningAtEvents)
            : base(ControlUnitType.OutpatientWaitingList, name, parentControlUnit, parentSimulationModel)
        {
            _inputData = input;
            _assigningSlotAtEvents = assigningAtEvents;
            _doDispatching = false;
            _waitingListSchedule = input.GetWaitingListSchedule();
        }

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Entities
        //--------------------------------------------------------------------------------------------------

        #region WaitingListSchedule

        protected EntityWaitingListSchedule _waitingListSchedule;

        /// <summary>
        /// Associated waiting list schedule
        /// </summary>
        public EntityWaitingListSchedule WaitingListSchedule
        {
            get
            {
                return _waitingListSchedule;
            }
        } // end of EntitiyWaitingListSchedule

        #endregion WaitingListSchedule

        //--------------------------------------------------------------------------------------------------
        // Controlled Treatments
        //--------------------------------------------------------------------------------------------------

        #region HandledOutpatientAdmissionTypes

        /// <summary>
        /// Overriden handled outpatient admissions, returns empty array as this
        /// is an waiting list control
        /// </summary>
        public override OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes
        {
            get
            {
                return Helpers<OutpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledOutpatientAdmissionTypes

        #endregion HandledOutpatientAdmissionTypes

        #region HandledInpatientAdmissionTypes

        /// <summary>
        /// Overriden handled inpatient admissions, returns empty array as this
        /// is an waiting list control
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
        /// is an waiting list control
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                return Helpers<SpecialServiceAdmissionTypes>.EmptyArray();
            }
        } // end of HandledSpecialFacilityAdmissionTypes

        #endregion HandledSpecialFacilityAdmissionTypes

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region SetParentControlUnit

        /// <summary>
        /// Method to set the parent outpatient control
        /// </summary>
        /// <param name="parentControl"></param>
        public void SetParentControlUnit(ControlUnit parentControl)
        {
            _parentControlUnit = parentControl;
        } // end of SetParentControlUnit

        #endregion SetParentControlUnit

        //--------------------------------------------------------------------------------------------------
        // Input
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputOutpatient _inputData;

        /// <summary>
        /// Outpatient input data
        /// </summary>
        public IInputOutpatient InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion InputData

        #region AssigningSlotsAtEvents

        private bool _assigningSlotAtEvents;

        /// <summary>
        /// Flag if slots are assigned at events only, or immediately
        /// </summary>
        public bool AssigningSlotsAtEvents
        {
            get
            {
                return _assigningSlotAtEvents;
            }
            set
            {
                _assigningSlotAtEvents = value;
            }
        } // end of AssigningSlotsAtEvents

        #endregion AssigningSlotsAtEvents

        #region DoDispatching

        private bool _doDispatching;

        /// <summary>
        /// Flag if dispatching is due
        /// </summary>
        public bool DoDispatching
        {
            get
            {
                return _doDispatching;
            }
            set
            {
                _doDispatching = value;
            }
        } // end of DoDispatching

        #endregion DoDispatching
    }
}
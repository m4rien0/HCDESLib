using Enums;
using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Delegates;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Base class for special service control units
    /// </summary>
    public abstract class ControlUnitSpecialServiceModel : ControlUnitHealthCareDepartment
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent management control unit</param>
        /// <param name="handledAdmissions">Handled special service admissions</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="waitingListSchedule"></param>
        /// <param name="inputData">Special service data</param>
        public ControlUnitSpecialServiceModel(string name,
            ControlUnit parentControlUnit,
            SpecialServiceAdmissionTypes[] handledAdmissions,
            SimulationModel parentSimulationModel,
            EntityWaitingListSchedule waitingListSchedule,
            IInputSpecialFacility inputData)
            : base(ControlUnitType.SpecialFacilityModel,
                   name,
                   parentControlUnit,
                   parentSimulationModel,
                   inputData)
        {
            _delegateHandlingMethods.Add(typeof(RequestSpecialFacilitiyService), DefaultDelegateHandling.BookImmediateServiceRequestSpecialTreatmentModel);
            _waitintListSchedule = waitingListSchedule;

            _handledSpecialFacilitysAdmissions = handledAdmissions;

            _inputData = inputData;
        } // end of ControlUnitDiagnostics

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // HandledTreatments
        //--------------------------------------------------------------------------------------------------

        #region HandledSpecialFacilityAdmissionTypes

        private SpecialServiceAdmissionTypes[] _handledSpecialFacilitysAdmissions;

        /// <summary>
        /// Handled special service admission types by the control
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                return _handledSpecialFacilitysAdmissions;
            }
        } // end of HandledSpecialFacilityAdmissionTypes

        #endregion HandledSpecialFacilityAdmissionTypes

        #region HandledOutpatientAdmissionTypes

        /// <summary>
        /// Overriden handled outpatient admissions, returns empty array as this
        /// is a special service control
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
        /// is a special service control
        /// </summary>
        public override InpatientAdmissionTypes[] HandledInpatientAdmissionTypes
        {
            get
            {
                return Helpers<InpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledInpatientAdmissionTypes

        #endregion HandledInpatientAdmissionTypes

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region WaitingListSchedule

        private EntityWaitingListSchedule _waitintListSchedule;

        /// <summary>
        /// Waiting list schedule used by special service control
        /// </summary>
        public EntityWaitingListSchedule WaitingListSchedule
        {
            get
            {
                return _waitintListSchedule;
            }
        } // end of WaitingListSchedule

        #endregion WaitingListSchedule

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
        } // end of InputData

        #endregion InputData
    } // end of ControlUnitDiagnostics
}
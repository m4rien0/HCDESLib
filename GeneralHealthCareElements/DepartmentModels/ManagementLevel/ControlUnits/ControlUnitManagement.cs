using Enums;
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Delegates;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.Management
{
    /// <summary>
    /// Base control unit for management control units
    /// </summary>
    abstract public class ControlUnitManagement : ControlUnitHealthCare
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, adds default delegate handling methods for RequestMoveOutpatient,
        /// RequestMoveInpatient, RequestSpecialFacilitiyService and RequestSpecialFacilitiyService
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control, equals null if this control is root</param>
        /// <param name="parentSimulationModel">Parent simulation control</param>
        public ControlUnitManagement(string name,
            ControlUnit parentControlUnit,
            SimulationModel parentSimulationModel)
            : base(ControlUnitType.Management,
                    name,
                    parentControlUnit,
                    parentSimulationModel)
        {
            _delegateHandlingMethods.Add(typeof(RequestMoveOutpatient), DefaultDelegateHandling.HandleMoveOutpatient);
            _delegateHandlingMethods.Add(typeof(RequestMoveInpatient), DefaultDelegateHandling.HandleMoveInpatient);
            _delegateHandlingMethods.Add(typeof(RequestSpecialFacilitiyService), DefaultDelegateHandling.ForwardServiceRequestSpecialTreatmentModel);
            _delegateHandlingMethods.Add(typeof(DelegateRequestDocsForAssisting), DefaultDelegateHandling.HandleRequireDocs);
        } // end of ControlUnitOutpatient

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // TreatmentsHandled
        //--------------------------------------------------------------------------------------------------

        #region HandledInpatientAdmissionTypes

        private InpatientAdmissionTypes[] _handledInpatientAdmissionTypes = null;

        /// <summary>
        /// Handled Inpatient Admissions, contain all admission types handled by controls of
        /// the sub-tree
        /// </summary>
        public override InpatientAdmissionTypes[] HandledInpatientAdmissionTypes
        {
            get
            {
                if (_handledInpatientAdmissionTypes == null)
                {
                    List<InpatientAdmissionTypes> allAdmissions = new List<InpatientAdmissionTypes>();

                    foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                    {
                        allAdmissions.AddRange(childControl.HandledInpatientAdmissionTypes);
                        _handledInpatientAdmissionTypes = allAdmissions.ToArray();
                    } // end foreach
                } // end if

                return _handledInpatientAdmissionTypes;
            }
        } // end of HandledInpatientAdmissionTypes

        #endregion HandledInpatientAdmissionTypes

        #region HandledOutpatientAdmissionTypes

        private OutpatientAdmissionTypes[] _handledOutpatientAdmissionTypes = null;

        /// <summary>
        /// Handled Outpatient Admissions, contain all admission types handled by controls of
        /// the sub-tree
        /// </summary>
        public override OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes
        {
            get
            {
                if (_handledOutpatientAdmissionTypes == null)
                {
                    List<OutpatientAdmissionTypes> allAdmissions = new List<OutpatientAdmissionTypes>();

                    foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                    {
                        allAdmissions.AddRange(childControl.HandledOutpatientAdmissionTypes);
                        _handledOutpatientAdmissionTypes = allAdmissions.ToArray();
                    } // end foreach
                } // end if

                return _handledOutpatientAdmissionTypes;
            }
        } // end of HandledOutpatientAdmissionTypes

        #endregion HandledOutpatientAdmissionTypes

        #region HandledDiagnosticsTreatments

        private SpecialServiceAdmissionTypes[] _handledDiagnosticsTreatments = null;

        /// <summary>
        /// Handled Special Facility Admissions, contain all admission types handled by controls of
        /// the sub-tree
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                if (_handledDiagnosticsTreatments == null)
                {
                    List<SpecialServiceAdmissionTypes> allAdmissions = new List<SpecialServiceAdmissionTypes>();

                    foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                    {
                        allAdmissions.AddRange(childControl.HandledSpecialFacilityAdmissionTypes);
                        _handledDiagnosticsTreatments = allAdmissions.ToArray();
                    } // end foreach
                } // end if

                return _handledDiagnosticsTreatments;
            }
        } // end of HandledDiagnosticsTreatments

        #endregion HandledDiagnosticsTreatments

        #region InputData

        /// <summary>
        /// Input data of management control
        /// </summary>
        public abstract IInputManagement InputData { get; }

        #endregion InputData

        #region CurrentMovingActivities

        /// <summary>
        /// Current moving activities hosted by this management control
        /// </summary>
        public List<ActivityMove> CurrentMovingActivities
        {
            get
            {
                List<ActivityMove> currentMovingActivities = CurrentActivities.Where(p => p.ActivityName == "ActivityMove").Cast<ActivityMove>().ToList();

                foreach (ControlUnit childControl in ChildControlUnits)
                {
                    if (childControl is ControlUnitManagement)
                        continue;

                    currentMovingActivities.AddRange(childControl.CurrentActivities.Where(p => p.ActivityName == "ActivityMove").Cast<ActivityMove>());
                } // end foreach

                return currentMovingActivities;
            }
        } // end for

        #endregion CurrentMovingActivities

        //--------------------------------------------------------------------------------------------------
        // Enter Leave
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        /// <param name="entity"></param>
        /// <param name="originDelegate"></param>
        /// <returns></returns>
        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityEnterControlUnit

        #endregion EntityEnterControlUnit

        #region EntityLeaveControlUnit

        /// <summary>
        /// Empty
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        /// <param name="entity"></param>
        /// <param name="originDelegate"></param>
        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
        } // end of EntityLeaveControlUnit

        #endregion EntityLeaveControlUnit
    } // end of ControlUnitManagement
}
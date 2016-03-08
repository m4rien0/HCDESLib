using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.ControlUnits
{
    /// <summary>
    /// Base control unit for organizational areas within health care departments
    /// </summary>
    public abstract class ControlUnitOrganizationalUnit : ControlUnitHealthCare
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control, either a department control or another organizational control</param>
        /// <param name="parentDepartmentControl">Parent department control</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Parent department input</param>
        public ControlUnitOrganizationalUnit(string name,
                           ControlUnit parentControlUnit,
                           ControlUnitHealthCareDepartment parentDepartmentControl,
                           SimulationModel parentSimulationModel,
                           IInputHealthCareDepartment inputData) 
            : base(ControlUnitType.OrganizationalUnit, name, parentControlUnit, parentSimulationModel)
        {
            _parentDepartmentControl = parentDepartmentControl;
            _childOrganizationalUnits = new ControlUnitOrganizationalUnit[] { };
            _assingedTreatmentFacilities = new List<EntityTreatmentFacility>();

            _inputData = inputData;
        } // end of 

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputHealthCareDepartment _inputData;

        /// <summary>
        /// Parent department control
        /// </summary>
        public IInputHealthCareDepartment InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion

        #region ParentDepartmentControl

        private ControlUnitHealthCareDepartment _parentDepartmentControl;

        /// <summary>
        /// Parent department control unit
        /// </summary>
        public ControlUnitHealthCareDepartment ParentDepartmentControl
        {
            get
            {
                return _parentDepartmentControl;
            }
            set
            {
                _parentDepartmentControl = value;
            }
        } // end of ParentDepartmentControl

        #endregion

        #region ChildOrganizationalUnits

        private ControlUnitOrganizationalUnit[] _childOrganizationalUnits;

        /// <summary>
        /// All child organizational controls (if there is a tree of organizational control below a department)
        /// </summary>
        public ControlUnitOrganizationalUnit[] ChildOrganizationalUnits
        {
            get
            {
                return _childOrganizationalUnits;
            }
            set
            {
                _childOrganizationalUnits = value;
            }
        } // end of ChildOrganizationalUnits

        #endregion

        #region AssignedTreatmentFacilities

        private List<EntityTreatmentFacility> _assingedTreatmentFacilities;

        /// <summary>
        /// Treatment facilities that have been assigned (temporarily or permanently) assigned to the
        /// control unit
        /// </summary>
        public IReadOnlyList<EntityTreatmentFacility> AssignedTreatmentFacilities
        {
            get
            {
                return _assingedTreatmentFacilities;
            }
        } // end of AssignedTreatmentFacilities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Handled Admission types are set to an empty array, since this handling is done at the department
        // level
        //--------------------------------------------------------------------------------------------------

        #region HandledOutpatientAdmissionTypes

        /// <summary>
        /// Overrided the handled oupatient admissions, as admissions are handled by
        /// department controls an empty array is returned
        /// </summary>
        public override OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes
        {
            get
            {
                return Helpers<OutpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledOutpatientAdmissionTypes

        #endregion

        #region HandledInpatientAdmissionTypes

        /// <summary>
        /// Overrided the handled inatient admissions, as admissions are handled by
        /// department controls an empty array is returned
        /// </summary>
        public override InpatientAdmissionTypes[] HandledInpatientAdmissionTypes
        {
            get
            {
                return Helpers<InpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledInpatientAdmissionTypes

        #endregion

        #region HandledSpecialFacilityAdmissionTypes

        /// <summary>
        /// Overrided the handled special facility admissions, as admissions are handled by
        /// department controls an empty array is returned
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                return Helpers<SpecialServiceAdmissionTypes>.EmptyArray();
            }
        } // end of HandledDiagnosticsTreatments

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region BehaviorOccured

        public override bool BehaviorOccured
        {
            get
            {
                return base.BehaviorOccured;
            }
            set
            {
                base.BehaviorOccured = value;
                ParentControlUnit.BehaviorOccured = true;
            }
        } // end of BehaviorOccured

        #endregion

        #region SetOrganizationalChildControl

        /// <summary>
        /// Sets all child organizational controls
        /// </summary>
        /// <param name="childOrgControls"></param>
        public void SetOrganizationalChildControl(ControlUnitOrganizationalUnit[] childOrgControls)
        {
            if (childOrgControls == null)
                return;

            _childOrganizationalUnits = childOrgControls;
        } // end of SetOrganizationalChildControl

        #endregion

        #region AddRequest

        /// <summary>
        /// Overrides the add request method for control units as all requests are
        /// routed first to the parent department control and not directly to the 
        /// organizational unit
        /// </summary>
        /// <param name="req"></param>
        public override void AddRequest(ActivityRequest req)
        {
            ParentDepartmentControl.AddRequest(req);
        } // end of AddRequest

        #endregion

        #region AssignRequest

        /// <summary>
        /// This method should be used assign requests to organizational units by the parent department or organizational control
        /// </summary>
        /// <param name="request">The request to assign</param>
        public void AssignRequest(ActivityRequest request)
        {
            _rael.Add(request);
            BehaviorOccured = true;
        } // end of AssignRequest

        #endregion

        #region AddAssignedTreatmentFacility

        /// <summary>
        /// Temporarily or permanently assign facilities to organizational units
        /// </summary>
        /// <param name="treatFac">The treatment facility to assign</param>
        public void AddAssignedTreatmentFacility(EntityTreatmentFacility treatFac)
        {
            _assingedTreatmentFacilities.Add(treatFac);
            treatFac.CurrentlyAssignedOrganizationalUnit = this;
        } // end of AddAssignedTreatmentFacility

        #endregion

        #region RemoveAssignedTreatmentFacility

        /// <summary>
        /// Remove an assigned treatment facility from a organizational unit
        /// </summary>
        /// <param name="treatFac">The treamten facility to remove</param>
        public void RemoveAssignedTreatmentFacility(EntityTreatmentFacility treatFac)
        {
            _assingedTreatmentFacilities.Remove(treatFac);
            treatFac.CurrentlyAssignedOrganizationalUnit = null;
        } // end of RemoveAssignedTreatmentFacility

        #endregion

        #region EntityEnterLeaveControlUnit

        /// <summary>
        /// Not implemented per default
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

        /// <summary>
        /// Not implemented per default
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        /// <param name="entity"></param>
        /// <param name="originDelegate"></param>
        /// <returns></returns>
        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityLeaveControlUnit

        #endregion

        #region GetCurrentActivitiesPlusChildActivities

        /// <summary>
        /// Returns all activities currently hosted by department and all child 
        /// organizational controls
        /// </summary>
        /// <param name="listToAdd"></param>
        /// <returns>A list of all current activities in the department control and all sub trees
        /// of organizational control units</returns>
        public List<Activity> AddCurrentActivitiesPlusChildActivities(List<Activity> listToAdd)
        {
            listToAdd.AddRange(CurrentActivities);

            foreach (ControlUnitOrganizationalUnit orgUnit in ChildOrganizationalUnits)
            {
                AddCurrentActivitiesPlusChildActivities(listToAdd);
            } //end foreach

            return listToAdd;
        } // end of GetCurrentActivitiesPlusChildActivities

        #endregion

    } // end of ControlUnitOrganizationalUnit
}

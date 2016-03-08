using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.ControlUnits
{

    /// <summary>
    /// Base class for health care departments
    /// </summary>
    public abstract class ControlUnitHealthCareDepartment : ControlUnitHealthCare
    {
        #region Constructor

        /// <summary>
        /// Bsaic constructor
        /// </summary>
        /// <param name="type">Type of health care control unit</param>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control unit if this is not the root control unit of the control tree</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Input data for the department</param>
        public ControlUnitHealthCareDepartment(ControlUnitType type,
                           string name,
                           ControlUnit parentControlUnit,
                           SimulationModel parentSimulationModel,
                           IInputHealthCareDepartment inputData)
            : base(type,
                  name,
                  parentControlUnit,
                  parentSimulationModel)
        {
            _childOrganizationalUnits = new ControlUnitOrganizationalUnit[] { };
            _organizationalUnitsPerName = new Dictionary<string, ControlUnitOrganizationalUnit>();

            _inputData = inputData;

        } // end of ControlUnitHealthCare

        #endregion

        #region Initialize

        /// <summary>
        /// Overrides the base initialize method of control units. All staff members, treatment facilities
        /// and waiting areas are intialized and assigned to sub organizational control units and structural areas
        /// </summary>
        /// <param name="startTime">Time the simulation starts</param>
        /// <param name="simEngine">SimEngine that handles the simulation run</param>
        public override void Initialize(DateTime startTime, ISimulationEngine simEngine)
        {
            #region TreatmentFacilityInitialize

            string[] structuralAreaIdentifier = InputData.GetStructuralAreaIdentifiers();

            Dictionary<string, StructuralArea> strucArePerName = new Dictionary<string, StructuralArea>();

            // create all structural areas that occur in waiting areas and treamtent facilities
            foreach (string identifier in structuralAreaIdentifier)
            {
                strucArePerName.Add(identifier, new StructuralArea(identifier));
            } // end foreach

            _assingedTreatmentFacilities = new List<EntityTreatmentFacility>();

            ResourceAssignmentPhysical<EntityTreatmentFacility>[] treatmentFacilityAssigns = InputData.GetTreatmentFacilities();

            // assign treatment facilities to organizational and structural areas
            foreach (ResourceAssignmentPhysical<EntityTreatmentFacility> assignment in treatmentFacilityAssigns)
            {
                assignment.Resource.SetParentDepartmentControl(this);

                assignment.Resource.AssignmentType = assignment.AssignmentType;

                if (assignment.OrganizationalUnit != "RootDepartment")
                {
                    // the facility is assigned to the specified organizational control
                    OrganizationalUnitPerName[assignment.OrganizationalUnit].AddEntity(assignment.Resource);
                    OrganizationalUnitPerName[assignment.OrganizationalUnit].AddAssignedTreatmentFacility(assignment.Resource);
                }
                else
                {
                    // if no organizational area was specified or the root department was specified
                    // the facility is added in the department control
                    AddEntity(assignment.Resource);
                    AssignedTreatmentFacilities.Add(assignment.Resource);
                } // end if

                // faciltiy is added to the specified structural area
                if (!strucArePerName.ContainsKey(assignment.StructuralArea))
                    throw new InvalidOperationException("Structural area identifier not specified");

                if (assignment.Resource is EntityMultiplePatientTreatmentFacility)
                    strucArePerName[assignment.StructuralArea].MultiplePatientTreatmentFacilities.Add((EntityMultiplePatientTreatmentFacility)assignment.Resource);
                else
                    strucArePerName[assignment.StructuralArea].TreatmentFacilities.Add(assignment.Resource);

            } // end foreach

            #endregion

            #region WaitingAreas

            // same principle as for treatment facility is used for waiting areas

            foreach (ResourceAssignmentPhysical<EntityWaitingArea> waitingRoomAssignment in InputData.GetWaitingRoomPatients())
            {

                if (waitingRoomAssignment.OrganizationalUnit != "RootDepartment")
                {
                    OrganizationalUnitPerName[waitingRoomAssignment.OrganizationalUnit].AddEntity(waitingRoomAssignment.Resource);
                }
                else
                {
                    AddEntity(waitingRoomAssignment.Resource);
                } // end if

                if (!strucArePerName.ContainsKey(waitingRoomAssignment.StructuralArea))
                    throw new InvalidOperationException("Structural area identifier not specified");

                strucArePerName[waitingRoomAssignment.StructuralArea].WaitingAreasPatients.Add(waitingRoomAssignment.Resource);

            } // end foreach

            foreach (ResourceAssignmentPhysical<EntityWaitingArea> waitingRoomAssignment in InputData.GetWaitingRoomsStaff())
            {

                if (waitingRoomAssignment.OrganizationalUnit != "RootDepartment")
                {
                    OrganizationalUnitPerName[waitingRoomAssignment.OrganizationalUnit].AddEntity(waitingRoomAssignment.Resource);
                }
                else
                {
                    AddEntity(waitingRoomAssignment.Resource);
                } // end if

                if (!strucArePerName.ContainsKey(waitingRoomAssignment.StructuralArea))
                    throw new InvalidOperationException("Structural area identifier not specified");

                strucArePerName[waitingRoomAssignment.StructuralArea].StaffWaitingRoom = waitingRoomAssignment.Resource;

            } // end foreach

            #endregion

            _structuralAres = strucArePerName.Values.ToArray();

            #region StaffInitialze

            // the starting staff assignements specified by the input is obtained
            List<ResourceAssignmentStaff> staffAssignments = InputData.StaffHandler.GetStartingStaff(startTime);

            foreach (ResourceAssignmentStaff staffAssignment in staffAssignments)
            {
                staffAssignment.Resource.StaffOutsideShift = false;
                staffAssignment.Resource.BlockedForDispatching = false;
                staffAssignment.Resource.BaseControlUnit = this;
                staffAssignment.Resource.AssignmentType = staffAssignment.AssignmentType;

                // staff is either assigned to a organizational control or the department control
                if (staffAssignment.OrganizationalUnit == "RootDepartment")
                    AddEntity(staffAssignment.Resource);
                else
                    OrganizationalUnitPerName[staffAssignment.OrganizationalUnit].AddEntity(staffAssignment.Resource);
            } // end foreach

            DateTime nextStaffChange;

            // the next time the staffing levels change an event is scheduled with the associated staff changes
            Event staffChangeEvent = InputData.StaffHandler.GetNextStaffChangingEvent(this, startTime, out nextStaffChange);

            simEngine.AddScheduledEvent(staffChangeEvent, nextStaffChange);

            // all hanlded staff members (doctors and nurses) are sent in a waiting activity
            foreach (EntityDoctor doc in HandledDoctors)
            {
                Event docWait = doc.StartWaitingActivity(WaitingRoomForStaff(doc));
                docWait.Trigger(startTime, simEngine);
            } // end foreach

            foreach (EntityNurse nurse in HandledNurses)
            {
                Event nurseWait = nurse.StartWaitingActivity(WaitingRoomForStaff(nurse));
                nurseWait.Trigger(startTime, simEngine);
            } // end foreach

            #endregion

            // base initializ is called to move down the tree
            base.Initialize(startTime, simEngine);

        } // end of Initilaize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region ChildOrganizationalUnits

        private ControlUnitOrganizationalUnit[] _childOrganizationalUnits;

        /// <summary>
        /// A list of all child organizational units to avoid casts
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

        #region OrganizationalUnitPerName

        private Dictionary<string,ControlUnitOrganizationalUnit> _organizationalUnitsPerName;

        /// <summary>
        /// Makes organizational control accessible per name, helps to formulate routing policies
        /// </summary>
        public Dictionary<string,ControlUnitOrganizationalUnit> OrganizationalUnitPerName
        {
            get
            {
                return _organizationalUnitsPerName;
            }
            set
            {
                _organizationalUnitsPerName = value;
            }
        } // end of OrganizationalUnitPerName

        #endregion

        #region InputData

        private IInputHealthCareDepartment _inputData;

        /// <summary>
        /// Healcht Care Department Input
        /// </summary>
        public IInputHealthCareDepartment InputData
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

        #region AssignedTreatmentFacilities

        private List<EntityTreatmentFacility> _assingedTreatmentFacilities;

        /// <summary>
        /// Treatment facilities that have been assigned (temporarily or permanently) assigned to the
        /// control unit
        /// </summary>
        public List<EntityTreatmentFacility> AssignedTreatmentFacilities
        {
            get
            {
                return _assingedTreatmentFacilities;
            }
            set
            {
                _assingedTreatmentFacilities = value;
            }
        } // end of AssignedTreatmentFacilities

        #endregion

        #region StructuralAreas

        private StructuralArea[] _structuralAres;

        /// <summary>
        /// All structural areas of the department
        /// </summary>
        public StructuralArea[] StructuralAreas
        {
            get
            {
                return _structuralAres;
            }
        } // end of StructuralAreas

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region AddChildOrganizationalControls
        
        /// <summary>
        /// Adds a child organiaztional control unit and the entire sub tree represented by it to the department
        /// </summary>
        /// <param name="childOrgControl">The child organizational control</param>
        private void AddChildOrganizationalControls(ControlUnitOrganizationalUnit childOrgControl)
        {
            _organizationalUnitsPerName.Add(childOrgControl.Name, childOrgControl);

            foreach (ControlUnitOrganizationalUnit childOrg in childOrgControl.ChildOrganizationalUnits)
            {
                AddChildOrganizationalControls(childOrg);
            } // end foreach

        } // end of AddChildOrganizationalControls

        #endregion

        #region SetChildOrganizationalControls

        /// <summary>
        /// Via this method child organizational controls can be added to the department control, the sub tree
        /// of the passed child organizational controls are automatically added as well.
        /// </summary>
        /// <param name="childControlUnits">Child organizational control units</param>
        public void SetChildOrganizationalControls(ControlUnitOrganizationalUnit[] childControlUnits)
        {
            if (childControlUnits == null)
                return;

            _childOrganizationalUnits = childControlUnits;
            SetChildControlUnits(childControlUnits);

            foreach (ControlUnitOrganizationalUnit childOrg in childControlUnits)
            {
                AddChildOrganizationalControls(childOrg);
            } // end foreach
        } // end of SetChildOrganizationalControls

        #endregion

        #region GetCurrentActivitiesPlusOrganizationalUnit

        /// <summary>
        /// Returns all activities currently hosted by department and all child 
        /// organizational controls
        /// </summary>
        /// <returns>A list of all current activities in the department control and all sub trees
        /// of organizational control units</returns>
        public List<Activity> GetCurrentActivitiesPlusOrganizationalUnit()
        {
            List<Activity> currentActivities = new List<Activity>(CurrentActivities);

            foreach (ControlUnitOrganizationalUnit orgUnit in ChildOrganizationalUnits)
            {
                orgUnit.AddCurrentActivitiesPlusChildActivities(currentActivities);
            } //end foreach

            return currentActivities;

        } // end of GetCurrentActivitiesPlusOrganizationalUnit

        #endregion

        #region SkipNextAction

        /// <summary>
        /// Defines conditional routine, e.g. if resources are qualified enough the next consultation does
        /// not have to take place. Per default no action should be skipped, if wanted the method has
        /// to be overriden by the user
        /// </summary>
        /// <param name="patient">Considered patient</param>
        /// <param name="mainDoc">Main doctor of the intial action</param>
        /// <param name="initialTreatmentType">The initial treatment that defines a possible skip</param>
        /// <param name="nextActionType">The action to be skipped</param>
        /// <returns>True if action should be skipped</returns>
        virtual public bool SkipNextAction(EntityPatient patient, EntityDoctor mainDoc, ActionTypeClass initialTreatmentType, ActionTypeClass nextActionType)
        {
            return false;
        } // end of SkipNextAction

        #endregion

        #region WaitingAreaPatientForNextActionType

        /// <summary>
        /// Defines waiting areas for patient actions
        /// </summary>
        /// <param name="nextActionType">Action type patient is waiting for</param>
        /// <returns></returns>
        public virtual EntityWaitingArea WaitingAreaPatientForNextActionType(ActionTypeClass nextActionType)
        {
            return StructuralAreas.First().WaitingAreasPatients.First();
        } // end of WaitingAreaPatientForNextActionType

        #endregion

        #region WaitingAreaStaffAfterActionType

        /// <summary>
        /// Defines waiting areas for staff members
        /// </summary>
        /// <param name="staff">Staff member that is about to start a waiting activity</param>
        /// <returns></returns>
        public virtual EntityWaitingArea WaitingRoomForStaff(EntityHealthCareStaff staff)
        {
            return StructuralAreas.First().StaffWaitingRoom;
        } // end of WaitingAreaStaffAfterActionType

        #endregion

    } // end of ControlUnitHealthCareDepartment
}

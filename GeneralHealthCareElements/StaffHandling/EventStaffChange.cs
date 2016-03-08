using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.StaffHandling
{
    /// <summary>
    /// Event that triggers a change in the available staffing levels for a control unit
    /// </summary>
    public class EventStaffChange : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControl">Control unit where the staff change will occur</param>
        /// <param name="staffLeaving">Staff members that leave upon change</param>
        /// <param name="staffArriving">Staff members that arrive upon change</param>
        /// <param name="staffHandler">Staff handler that created event</param>
        public EventStaffChange(ControlUnitHealthCareDepartment parentControl, 
            ResourceAssignmentStaff[] staffLeaving,
            ResourceAssignmentStaff[] staffArriving,
            IStaffHandling staffHandler) : base(EventType.Standalone, parentControl)
        {
            _parentDepartmentControl = parentControl;

            _staffAriving = staffArriving;
            _staffLeaving = staffLeaving;
            _allStaff = new List<ResourceAssignmentStaff>(staffLeaving);
            _allStaff.AddRange(staffArriving);
            _staffHandler = staffHandler;
        } // end of EventStaffChange

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// Overrides the state change method, staffing levels will change, for leaving 
        /// staff requests for absence are filed, incoming staff is added to the
        /// parent control unit
        /// </summary>
        /// <param name="time">Time the staffing changes</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // foreach leaving staff a request to be absent is filed at their control unit
            // arriving staff is automatically added to the control by the triggering 
            // of the event
            foreach (ResourceAssignmentStaff staffAssignment in StaffLeaving)
            {
                ParentDepartmentControl.AddRequest(new RequestBeAbsent(staffAssignment.Resource, time));
                staffAssignment.Resource.StaffOutsideShift = true;
            } // end foreach

            foreach (ResourceAssignmentStaff staffAssignment in StaffAriving)
            {
                staffAssignment.Resource.StaffOutsideShift = false;
                staffAssignment.Resource.BlockedForDispatching = false;
                staffAssignment.Resource.BaseControlUnit = ParentDepartmentControl;
                staffAssignment.Resource.AssignmentType = staffAssignment.AssignmentType;

                if (staffAssignment.OrganizationalUnit == "RootDepartment")
                    ParentDepartmentControl.AddEntity(staffAssignment.Resource);
                else
                    ParentDepartmentControl.OrganizationalUnitPerName[staffAssignment.OrganizationalUnit].AddEntity(staffAssignment.Resource);

                SequentialEvents.Add(staffAssignment.Resource.StartWaitingActivity(ParentDepartmentControl.WaitingRoomForStaff(staffAssignment.Resource)));
            } // end foreach

            DateTime nextTime;

            // schedule the next staff change
            Event nextChange = StaffHandler.GetNextStaffChangingEvent(ParentDepartmentControl, time, out nextTime);

            simEngine.AddScheduledEvent(nextChange, nextTime);

        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Parameter 
        //--------------------------------------------------------------------------------------------------

        #region ParentDepartmentControl

        private ControlUnitHealthCareDepartment _parentDepartmentControl;

        /// <summary>
        /// Parent department control where staffing is done
        /// </summary>
        public ControlUnitHealthCareDepartment ParentDepartmentControl
        {
            get
            {
                return _parentDepartmentControl;
            }
        } // end of ParentDepartmentControl

        #endregion

        #region StaffHandler

        private IStaffHandling _staffHandler;

        /// <summary>
        /// Staff handler that created the event
        /// </summary>
        public IStaffHandling StaffHandler
        {
            get
            {
                return _staffHandler;
            }
        } // end of StaffHandler

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region StaffLeaving

        private ResourceAssignmentStaff[] _staffLeaving;

        /// <summary>
        /// Reseource assignment of staff that is leaving at change
        /// </summary>
        public ResourceAssignmentStaff[] StaffLeaving
        {
            get
            {
                return _staffLeaving;
            }
        } // end of StaffLeaving

        #endregion

        #region StaffAriving

        private ResourceAssignmentStaff[] _staffAriving;

        /// <summary>
        /// Staff members that arrive upon change
        /// </summary>
        public ResourceAssignmentStaff[] StaffAriving
        {
            get
            {
                return _staffAriving;
            }
        } // end of StaffAriving

        #endregion

        #region AllStaff

        /// <summary>
        /// Combines arriving and leaving staff
        /// </summary>
        private List<ResourceAssignmentStaff> _allStaff;

        #endregion

        #region AffectedEntites

        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] {};
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventStaffChange";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventStaffChange(ParentDepartmentControl, StaffLeaving, StaffAriving, StaffHandler);
        } // end of Clone

        #endregion
      
    } // end EventStaffChange
}

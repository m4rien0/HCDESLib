using GeneralHealthCareElements.Delegates;
using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.Events
{
    /// <summary>
    /// Default event for entering and leaving of staff when changing department control units
    /// Blocks a staff members if it enters for assisting
    /// </summary>
    public class EventControlUnitStaffEnterLeave : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Control unit the event is triggered, i.e. the unit the staff member enters or leaves</param>
        /// <param name="isEnter">Flag if the event is called for an entering or leaving staff member</param>
        /// <param name="staff">The staff member that is entering or leaving</param>
        /// <param name="del">Delegate resposnible for the staff movement</param>
        /// <param name="staffArea">Possible waiting area the staff members waits in after arrival</param>
        public EventControlUnitStaffEnterLeave(ControlUnit parentControlUnit,
            bool isEnter, EntityStaff staff, IDelegate del, EntityWaitingArea staffArea = null)
            : base(EventType.Standalone, parentControlUnit)
        {
            _isEnter = isEnter;
            _staff = staff;
            _incomingDelegate = del;
            _staffWaitingArea = staffArea;
        } // end of EventInpatientStaffEnterLeave

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// Overrides the state change method, incoming staff is blocked for dispatching if the origin delegate is DelegateSentDocForAssistedTreatment
        /// </summary>
        /// <param name="time">Time the staff member enters or leaves</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            if (!IsEnter)
            {
                Staff.BlockedForDispatching = false;
            } // end if

            if (IsEnter)
            {
                SequentialEvents.Add(Staff.StartWaitingActivity(StaffWaitingArea));

                if (IncomingDelegate != null && IncomingDelegate is DelegateSentDocForAssistedTreatment)
                    Staff.BlockedForDispatching = true;
            } // end if
        } // end of StateChange

        #endregion Trigger

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Staff

        private EntityStaff _staff;

        /// <summary>
        /// The staff member that is entering or leaving
        /// </summary>
        public EntityStaff Staff
        {
            get
            {
                return _staff;
            }
        } // end of Staff

        #endregion Staff

        #region AffectedEntities

        public override Entity[] AffectedEntities
        {
            get { return new Entity[] { Staff }; }
        } // end of AffectedEntities

        #endregion AffectedEntities

        //--------------------------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------------------------

        #region IsEnter

        private bool _isEnter;

        /// <summary>
        /// Flag if the event is called for an entering or leaving staff member
        /// </summary>
        public bool IsEnter
        {
            get
            {
                return _isEnter;
            }
        } // end of IsEnter

        #endregion IsEnter

        #region IncomingDelegate

        private IDelegate _incomingDelegate;

        /// <summary>
        /// Delegate resposnible for the staff movement
        /// </summary>
        public IDelegate IncomingDelegate
        {
            get
            {
                return _incomingDelegate;
            }
        } // end of DelegateOfMove

        #endregion IncomingDelegate

        #region StaffWaitingArea

        private EntityWaitingArea _staffWaitingArea;

        /// <summary>
        /// Possible waiting area the staff members waits in after arrival
        /// </summary>
        public EntityWaitingArea StaffWaitingArea
        {
            get
            {
                return _staffWaitingArea;
            }
        } // end of StaffWaitingArea

        #endregion StaffWaitingArea

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            if (IsEnter)
                return "Event" + ParentControlUnit.Name + "StaffEnter";
            else
                return "Event" + ParentControlUnit.Name + "StaffLeave";
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Event Clone()
        {
            throw new NotImplementedException();
        } // end of Clone

        #endregion Clone
    } // end of EventInpatientStaffEnterLeave
}
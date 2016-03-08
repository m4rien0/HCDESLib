using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
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
    /// Event for a actual staff leave, after absence request has been handled
    /// </summary>
    public class EventStaffLeave : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControl">Control where staff is leaving to be absent</param>
        /// <param name="staff">Staff that is going in an absent state</param>
        public EventStaffLeave(ControlUnit parentControl,
            EntityHealthCareStaff staff)
            : base(EventType.Standalone, parentControl)
        {
            _staffLeaving = staff;
        } // end of EventStaffChange

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// STaff is removed from control unit
        /// </summary>
        /// <param name="time">Time staff is leaving</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            ParentControlUnit.RemoveEntity(StaffLeaving);
        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region StaffLeaving

        private EntityHealthCareStaff _staffLeaving;

        /// <summary>
        /// Staff that is going in an absent state
        /// </summary>
        public EntityHealthCareStaff StaffLeaving
        {
            get
            {
                return _staffLeaving;
            }
        } // end of StaffLeaving

        #endregion

        #region AffectedEntites

        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] {StaffLeaving};
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventStaffLeave";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventStaffLeave(ParentControlUnit, StaffLeaving);
        } // end of Clone

        #endregion

    } // end of EventStaffLeave
}

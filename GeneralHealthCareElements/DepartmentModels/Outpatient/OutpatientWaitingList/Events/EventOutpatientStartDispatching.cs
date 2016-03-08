using GeneralHealthCareElements.BookingModels;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList
{
    /// <summary>
    /// Event that represents stream when slot assignments are performed (if not immediately)
    /// </summary>
    public class EventOutpatientStartDispatching : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent waiting list control</param>
        /// <param name="waitingListSchedule">Waiting list schedule for slot booking</param>
        /// <param name="inputData">Outpatient input data</param>
        public EventOutpatientStartDispatching(ControlUnit parentControlUnit, 
            EntityWaitingListSchedule waitingListSchedule,
            IInputOutpatient inputData)
            : base(EventType.Standalone, parentControlUnit)
        {
            _waitingListSchedule = waitingListSchedule;
            _inputData = inputData;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// State change of event. Sets Dispatching flag to true and schedules next event.
        /// </summary>
        /// <param name="time">Current time</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            WaitingListSchedule.ReadyForDispatch = true;

            EventOutpatientStartDispatching nextDispatch = 
                new EventOutpatientStartDispatching(ParentControlUnit, WaitingListSchedule, InputData);

            simEngine.AddScheduledEvent(nextDispatch, InputData.NextDispatching(time));
        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
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
            set
            {
                _inputData = value;
            }
        } // end of InputData

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region WaitingListSchedule

        private EntityWaitingListSchedule _waitingListSchedule;

        /// <summary>
        /// Waiting list schedule used
        /// </summary>
        public EntityWaitingListSchedule WaitingListSchedule
        {
            get
            {
                return _waitingListSchedule;
            }
        } // end of WaitingListSchedule

        #endregion
        
        #region AffectedEntites

        /// <summary>
        /// Waiting list schedule is only affected entity
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { WaitingListSchedule };
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventOutpatientStartDispatching";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventOutpatientStartDispatching(ParentControlUnit, (EntityWaitingListSchedule)WaitingListSchedule.Clone(), InputData);
        } // end of Clone

        #endregion
    }
}

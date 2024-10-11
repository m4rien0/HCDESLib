using SimulationCore.SimulationClasses;
using System;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// The base abstract class for all activities. Provides members for start and end events, and methods
    /// to override fór the actual state changes upon start and end of the activity. Note that interruption
    /// is handled as a premature end event. Also stores and automatically sets start and end times of activity
    /// and the corresponding duration.
    /// </summary>
    public abstract class Activity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit"> Control unit the activity is nested in, has to be set upon creation of object and not triggering of start event</param>
        /// <param name="activityType">String representing the activity type, e.g move</param>
        /// <param name="preEmptable">Indicitates of activity may be interuppted (pre-empted)</param>
        public Activity(ControlUnit parentControlUnit, string activityType, bool preEmptable)
        {
            _parentControlUnit = parentControlUnit;
            _preEmtable = preEmptable;

            // events for start and end are created
            _startEvent = new EventActivity(this, EventType.Start);
            _endEvent = new EventActivity(this, EventType.End);

            _activityType = activityType;
        } // end of Event

        #endregion Constructor

        #region ActivityName

        private string _activityType;

        public string ActivityName
        {
            get
            {
                return _activityType;
            }
        } // end of ActivityType

        #endregion ActivityName

        #region StartTime

        private DateTime _startTime;

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                _startTime = value;
            }
        } // end of StartTime

        #endregion StartTime

        #region EndTime

        private DateTime _endTime;

        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
            }
        } // end of EndTime

        #endregion EndTime

        //--------------------------------------------------------------------------------------------------
        // Start and end event definitions
        //--------------------------------------------------------------------------------------------------

        #region StartEvent

        private EventActivity _startEvent;

        public EventActivity StartEvent
        {
            get
            {
                return _startEvent;
            }
            set
            {
                _startEvent = value;
            }
        } // end of StartEvent

        #endregion StartEvent

        #region EndEvent

        private EventActivity _endEvent;

        public EventActivity EndEvent
        {
            get
            {
                return _endEvent;
            }
            set
            {
                _endEvent = value;
            }
        } // end of EndEvent

        #endregion EndEvent

        #region StateChangeStartEvent

        /// <summary>
        /// User can override this method to implement custom state changes upon start of activity
        /// </summary>
        /// <param name="time"> time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public abstract void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine);

        #endregion StateChangeStartEvent

        #region StateChangeEndEvent

        /// <summary>
        /// User can override this method to implement custom state changes upon end of activity
        /// </summary>
        /// <param name="time"> time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        public abstract void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine);

        #endregion StateChangeEndEvent

        //--------------------------------------------------------------------------------------------------
        // Default Parameter
        //--------------------------------------------------------------------------------------------------

        #region PreEmptable

        private bool _preEmtable;

        /// <summary>
        /// Parameter that indicates if the activity is preemptable
        /// </summary>
        /// <returns> true if activity is pre-emptable</returns>
        public virtual bool PreEmptable()
        {
            return _preEmtable;
        } // end of PreEmptable

        #endregion PreEmptable

        //--------------------------------------------------------------------------------------------------
        // Member definitions
        //--------------------------------------------------------------------------------------------------

        #region ParentControlUnit

        private ControlUnit _parentControlUnit;

        public ControlUnit ParentControlUnit
        {
            get
            {
                return _parentControlUnit;
            }
        } // end of ParentControlUnit

        #endregion ParentControlUnit

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        /// <summary>
        /// To string of activities should be overwritten by user, e.g. can icnlude descriptions of
        /// affected entities
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        #endregion ToString

        #region AffectedEntities

        /// <summary>
        /// Should return all entities that are effected by the activity, this
        /// list is used to add/remoove entities to control units when start
        /// and end events are triggered
        /// </summary>
        public abstract Entity[] AffectedEntities { get; }

        #endregion AffectedEntities

        #region Clone

        public abstract Activity Clone();

        #endregion Clone
    } // end of Activity
}
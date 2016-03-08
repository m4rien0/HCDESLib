using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Activities
{
    /// <summary>
    /// Activity for moves between departments represented by control units
    /// </summary>
    public class ActivityMove : Activity
    {
        #region ActivityMove

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">The control unit that hosts the move</param>
        /// <param name="affectedEntity">The moving entity</param>
        /// <param name="origin">Origin control unit</param>
        /// <param name="destination">Destination control unit</param>
        /// <param name="del">A delegate that is associated with move, e.g. a resource or service request</param>
        /// <param name="duration">Duration of move</param>
        public ActivityMove(ControlUnit parentControlUnit,
            Entity affectedEntity,
            ControlUnit origin,
            ControlUnit destination,
            IDelegate del,
            TimeSpan duration)
            : base(parentControlUnit, "ActivityMove", true)
        {
            _movingEntity = affectedEntity;
            _orgigin = origin;
            _destination = destination;
            _delegateOrigin = del;
            _duration = duration;
        } // end of Activity

        #endregion

        #region Name

        public static string Name = "ActivityMove";

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region MovingEntity

        private Entity _movingEntity;

        /// <summary>
        /// The entity that is on the move
        /// </summary>
        public Entity MovingEntity
        {
            get
            {
                return _movingEntity;
            }
        } // end of MovingEntity

        #endregion

        #region Origin

        private ControlUnit _orgigin;

        /// <summary>
        /// Origin control unit
        /// </summary>
        public ControlUnit Origin
        {
            get
            {
                return _orgigin;
            }
        } // end of Origin

        #endregion

        #region Destination

        private ControlUnit _destination;

        /// <summary>
        /// Destination control unit
        /// </summary>
        public ControlUnit Destination
        {
            get
            {
                return _destination;
            }
        } // end of Destination

        #endregion
        
        #region AffectedEntites

        /// <summary>
        /// Representing the moving entity
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { MovingEntity};
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// Actual state change, calls the leaving method of the origin control unit and scheduled the end event
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            Origin.EntityLeaveControlUnit(time, simEngine, MovingEntity, DelegateOrigin);
            _endTime = time + Duration;
            simEngine.AddScheduledEvent(EndEvent, time + Duration);
        } // end of TriggerStartEvent

        #endregion
        
        #region TriggerEndEvent

        /// <summary>
        /// State change of the end event. Adds the enter event of the destination control unit
        /// or in case of pre-emption the scheduled end event is removed from the sim engine
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            if (time < EndTime)
            {
                simEngine.RemoveScheduledEvent(EndEvent);
                return;
            } // end if 

            Event enterEvent = Destination.EntityEnterControlUnit(time, simEngine, MovingEntity, DelegateOrigin);
            
            if (enterEvent != null)
                EndEvent.SequentialEvents.Add(enterEvent);
        } // end of TriggerEndEvent

        #endregion

        #region ToString

        public override string ToString()
        {
            return Name + ": " + Origin.ToString() + "-" + Destination.ToString();
        } // end of ToString

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Clone

        public override Activity Clone()
        {
            return new ActivityMove(ParentControlUnit, MovingEntity, Origin, Destination, DelegateOrigin, Duration);
        } // end of Clone

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Parameters
        //--------------------------------------------------------------------------------------------------

        #region DelegateOrigin

        private IDelegate _delegateOrigin;

        /// <summary>
        /// The delegate associated with the move, may be null
        /// </summary>
        public IDelegate DelegateOrigin
        {
            get
            {
                return _delegateOrigin;
            }
        } // end of DelegateOrigin

        #endregion

        #region Duration

        private TimeSpan _duration;

        /// <summary>
        /// The anticipated duration of the move
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
        } // end of Duration

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region EndTime

        private DateTime _endTime;

        /// <summary>
        /// Scheduled end time of move
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        } // end of EndTime

        #endregion
        
    } // end of ActivityMove
}

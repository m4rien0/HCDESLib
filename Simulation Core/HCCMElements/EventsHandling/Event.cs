using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Describes the type of event, either a standalone event or start and end events of activities.
    /// </summary>
    public enum EventType
    {
        Start,
        End,
        Standalone
    } // end of EventTypes

    /// <summary>
    /// Base class for all events
    /// </summary>
    public abstract class Event
    {
        #region Constructor

        /// <summary>
        /// Constructor where type and hosting control unit is defined.
        /// </summary>
        /// <param name="type">Type of activity</param>
        /// <param name="parentControlUnit">Control unit where event will be triggered</param>
        public Event(EventType type, ControlUnit parentControlUnit)
        {
            _eventType = type;
            _parentControlUnit = parentControlUnit;
            _sequentialEvents = new List<Event>();
        } // end of Event

        #endregion Constructor

        #region EventType

        private EventType _eventType;

        /// <summary>
        /// Property to reflect type of event
        /// </summary>
        public EventType EventType
        {
            get
            {
                return _eventType;
            }
        } // end of EventType

        #endregion EventType



        #region Trigger

        /// <summary>
        /// Trigger method of the event.
        /// </summary>
        /// <param name="time">Time when event is triggered</param>
        /// <param name="simEngine">SimEngine that executes the model</param>
        public void Trigger(DateTime time, ISimulationEngine simEngine)
        {
            foreach (Entity entity in AffectedEntities)
            {
                // if the parent control unit of the entity is already set it is checked if the
                // parent control unit differs from the control the event is hosted.
                // If it is different it is changed to the parent control of the event and the entity is
                // added to this control unit
                if (entity.ParentControlUnit != null)
                {
                    if (entity.ParentControlUnit != ParentControlUnit)
                    {
                        entity.ParentControlUnit.RemoveEntity(entity);
                        ParentControlUnit.AddEntity(entity);
                    }
                }
                // if the entities parent control is not set it is added to the parent control of the event
                else
                    ParentControlUnit.AddEntity(entity);
            } // endforeach

            // custom state change of the event, cann be overwritten by the user
            StateChange(time, simEngine);

            // the event is logged at the used simulation engine
            simEngine.LogEvent(this);

            // sequential events are triggered, they might be defined by custom state change method.
            foreach (Event seqEvent in SequentialEvents)
            {
                seqEvent.Trigger(time, simEngine);
            } // end foreach

            // The behavior occured flag of the parent control is set to true
            ParentControlUnit.BehaviorOccured = true;

            // sequential eventy are cleared
            SequentialEvents.Clear();
        } // end of Trigger

        #endregion Trigger

        #region SequentialEvents

        private List<Event> _sequentialEvents;

        /// <summary>
        /// Sequential events, will be automatically triggered at trigger method of event
        /// </summary>
        public List<Event> SequentialEvents
        {
            get
            {
                return _sequentialEvents;
            }
            set
            {
                _sequentialEvents = value;
            }
        } // end of SequentialEvents

        #endregion SequentialEvents

        #region ParentControlUnit

        private ControlUnit _parentControlUnit;

        /// <summary>
        /// Parent control where event is hosted and triggered
        /// </summary>
        public ControlUnit ParentControlUnit
        {
            get
            {
                return _parentControlUnit;
            }
        } // end of ParentControlUnit

        #endregion ParentControlUnit

        #region StateChange

        /// <summary>
        /// Custom method for state changes of event. This method should be overwritten for the actual
        /// state changes of the event
        /// </summary>
        /// <param name="time">Time when event is triggered</param>
        /// <param name="simEngine">SimEngine for simulation execution</param>
        protected abstract void StateChange(DateTime time, ISimulationEngine simEngine);

        #endregion StateChange

        #region ToString

        /// <summary>
        /// To string method for events
        /// </summary>
        /// <returns></returns>
        public abstract override string ToString();

        #endregion ToString

        #region Clone

        public abstract Event Clone();

        #endregion Clone

        #region AffectedEntities

        public abstract Entity[] AffectedEntities { get; }

        #endregion AffectedEntities

        #region GetDescription

        /// <summary>
        /// Basic string representation of event composed by its ToString method
        /// and the participating entities
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            StringBuilder description = new StringBuilder();

            description.Append(ToString() + "(");

            for (int i = 0; i < AffectedEntities.Length; i++)
            {
                description.Append(AffectedEntities[i].ToString());

                if (i < AffectedEntities.Length - 1)
                    description.Append(", ");
            } // end for

            description.Append(")");

            return description.ToString();
        } // end of GetDescription

        #endregion GetDescription
    }
}
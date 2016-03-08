using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// A sealed class the implements start and end events of activities, with a modified trigger mehtod.
    /// </summary>
    public sealed class EventActivity : Event
    {
        #region Constructor

        /// <summary>
        /// Constructor that is only called within the constructor of the activity class
        /// </summary>
        /// <param name="parentActivity">Parent activity</param>
        /// <param name="type">Type of event, either start or end</param>
        public EventActivity(Activity parentActivity, EventType type)
            : base(type, parentActivity.ParentControlUnit)
        {
            _parentActivity = parentActivity;
        } // end of Event

        #endregion

        #region ParentActivity

        private Activity _parentActivity;

        /// <summary>
        /// The activity the event belongs to
        /// </summary>
        public Activity ParentActivity
        {
            get
            {
                return _parentActivity;
            }
            set
            {
                _parentActivity = value;
            }
        } // end of ParentActivity

        #endregion

        #region AffectedEntites

        /// <summary>
        /// Returns the affected entities, these are specified in the activity class and are just reflected here
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return ParentActivity.AffectedEntities;
            }
        } // end of AffectedEntities

        #endregion

        #region Trigger

        /// <summary>
        /// Modified trigger method, different handling for start and end events. The custom mehtods to override
        /// state changes are nested in activity class
        /// </summary>
        /// <param name="time">Time event is triggered</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {

            //--------------------------------------------------------------------------------------------------
            // Different handling for start and end events 
            //--------------------------------------------------------------------------------------------------

            if (EventType == EventType.Start)
            {
                // start time of parent activity is set
                ParentActivity.StartTime = time;

                // affected entities are added/removed to and from control units
                foreach (Entity entity in ParentActivity.AffectedEntities)
                {
                    if (entity.ParentControlUnit != null)
                    {
                        if (entity.ParentControlUnit != ParentActivity.ParentControlUnit)
                        {
                            entity.ParentControlUnit.RemoveEntity(entity);
                            ParentActivity.ParentControlUnit.AddEntity(entity);
                        } // end if
                    }
                    else
                    {
                        ParentActivity.ParentControlUnit.AddEntity(entity);
                    } // end if

                    // if the entity is active the activity is added to its current activities
                    if (entity is IActiveEntity)
                        ((IActiveEntity)entity).AddActivity(ParentActivity);
                } // end foreach

                // custom method for state change is called
                ParentActivity.StateChangeStartEvent(time, simEngine);

                // the activity is added to the parents activity list
                ParentActivity.ParentControlUnit.AddActivity(ParentActivity);

                
            }
            else if (EventType == EventType.End)
            {
                // custom state change method is called
                ParentActivity.StateChangeEndEvent(time, simEngine);

                // activity is removed from parent control unit
                ParentActivity.ParentControlUnit.RemoveActivity(ParentActivity);

                // end time of activity is set
                ParentActivity.EndTime = time;

                foreach (Entity entity in ParentActivity.AffectedEntities)
                {
                    // for all active entities the activity is removed from its current activity list
                    if (entity is IActiveEntity)
                        ((IActiveEntity)entity).RemoveActivity(ParentActivity);
                } // end foreach
            } // end if
        } // end of Trigger

        #endregion

        #region ToString

        /// <summary>
        /// ToString method that uses representation of parent activity and strings whether it 
        /// is the start or end event of the activity
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (EventType == EventType.Start)
            {
                return ParentActivity.ToString() + ".Start";
            }
            else
            {
                return ParentActivity.ToString() + ".End";
            } // end if
        } // end of 

        #endregion

        #region Clone

        public override Event Clone()
        {
            return new EventActivity(ParentActivity.Clone(), EventType);
        } // end of Clone

        #endregion

    } // end of EventActivity
}

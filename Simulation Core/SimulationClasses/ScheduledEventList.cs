using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.SimulationClasses
{
    /// <summary>
    /// Implements a scheduling calender for events
    /// </summary>
    public class ScheduledEventList
    {
        #region ScheduledEvent

        /// <summary>
        /// Struct that combines an event and the time it is scheduled
        /// </summary>
        public struct ScheduledEvent
        {
            public DateTime Time;
            public Event Event;

            public override string ToString()
            {
                return "(" + Time.ToString() + "," + Event.ToString() + ")";
            } // end of ToString

        } // end of ScheduledEvent

        #endregion

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        public ScheduledEventList()
        {
            _selList = new List<ScheduledEvent>();
        } // end of ScheduledEventList

        #endregion

        #region ScheduledEvents
        
        /// <summary>
        /// List of all scheduled events
        /// </summary>
        private List<ScheduledEvent> _selList;

        #endregion

        #region RemoveScheduledEvent

        /// <summary>
        /// Removes a scheduled event from the calender
        /// </summary>
        /// <param name="ev">Event that should be removed</param>
        public void RemoveScheduledEvent(Event ev)
        {
            _selList.RemoveAll(p => p.Event == ev);

        } // end of RemoveScheduledEvent

        #endregion

        #region AddScheduledEvent

        /// <summary>
        /// Adds an event to the calender at the specified time
        /// </summary>
        /// <param name="ev">Event to add</param>
        /// <param name="time">Time when event is scheduled</param>
        public void AddScheduledEvent(Event ev, DateTime time)
        {
            ScheduledEvent sEvent = new ScheduledEvent();
            sEvent.Event = ev;
            sEvent.Time = time;

            if (_selList.Count == 0)
            {
                _selList.Add(sEvent);
                return;
            }
            else if (_selList.First().Time >= time)
            {
                _selList.Insert(0, sEvent);
                return;
            }
            else if (_selList.Last().Time <= time)
            {
                _selList.Add(sEvent);
                return;
            } // end if 

            int middleIndex = (_selList.Count - 1) / 2;
            int upperIndex = _selList.Count - 1;
            int lowerIndex = 0;

            while (upperIndex - lowerIndex > 1)
            {
                if (_selList[middleIndex].Time == time)
                {
                    _selList.Insert(middleIndex + 1, sEvent);
                    return;
                }
                else if (_selList[middleIndex].Time < time)
                {
                    lowerIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                }
                else
                {
                    upperIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                } // end if
            } // end while

            _selList.Insert(upperIndex, sEvent);

        } // end of AddScheduledEvent

        #endregion

        #region TriggerFirstEvent

        /// <summary>
        /// Triggers and removes the first event of the calender
        /// </summary>
        /// <param name="simEngine">SimEngine that is responsible for the simulation execution</param>
        public void TriggerFirstEvent(SimulationEngine simEngine)
        {
            if (_selList.Count > 0)
            {
                ScheduledEvent first = _selList.First();

                first.Event.Trigger(first.Time, simEngine);

                RemoveScheduledEvent(first.Event);

            } // end if
        } // end of TriggerFirstEvent

        #endregion

        #region GetFirstEventTime

        /// <summary>
        /// Gets the next time on the calender
        /// </summary>
        /// <returns>Time of the next scheduled event on the calender</returns>
        public DateTime GetFirstEventTime()
        {
            if (_selList.Count > 0)
                return _selList.First().Time;

            return DateTime.MaxValue;
        } // end of

        #endregion

        #region NumberEvents

        /// <summary>
        /// Number of future events scheduled on the calender
        /// </summary>
        public int NumberEvents
        {
            get 
            {
                return _selList.Count;
            }
        } // end of NumberEvent

        #endregion

    } // end of ScheduledEventList
}

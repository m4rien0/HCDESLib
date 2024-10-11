using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleHospitalModel.ModelLog
{
    /// <summary>
    /// Represents a full control unit state, consisting of triggered events, and a logging state, needs revision
    /// </summary>
    public class SimulationState
    {
        #region Constructor

        /// <summary>
        ///
        /// </summary>
        /// <param name="rootControlUnitState"></param>
        /// <param name="triggeredEvents"></param>
        /// <param name="time"></param>
        public SimulationState(LogControlUnitState rootControlUnitState, IEnumerable<Event> triggeredEvents, DateTime time)
        {
            _rootControlUnitState = rootControlUnitState;
            //_triggeredEvents = triggeredEvents;
            _timeStamp = time;

            StringBuilder state = new StringBuilder();

            //state.Append(RootControlUnitState.GetDescription());
            foreach (Event ev in triggeredEvents)
            {
                state.Append(TimeStamp.ToString("MM.dd:HH:mm:ss:fff") + ": ");
                state.Append(ev.GetDescription());
                state.AppendLine();
            } // end foreach

            //state.AppendLine("----------------------------------------------------------------------------------------------------------");

            _logState = state.ToString();
        } // end of SimulationState

        #endregion Constructor

        #region TimeStamp

        private DateTime _timeStamp;

        public DateTime TimeStamp
        {
            get
            {
                return _timeStamp;
            }
        } // end of TimeStamp

        #endregion TimeStamp



        #region RootControlUnitState

        private LogControlUnitState _rootControlUnitState;

        public LogControlUnitState RootControlUnitState
        {
            get
            {
                return _rootControlUnitState;
            }
        } // end of RootControlUnitState

        #endregion RootControlUnitState

        #region TriggeredEvents

        private List<Event> _triggeredEvents;

        public List<Event> TriggeredEvents
        {
            get
            {
                return _triggeredEvents;
            }
        } // end of TriggeredEvents

        #endregion TriggeredEvents

        #region EventLog

        private string _eventLog;

        public string EventLog
        {
            get
            {
                return _eventLog;
            }
        } // end of EventLog

        #endregion EventLog

        #region LogState

        private string _logState;

        public string LogState
        {
            get
            {
                return _logState;
            }
        } // end of LogState

        #endregion LogState

        #region GetDescription

        public string GetDescription()
        {
            return LogState;
        } // end of GetDescription

        #endregion GetDescription
    } // end of FullSimulationState
}
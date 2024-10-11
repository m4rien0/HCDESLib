using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using System;
using System.Collections.Generic;

namespace SimulationCore.SimulationClasses
{
    /// <summary>
    /// Example of a simulation engine that can execute a simulation run
    /// </summary>
    public class SimulationEngine : ISimulationEngine
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that includes a visualization engine and a logging engine
        /// </summary>
        /// <param name="drawingEngine">The drawing engine to be used</param>
        /// <param name="loggingEngine">The logging engine to be used</param>
        public SimulationEngine(IDrawingSimulationEngine drawingEngine, IModelLog loggingEngine)
        {
            _currentlyTriggeredEvents = new List<Event>();
            _drawingEngine = drawingEngine;
            _selList = new ScheduledEventList();
            _loggingEngine = loggingEngine;
        } // end of SimulationEngine

        public SimulationEngine()
        {
            _currentlyTriggeredEvents = new List<Event>();
            _selList = new ScheduledEventList();
        } // end of SimulationEngine

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // State Reporting
        //--------------------------------------------------------------------------------------------------

        #region DrawingEngine

        private IDrawingSimulationEngine _drawingEngine;

        /// <summary>
        /// Drawing Engine to be used by the simulation engine
        /// </summary>
        public IDrawingSimulationEngine DrawingEngine
        {
            get
            {
                return _drawingEngine;
            }
        } // end of DrawingEngine

        #endregion DrawingEngine

        #region LoggingEngine

        private IModelLog _loggingEngine;

        /// <summary>
        /// Logging engine to be used
        /// </summary>
        public IModelLog LoggingEngine
        {
            get
            {
                return _loggingEngine;
            }
            set
            {
                _loggingEngine = value;
            }
        } // end of LoggingEngine

        #endregion LoggingEngine

        #region CreateEventLog

        private bool _createEventLog;

        /// <summary>
        /// Flag if event log should be made
        /// </summary>
        public bool CreateEventLog
        {
            get
            {
                return _createEventLog;
            }
            set
            {
                _createEventLog = value;
            }
        } // end of CreateEventLog

        #endregion CreateEventLog

        #region CurrentlyTriggeredEvents

        private List<Event> _currentlyTriggeredEvents;

        /// <summary>
        /// List of events that have been triggered in the current simulation step
        /// </summary>
        public List<Event> CurrentlyTriggeredEvents
        {
            get
            {
                return _currentlyTriggeredEvents;
            }
        } // end of CurrentlyTriggeredEvents

        #endregion CurrentlyTriggeredEvents

        #region LogEvent

        /// <summary>
        /// Logs an event
        /// </summary>
        /// <param name="logedEvent"></param>
        public void LogEvent(Event logedEvent)
        {
            _currentlyTriggeredEvents.Add(logedEvent);
        } // end of LogEvent

        #endregion LogEvent

        //--------------------------------------------------------------------------------------------------
        // ISimulationEngine Methods
        //--------------------------------------------------------------------------------------------------

        #region AddScheduledEvent

        public void AddScheduledEvent(Event ev, DateTime time)
        {
            SELlist.AddScheduledEvent(ev, time);
        } // end of AddScheduledEvent

        #endregion AddScheduledEvent

        #region RemoveScheduledEvent

        public void RemoveScheduledEvent(Event ev)
        {
            SELlist.RemoveScheduledEvent(ev);
        } // end of RemoveScheduledEvent

        #endregion RemoveScheduledEvent

        #region RunSimulationModel

        /// <summary>
        /// Runs the entire simulation model, start and endtime are specified by the model
        /// </summary>
        /// <param name="simModel">The simulation model to be ran</param>
        /// <returns></returns>
        public void RunSimulationModel(SimulationModel simModel)
        {
            DateTime currentTime = simModel.StartTime;

            while (!SimulationModel.StopSimulation(currentTime))
            {
                DateTime newTime;
                bool modelRunning = RunSingleStepSimulationModel(currentTime, out newTime);

                if (modelRunning)
                {
                    currentTime = newTime;
                } // end if
            } // end while

            SimulationModel.CreateSimulationResultsAfterStop();
        } // end of RunSimulationModel

        #endregion RunSimulationModel

        #region RunSingleStepSimulationModel

        /// <summary>
        /// Runs a single time step of a simulation model, i.e. triggering all events that occur at
        /// the current time
        /// </summary>
        /// <param name="currentTime">Current time of the execution</param>
        /// <param name="newTime">Next time events will occur</param>
        /// <returns>Returns false if no future events exist, else returns true</returns>
        public bool RunSingleStepSimulationModel(DateTime currentTime, out DateTime newTime)
        {
            _currentlyTriggeredEvents.Clear();

            newTime = currentTime;

            // all scheduled behavior of this time is executed
            if (SELlist.NumberEvents == 0)
            {
                newTime = SimulationModel.EndTime;
                return false;
            }
            else
            {
                while (SELlist.GetFirstEventTime() == currentTime)
                {
                    SELlist.TriggerFirstEvent(this);
                } // end while
            } // end if

            // perform conditional rules on model
            SimulationModel.PerformModelRules(CurrentlyTriggeredEvents, currentTime, this);

            if (CreateEventLog)
                LoggingEngine.CreateCurrentState(CurrentlyTriggeredEvents, currentTime);

            if (SELlist.NumberEvents > 0)
            {
                newTime = SELlist.GetFirstEventTime();
                return true;
            }
            else
            {
                newTime = SimulationModel.EndTime;
                return false;
            } // end if
        } // end of RunSingleStepSimulationModel

        #endregion RunSingleStepSimulationModel

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region SimulationModel

        private SimulationModel _simulationModel;

        /// <summary>
        /// The model that is simulated
        /// </summary>
        public SimulationModel SimulationModel
        {
            get
            {
                return _simulationModel;
            }
            set
            {
                _simulationModel = value;
            }
        } // end of SimulationModel

        #endregion SimulationModel

        #region SELlist

        private ScheduledEventList _selList;

        /// <summary>
        /// The future events calender
        /// </summary>
        private ScheduledEventList SELlist
        {
            get
            {
                return _selList;
            }
        } // end of SELlist

        #endregion SELlist
    }
}
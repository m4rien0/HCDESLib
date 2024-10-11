using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.ModelLog
{
    /// <summary>
    /// Class to create output measures, data is collected upon triggering of events and after rule triggering of
    /// control units.
    /// </summary>
    public abstract class GenericOutputGenereator : IModelLog
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, delegate dictionaries for event and control unit data collection
        /// methods are initialized
        /// </summary>
        /// <param name="parentSimulationModel">Simulation model data is collected for</param>
        /// <param name="endOfWarmUpPeriod">Time after which data is collected</param>
        public GenericOutputGenereator(SimulationModel parentSimulationModel, DateTime endOfWarmUpPeriod)
        {
            _parentSimulationModel = parentSimulationModel;

            _collectingMethodsPerControlUnitType = new Dictionary<Type, CollectControlUnitStateData>();
            _collectingMethodsPerStandaloneEventType = new Dictionary<Type, CollectEventData>();
            _collectingMethodsPerActivityStartEventType = new Dictionary<Type, CollectActivityData>();
            _collectingMethodsPerActivityEndEventType = new Dictionary<Type, CollectActivityData>();
        } // end of

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region EndOfWarmUpPeriod

        private DateTime _endOfWarmUpPeriod;

        /// <summary>
        /// Time after which data is collected
        /// </summary>
        public DateTime EndOfWarmUpPeriod
        {
            get
            {
                return _endOfWarmUpPeriod;
            }
            set
            {
                _endOfWarmUpPeriod = value;
            }
        } // end of EndOfWarmUpPeriod

        #endregion EndOfWarmUpPeriod

        #region ParentSimulationModel

        private SimulationModel _parentSimulationModel;

        /// <summary>
        /// Simulation model data is collected for
        /// </summary>
        public SimulationModel ParentSimulationModel
        {
            get
            {
                return _parentSimulationModel;
            }
        } // end of ParentSimulationModel

        #endregion ParentSimulationModel

        #region DelegateCollectEventData

        /// <summary>
        /// Delegate that defines data collection methods for events
        /// </summary>
        /// <param name="ev">Event data is collected for</param>
        /// <param name="time">Time data is collected</param>
        protected delegate void CollectEventData(Event ev, DateTime time);

        #endregion DelegateCollectEventData

        #region DelegateCollectActivityData

        /// <summary>
        /// Delegate that defines data collection methods for activities
        /// </summary>
        /// <param name="ev">Activity data is collected for</param>
        /// <param name="time">Time data is collected</param>
        protected delegate void CollectActivityData(Activity activity, DateTime time);

        #endregion DelegateCollectActivityData

        #region DelegateCollectControlUnitStateData

        /// <summary>
        /// Delegate that defines data collection methods for control units
        /// </summary>
        /// <param name="ev">Control unit data is collected for</param>
        /// <param name="time">Time data is collected</param>
        protected delegate void CollectControlUnitStateData(ControlUnit ev, DateTime time);

        #endregion DelegateCollectControlUnitStateData

        #region CollectingMethodsPerEventType

        private Dictionary<Type, CollectEventData> _collectingMethodsPerStandaloneEventType;

        /// <summary>
        /// Dictionary that holds methods for event types to collect data
        /// </summary>
        protected Dictionary<Type, CollectEventData> CollectingMethodsPerStandaloneEventType
        {
            get
            {
                return _collectingMethodsPerStandaloneEventType;
            }
        } // end of CollectingMethodsPerStandaloneEventType

        #endregion CollectingMethodsPerEventType

        #region CollectingMethodsPerActivityStartEventType

        private Dictionary<Type, CollectActivityData> _collectingMethodsPerActivityStartEventType;

        /// <summary>
        /// Dictionary that holds methods for activity start event types to collect data
        /// </summary>
        protected Dictionary<Type, CollectActivityData> CollectingMethodsPerActivityStartEventType
        {
            get
            {
                return _collectingMethodsPerActivityStartEventType;
            }
        } // end of CollectingMethodsPerActivityStartEventType

        #endregion CollectingMethodsPerActivityStartEventType

        #region CollectingMethodsPerActivityEndEventType

        private Dictionary<Type, CollectActivityData> _collectingMethodsPerActivityEndEventType;

        /// <summary>
        /// Dictionary that holds methods for activity end event types to collect data
        /// </summary>
        protected Dictionary<Type, CollectActivityData> CollectingMethodsPerActivityEndEventType
        {
            get
            {
                return _collectingMethodsPerActivityEndEventType;
            }
        } // end of CollectingMethodsPerActivityEndEventType

        #endregion CollectingMethodsPerActivityEndEventType

        #region CollectingMethodsPerControlUnitType

        private Dictionary<Type, CollectControlUnitStateData> _collectingMethodsPerControlUnitType;

        /// <summary>
        /// Dictionary that holds methods for control unit types to collect data
        /// </summary>
        protected Dictionary<Type, CollectControlUnitStateData> CollectingMethodsPerControlUnitType
        {
            get
            {
                return _collectingMethodsPerControlUnitType;
            }
            set
            {
                _collectingMethodsPerControlUnitType = value;
            }
        } // end of CollectingMethodsPerControlUnitType

        #endregion CollectingMethodsPerControlUnitType

        //--------------------------------------------------------------------------------------------------
        // Interface Methods
        //--------------------------------------------------------------------------------------------------

        #region CreateCurrentState

        /// <summary>
        /// Creates the current state, for each triggered event the appropriate method is called if any
        /// is stored in the dictionaries, for all control units it is checked if a data collection method
        /// was stored, if so it is called
        /// </summary>
        /// <param name="currentEvents">List of events that have been triggered at current time of simulation</param>
        /// <param name="time">Current time of simulation</param>
        public void CreateCurrentState(List<Event> currentEvents, DateTime time)
        {
            if (time < EndOfWarmUpPeriod)
                return;

            foreach (Event ev in currentEvents)
            {
                if (ev is EventActivity)
                {
                    EventActivity activityEvent = ev as EventActivity;
                    Type activityType = activityEvent.ParentActivity.GetType();

                    if (activityEvent.EventType == EventType.Start)
                    {
                        if (CollectingMethodsPerActivityStartEventType.ContainsKey(activityType))
                            CollectingMethodsPerActivityStartEventType[activityType](activityEvent.ParentActivity, time);
                    }
                    else if (((EventActivity)ev).EventType == EventType.End)
                    {
                        if (CollectingMethodsPerActivityEndEventType.ContainsKey(activityType))
                            CollectingMethodsPerActivityEndEventType[activityType](activityEvent.ParentActivity, time);
                    } // end if
                }
                else
                {
                    if (CollectingMethodsPerStandaloneEventType.ContainsKey(ev.GetType()))
                        CollectingMethodsPerStandaloneEventType[ev.GetType()](ev, time);
                } // end if
            } // end foreach

            foreach (ControlUnit control in ParentSimulationModel.ControlUnits.Values)
            {
                if (CollectingMethodsPerControlUnitType.ContainsKey(control.GetType()))
                    CollectingMethodsPerControlUnitType[control.GetType()](control, time);
            } // end foreach
        } // end of CreateCurrentState

        #endregion CreateCurrentState

        #region GetCurrentStateRepresentation

        /// <summary>
        /// No representation implemented in base class
        /// </summary>
        /// <returns></returns>
        public string GetCurrentStateRepresentation()
        {
            throw new NotImplementedException();
        } // end of GetCurrentStateRepresentation

        #endregion GetCurrentStateRepresentation

        #region CreateSimulationResult

        /// <summary>
        /// Abstract method to create result from collected data
        /// </summary>
        public abstract void CreateSimulationResult();

        #endregion CreateSimulationResult

        //--------------------------------------------------------------------------------------------------
        // Helpers
        //--------------------------------------------------------------------------------------------------

        #region GetIntermediateValuesTimeStampedData

        /// <summary>
        /// Helper function to create a time series of values over fixed time interval lengths
        /// from data that is generated at discrete not periodic time stamps
        /// </summary>
        /// <typeparam name="T">Type of collected data</typeparam>
        /// <param name="timeStampedData">Data collected at non-periodic time stamps</param>
        /// <param name="resolution">Resolution of periodic time series that should be generated</param>
        /// <returns>Data at periodic time stamps</returns>
        public List<T> GetIntermediateValuesTimeStampedData<T>(Dictionary<DateTime, T> timeStampedData, TimeSpan resolution)
        {
            List<DateTime> orderedTime = timeStampedData.Keys.OrderBy(p => p.Ticks).ToList();

            if (orderedTime.Count <= 1)
                return timeStampedData.Values.ToList();

            DateTime startTime = orderedTime.First();
            DateTime endTime = orderedTime.Last();

            DateTime currentRefTime = startTime;
            DateTime currentTime = startTime;
            int currentIndex = 0;

            List<T> data = new List<T>();

            data.Add(timeStampedData[currentRefTime]);

            while (currentTime < endTime)
            {
                currentTime += resolution;
            } // end while

            return new List<T>();
        } // end of GetIntermediateValuesTimeStampedData

        #endregion GetIntermediateValuesTimeStampedData
    } // end of GenericOutputGenereator
}
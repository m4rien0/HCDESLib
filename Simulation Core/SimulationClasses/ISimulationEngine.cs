using SimulationCore.HCCMElements;
using System;

namespace SimulationCore.SimulationClasses
{
    public interface ISimulationEngine
    {
        /// <summary>
        /// Logs an event that has been triggered in the hccm model
        /// </summary>
        /// <param name="logedEvent">Event to be logged</param>
        void LogEvent(Event logedEvent);

        /// <summary>
        /// Adds an event at a scheduled time in the future
        /// </summary>
        /// <param name="ev">Event that should be scheduled</param>
        /// <param name="time">Time when event should be triggered</param>
        void AddScheduledEvent(Event ev, DateTime time);

        //
        /// <summary>
        /// In case a scheduled event is not needed any more
        /// it has to be removed from the simulation engine
        /// </summary>
        /// <param name="ev">Event to be removed</param>
        void RemoveScheduledEvent(Event ev);

        /// <summary>
        /// Runs the entire simulation model, start and endtime are specified by the model
        /// </summary>
        /// <param name="simModel">The simulation model to be ran</param>
        /// <returns></returns>
        void RunSimulationModel(SimulationModel simModel);

        /// <summary>
        /// Runs a single time step of a simulation model, i.e. triggering all events that occur at
        /// the current time
        /// </summary>
        /// <param name="currentTime">Current time of the execution</param>
        /// <param name="newTime">Next time events will occur</param>
        /// <returns>Returns false if no future events exist, else returns true</returns>
        bool RunSingleStepSimulationModel(DateTime currentTime, out DateTime newTime);
    } // end of ISimulationEngine
}
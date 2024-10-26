using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using SimulationCore.SimulationClasses;
using System.IO;

namespace ActorDemo.Logging
{
    public class BaseLoggingEngine(SimulationModel parentSimulationModel) : IModelLog
    {
        /// <summary>
        /// Simulation model that is executed
        /// </summary>
        public SimulationModel ParentSimulationModel { get; set; } = parentSimulationModel;

        /// <summary>
        /// List of states that represent the simulation result
        /// </summary>
        public List<SimulationState> SimulationResult { get; set; } = [];

        /// <summary>
        /// Creates a LogControlUnitState and stores it in the simulation result
        /// </summary>
        /// <param name="currentEvents">Events triggered at current time</param>
        /// <param name="time">Current time of simulation execution</param>
        public void CreateCurrentState(List<Event> eventsTriggered, DateTime time)
        {
            LogControlUnitState currentState = new(time, this, ParentSimulationModel.RootControlUnit);

            SimulationResult.Add(new SimulationState(currentState, eventsTriggered, time));
        }

        /// <summary>
        /// Creates the simulation result by creating a txt file with the event log
        /// </summary>
        public void CreateSimulationResult()
        {
            StreamWriter outfile = new("Output.txt");

            for (int i = 0; i < SimulationResult.Count; i++)
            {
                outfile.Write(SimulationResult[i].GetDescription());
            }

            outfile.Close();
        }

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <returns>String that represents the current model state</returns>
        public string GetCurrentStateRepresentation()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns the string representation of an entity type
        /// </summary>
        /// <param name="defaultEntitiyType">The type of the entity</param>
        /// <returns></returns>
        public string GetStringRepDefaultEntityType(Type defaultEntitiyType)
        {
            return defaultEntitiyType.Name;
        }
    }
}
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.Interfaces
{
    /// <summary>
    /// Interface defining logging engines for simulation models
    /// </summary>
    public interface IModelLog
    {
        /// <summary>
        /// Creates simulation states for logging or output creation at given time.
        /// </summary>
        /// <param name="currentEvents">Events triggered at current time</param>
        /// <param name="time">Current time of simulation execution</param>
        void CreateCurrentState(List<Event> currentEvents, DateTime time);

        /// <summary>
        /// String representation of the current state, e.g. for logging
        /// </summary>
        /// <returns>String that represents the current model state</returns>
        string GetCurrentStateRepresentation();

        /// <summary>
        /// Method that produces simulation results after model execution
        /// </summary>
        void CreateSimulationResult();
    }
}

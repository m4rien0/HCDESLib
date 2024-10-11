using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;

namespace SimulationCore.Interfaces
{
    public interface IDrawingSimulationEngine
    {
        /// <summary>
        /// Creates a visualization of a simulation model at a given time.
        /// </summary>
        /// <param name="currentTime">Time at which model should be visualized</param>
        /// <param name="simModel">Model to visualize</param>
        void CreateModelVisualization(DateTime currentTime, SimulationModel simModel, IEnumerable<Event> currentEvents);

        /// <summary>
        /// Initilializes the visualization, can be at start of simulation (e.g. static visualization), at re-enabling
        /// visualization during a visualization run or re-runs after completion
        /// </summary>
        /// <param name="initializeTime">The time the model is initialized</param>
        /// <param name="simulationModel">The model at the initilization time</param>
        void InitializeModelVisualizationAtTime(DateTime initializeTime, SimulationModel simulationModel);
    } // end of IDrawingSimulationEngine
}
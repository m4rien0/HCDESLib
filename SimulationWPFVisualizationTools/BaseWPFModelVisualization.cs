using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFVisualizationBase;

namespace SimulationWPFVisualizationTools
{
    /// <summary>
    /// Base class to visualize a simulation model on a DrawingOnCoordinateSystem, Visualization is splitted in
    /// Visualization engines per control units
    /// </summary>
    public class BaseWPFModelVisualization : IDrawingSimulationEngine
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, initializes a dictionary of BaseWPFControlUnitVisualizationEngine per control unit
        /// </summary>
        /// <param name="simulationModel">The simulation model to visualize</param>
        /// <param name="drawingSystem">The drawing system the model is visualized in</param>
        public BaseWPFModelVisualization(SimulationModel simulationModel, DrawingOnCoordinateSystem drawingSystem)
        {
            _simulationModel = simulationModel;

            _visualizationPerControlUnit = new Dictionary<ControlUnit, BaseWPFControlUnitVisualizationEngine>();

            _drawingSystem = drawingSystem;
        } // end of BaseWPFModelVisualization

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region SimulationModel

        private SimulationModel _simulationModel;

        /// <summary>
        /// The simulation model to visualize
        /// </summary>
        public SimulationModel SimulationModel
        {
            get
            {
                return _simulationModel;
            }
        } // end of SimulationModel

        #endregion

        #region VisualizationPerControlUnit

        private Dictionary<ControlUnit,BaseWPFControlUnitVisualizationEngine> _visualizationPerControlUnit;

        /// <summary>
        /// Holds BaseWPFControlUnitVisualizationEngine for control units, control units that are not contained
        /// no visualization is done
        /// </summary>
        public Dictionary<ControlUnit,BaseWPFControlUnitVisualizationEngine> VisualizationPerControlUnit
        {
            get
            {
                return _visualizationPerControlUnit;
            }
            set
            {
                _visualizationPerControlUnit = value;
            }
        } // end of VisualizationPerControlUnit

        #endregion

        #region DrawingSystem

        private DrawingOnCoordinateSystem _drawingSystem;

        /// <summary>
        /// The drawing system the model is visualized in
        /// </summary>
        public DrawingOnCoordinateSystem DrawingSystem
        {
            get
            {
                return _drawingSystem;
            }
        } // end of DrawingSystem

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region CreateModelVisualization

        /// <summary>
        /// Creates a visualization of a simulation model at a given time. For each control unit
        /// of the model it is checked if a BaseWPFControlUnitVisualizationEngine has been
        /// specified and if so it is used to create the dynamic visualization of the control.
        /// If not nothing is done for that control unit.
        /// </summary>
        /// <param name="currentTime">Time at which model should be visualized</param>
        /// <param name="simModel">Model to visualize</param>
        /// <param name="currentEvents">Currently triggered events in the model</param>
        public void CreateModelVisualization(DateTime currentTime, SimulationModel simModel, IEnumerable<Event> currentEvents)
        {
            AdditionalDynamicVisualization(currentTime);

            foreach (ControlUnit control in SimulationModel.ControlUnits.Values)
            {
                if (VisualizationPerControlUnit.ContainsKey(control))
                    VisualizationPerControlUnit[control].VisualizeDynamicModel(currentTime, SimulationModel, control, currentEvents.Where(p=> p.ParentControlUnit == control));
            } // end foreach

        } // end of CreateModelVisualization

        #endregion

        #region InitializeModelVisualizationAtTime

        // <summary>
        /// Initilializes the visualization, can be at start of simulation (e.g. static visualization), at re-enabling
        /// visualization during a visualization run or re-runs after completion.  For each control unit
        /// of the model it is checked if a BaseWPFControlUnitVisualizationEngine has been
        /// specified and if so it is used to create the static visualization of the control.
        /// If not nothing is done for that control unit. 
        /// </summary>
        /// <param name="initializeTime">The time the model is initialized</param>
        /// <param name="simulationModel">The model at the initilization time</param>
        public void InitializeModelVisualizationAtTime(DateTime initializeTime, SimulationModel simulationModel)
        {
            DrawingSystem.ClearSystem();

            AdditionalStaticVisualization(initializeTime);

            foreach (ControlUnit control in SimulationModel.ControlUnits.Values)
            {
                if (VisualizationPerControlUnit.ContainsKey(control))
                    VisualizationPerControlUnit[control].IntializeVisualizationAtTime(initializeTime, SimulationModel, control);
            } // end foreach

        } // end of InitializeModelVisualizationAtTime

        #endregion

        #region AdditionalDynamicVisualization

        /// <summary>
        /// Additional dynamic visualization that is not done via control units
        /// </summary>
        /// <param name="currentTime">Time the model is visualized</param>
        public virtual void AdditionalDynamicVisualization(DateTime currentTime)
        {
        } // end of AdditionalDynamicVisualization

        #endregion

        #region AdditionalStaticVisualization

        /// <summary>
        /// Additional static visualization that is not done via control units
        /// </summary>
        /// <param name="currentTime">Time the model visualization is initalized</param>
        public virtual void AdditionalStaticVisualization(DateTime initializationTime)
        {
        } // end of AdditionalStaticVisualization

        #endregion

    } // end of BaseWPFModelVisualization
}

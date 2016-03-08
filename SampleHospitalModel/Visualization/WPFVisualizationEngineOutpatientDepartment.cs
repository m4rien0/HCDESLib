using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace SampleHospitalModel.Visualization
{
    /// <summary>
    /// Additional visualization for outpatient control units, with extra visualization of waiting list numbers:
    /// Number of patients waiting for a slot to be assigned
    /// Number of patients waiting for slots
    /// </summary>
    public class WPFVisualizationEngineOutpatientDepartment : WPFVisualizationEngineHealthCareDepartmentControlUnit<OutpatientActionTypeClass>
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, just calls base constructor
        /// </summary>
        /// <param name="drawingSystem">Drawing system used for visualization</param>
        /// <param name="position">Position of control unit visualization on drawing system</param>
        /// <param name="size">Size of control unit visualization on drawing system</param>
        /// <param name="personSize">Size in which persons are visualized</param>
        public WPFVisualizationEngineOutpatientDepartment(DrawingOnCoordinateSystem drawingSystem,
                                                          Point position,
                                                          Size size,
                                                          double personSize) : base(drawingSystem, position, size, personSize)
        {

        } // end of BaseWPFControlUnitVisualizationEngine

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region NumberSlotWaitString

        private DrawingObjectString _numberSlotWaitString;

        /// <summary>
        /// Drawing object for the string representing the number of patients waiting
        /// for a slot
        /// </summary>
        public DrawingObjectString NumberSlotWaitString
        {
            get
            {
                return _numberSlotWaitString;
            }
        } // end of NumberSlotWaitString

        #endregion

        #region NumberSlotAssignString

        private DrawingObjectString _numberSlotAssingString;

        /// <summary>
        /// Drawing object for the string representing the number of patients waiting
        /// for a slot to be assigned
        /// </summary>
        public DrawingObjectString NumberSlotAssignString
        {
            get
            {
                return _numberSlotAssingString;
            }
        } // end of NumberSlotAssignString

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region AdditionalStaticVisualization

        /// <summary>
        /// Calls base class method and additiona visualization for string captions
        /// </summary>
        /// <param name="initializationTime">Time at which static visualization is generated</param>
        /// <param name="simModel">Simulation model for which the visuslization is generated</param>
        /// <param name="parentControlUnit">Control unit that is visualized</param>
        public override void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            base.AdditionalStaticVisualization(initializationTime, simModel, parentControlUnit);

            DrawingObjectString personWaitingforSlotString = new DrawingObjectString(Position - new Vector(0,50), "Patients Waiting for Slot: ", CustomStringAlignment.Left, 24, Colors.Gray);

            _numberSlotWaitString = new DrawingObjectString(Position - new Vector(-500, 50), "0", CustomStringAlignment.Left, 24, Colors.Gray);

            DrawingSystem.AddObject(personWaitingforSlotString);
            DrawingSystem.AddObject(NumberSlotWaitString);

            DrawingObjectString personWaitingforSlotAssignString = new DrawingObjectString(Position - new Vector(0, 100), "Patients Waiting to be assigned to Slot: ", CustomStringAlignment.Left, 24, Colors.Gray);

            _numberSlotAssingString = new DrawingObjectString(Position - new Vector(-500, 100), "0", CustomStringAlignment.Left, 24, Colors.Gray);

            DrawingSystem.AddObject(NumberSlotAssignString);
            DrawingSystem.AddObject(personWaitingforSlotAssignString);

        } // end of AdditionalStaticVisualization

        #endregion

        #region AdditionalDynamicVisualization

        /// <summary>
        /// Additionally to event handling methods for visualization, the number of patients waiting
        /// for a slot to be assigned or waiting for a slot are showed.
        /// </summary>
        /// <param name="currentTime">Time for additional dynamic visualization</param>
        /// <param name="simModel">Simulation model to be visualized</param>
        /// <param name="parentControlUnit">Control to be visualized</param>
        /// <param name="currentEvents">Events that have been triggered at current time</param>
        public override void AdditionalDynamicVisualization(DateTime currentTime, SimulationModel simModel, ControlUnit parentControlUnit, IEnumerable<Event> currentEvents)
        {

            ControlUnit waitingListControl = ((ControlUnitOutpatient)parentControlUnit).WaitingListControlUnit;

            NumberSlotAssignString.StringToDraw = waitingListControl.CurrentActivities.Where(p => p.ActivityName == "ActivityOutpatientWaitingListWaitToAssignSlot").Count().ToString();
            NumberSlotWaitString.StringToDraw = waitingListControl.CurrentActivities.Where(p => p.ActivityName == "ActivityWait").Count().ToString();

        } // end of AdditionalDynamicVisualization

        #endregion

    } // end of WPFVisualizationEngineOutpatientDepartment
}

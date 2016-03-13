using SimulationCore.HCCMElements;
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
    /// Base visualization of control units, basic principle is that methods for events (standalon, activity start and activity end)
    /// can be specified (defined by delegates) that are then automatically called by this class to visualize changes. Methods to
    /// obtain drawing objects for entities handle object creation.
    /// Holds all drawing objects for entities.
    /// </summary>
    public abstract class BaseWPFControlUnitVisualizationEngine
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructors, dictionaries for handling methods per type are initialized
        /// </summary>
        /// <param name="drawingSystem">The drawing system used for visualization</param>
        public BaseWPFControlUnitVisualizationEngine(DrawingOnCoordinateSystem drawingSystem)
        {
            _drawingSystem = drawingSystem;
            _drawingObjectPerEntity = new Dictionary<Entity, DrawingObject>();
            _entityVisualizationObjectCreatingMethodPerType = new Dictionary<Type, CreateEntityDrawingObject>();
            _activityStartEventVisualizationMethods = new Dictionary<Type, ActivityVisualizationMethod>();
            _activityEndEventVisualizationMethods = new Dictionary<Type, ActivityVisualizationMethod>();
            _activityIntermediateVisualizationMethods = new Dictionary<Type, ActivityVisualizationMethod>();
            _holdingEntitiesVisualizationMethods = new Dictionary<Type, HoldingEntitiesVisualizationMethod>();
            _eventStandaloneDrawingMehtods = new Dictionary<Type, EventVisualizationMethod>();
        } // end of BaseWPFControlUnitVisualizationEngine

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Delegate Definitions 
        //--------------------------------------------------------------------------------------------------

        #region EventVisualizationMethod

        /// <summary>
        /// Delegate for visualizing standalone events
        /// </summary>
        /// <param name="ev">Event to visualize</param>
        protected delegate void EventVisualizationMethod(Event ev);

        #endregion

        #region ActivityVisualizationMethod

        /// <summary>
        /// Delegate for visualizing activity events and intermediate action of activities
        /// </summary>
        /// <param name="ev">Activity to visualize</param>
        /// <param name="time">Time to visualize</param>
        protected delegate void ActivityVisualizationMethod(Activity activity, DateTime time);

        #endregion

        #region CreateEntityDrawingObject

        /// <summary>
        /// Delegate to define entity drawing object creation
        /// </summary>
        /// <param name="entity">Entity object should be created for</param>
        /// <returns>A drawing object to visualize entity</returns>
        protected delegate DrawingObject CreateEntityDrawingObject(Entity entity);

        #endregion

        #region HoldingEntitiesVisualizationMethod

        /// <summary>
        /// Delegetate used for the definition of how holding entities should be visualized
        /// </summary>
        /// <param name="holdingEntity">The holding entity to visualize</param>
        protected delegate void HoldingEntitiesVisualizationMethod(IDynamicHoldingEntity holdingEntity);

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region DrawingSystem

        private DrawingOnCoordinateSystem _drawingSystem;

        /// <summary>
        /// The drawing system used for visualization
        /// </summary>
        public DrawingOnCoordinateSystem DrawingSystem
        {
            get
            {
                return _drawingSystem;
            }
        } // end of DrawingSystem

        #endregion

        #region EntityVisualizationObjectCreatingMethodsPerType

        private Dictionary<Type, CreateEntityDrawingObject> _entityVisualizationObjectCreatingMethodPerType;

        /// <summary>
        /// Methods for creation of entities of a specific types can be stored
        /// </summary>
        protected Dictionary<Type, CreateEntityDrawingObject> EntityVisualizationObjectCreatingMethodsPerType
        {
            get
            {
                return _entityVisualizationObjectCreatingMethodPerType;
            }
        } // end of EntityVisualizationObjectCreatingMethodsPerType

        #endregion

        #region EventStandaloneDrawingMethods

        private Dictionary<Type,EventVisualizationMethod> _eventStandaloneDrawingMehtods;

        /// <summary>
        /// Methods for visualization of standalone events can be stored
        /// </summary>
        protected Dictionary<Type, EventVisualizationMethod> EventStandaloneDrawingMethods
        {
            get
            {
                return _eventStandaloneDrawingMehtods;
            }
        } // end of EventStandaloneDrawingMethods

        #endregion

        #region ActivityStartEventVisualizationMethods

        private Dictionary<Type, ActivityVisualizationMethod> _activityStartEventVisualizationMethods;

        /// <summary>
        /// Methods for visualization of activity start events can be stored
        /// </summary>
        protected Dictionary<Type, ActivityVisualizationMethod> ActivityStartEventVisualizationMethods
        {
            get
            {
                return _activityStartEventVisualizationMethods;
            }
        } // end of ActivityStartEventVisualizationMethods

        #endregion

        #region ActivityEndEventVisualizationMethods

        private Dictionary<Type, ActivityVisualizationMethod> _activityEndEventVisualizationMethods;

        /// <summary>
        /// Methods for visualization of activity end events can be stored
        /// </summary>
        protected Dictionary<Type, ActivityVisualizationMethod> ActivityEndEventVisualizationMethods
        {
            get
            {
                return _activityEndEventVisualizationMethods;
            }
        } // end of ActivityEndEventVisualizationMethods

        #endregion

        #region ActivityIntermediateVisualizationMethods

        private Dictionary<Type, ActivityVisualizationMethod> _activityIntermediateVisualizationMethods;

        /// <summary>
        /// Methods for visualization of activity intermediate times can be stored
        /// </summary>
        protected Dictionary<Type, ActivityVisualizationMethod> ActivityIntermediateVisualizationMethods
        {
            get
            {
                return _activityIntermediateVisualizationMethods;
            }
        } // end of ActivityIntermediateVisualizationMethods

        #endregion

        #region HoldingEntitiesVisualizationMethods

        private Dictionary<Type, HoldingEntitiesVisualizationMethod> _holdingEntitiesVisualizationMethods;

        /// <summary>
        /// Methods for visualization of holding entities can be stored
        /// </summary>
        protected Dictionary<Type, HoldingEntitiesVisualizationMethod> HoldingEntitiesVisualizationMethods
        {
            get
            {
                return _holdingEntitiesVisualizationMethods;
            }
        } // end of HoldingEntitiesVisualizationMethods

        #endregion

        #region DrawingObjectPerEntity
        
        private Dictionary<Entity, DrawingObject> _drawingObjectPerEntity;

        /// <summary>
        /// General method to obtain a drawing object for an entity, looks if a 
        /// object exists for that entity, if not one is created. Therefore, it is checked if
        /// method for object creation has been speciefied for the type of entity
        /// and calls the corresponding method, otherwise throughs an exception.
        /// </summary>
        /// <param name="entity">Entity for which drawing object is required</param>
        /// <returns>Drawing object for entity</returns>
        public DrawingObject DrawingObjectPerEntity(Entity entity)
        {
            // if entity drawing object hasn't been created
            // the corresponding method is looked up
            if (!_drawingObjectPerEntity.ContainsKey(entity))
            { 
                // in case no method to create a drawing object for the entity is provided an
                // exception is thrown
                if(!EntityVisualizationObjectCreatingMethodsPerType.ContainsKey(entity.GetType()))
                    throw new InvalidOperationException(String.Format("Creation of Drawing Object for Entity {0} is not provided", entity.ToString()));

                // create the drawing object
                DrawingObject newDrawingObject = EntityVisualizationObjectCreatingMethodsPerType[entity.GetType()](entity);

                // add drawing obecjt to system, has to be set to visual manually by user
                DrawingSystem.AddObject(newDrawingObject);

                // drawing object is stored in the dictionary
                _drawingObjectPerEntity.Add(entity, newDrawingObject);
                
            } // end if 

            // the right key is now in the dictionary
            return _drawingObjectPerEntity[entity];
        } // end of DrawingObjectPerEntity
        
        #endregion

        #region RemoveDrawingObjectPerEntity

        /// <summary>
        /// Removes the drawing object for an entity
        /// </summary>
        /// <param name="entity">Entity for which an object should be removed</param>
        public void RemoveDrawingObjectPerEntity(Entity entity)
        {

            if (_drawingObjectPerEntity.ContainsKey(entity))
                DrawingSystem.RemoveObject(_drawingObjectPerEntity[entity]);

            _drawingObjectPerEntity.Remove(entity);
        } // end of RemoveDrawingObjectPerEntity

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region VisualizeDynamicModel

        /// <summary>
        /// Creates a visualization of a control unit at a given time. For all events, activities and holding entities it
        /// is checked if methods have been stored in corresponding dictionaries, and if so they are triggered.
        /// </summary>
        /// <param name="currentTime">Time at which model should be visualized</param>
        /// <param name="simModel">Model to visualize</param>
        /// <param name="parentControlUnit">Control unit to visualize</param>
        /// <param name="currentEvents">Currently triggered events in the model</param>
        public void VisualizeDynamicModel(DateTime currentTime, SimulationModel simModel, ControlUnit parentControlUnit, IEnumerable<Event> currentEvents)
        {
            #region VisualizationChangesFromEvents

            // visualization methods for all events are called (if provided)
            foreach (EventActivity ev in currentEvents.Where(p=> p is EventActivity && ((EventActivity)p).EventType == EventType.End).Cast<EventActivity>())
            {
                Activity parentActivity = ((EventActivity)ev).ParentActivity;

                if (parentActivity.StartTime == parentActivity.EndTime)
                    continue;

                // if a handling method for the end event is specified it is called
                if (ActivityEndEventVisualizationMethods.ContainsKey(parentActivity.GetType()))
                    ActivityEndEventVisualizationMethods[parentActivity.GetType()](parentActivity, currentTime);
            } // end foreach

            // visualization methods for all events are called (if provided)
            foreach (EventActivity ev in currentEvents.Where(p=> p is EventActivity && ((EventActivity)p).EventType == EventType.Start).Cast<EventActivity>())
            {
                Activity parentActivity = ((EventActivity)ev).ParentActivity;

                if (parentActivity.StartTime == parentActivity.EndTime)
                    continue;

                // if a handling method for the start event is specified it is called
                if (ActivityStartEventVisualizationMethods.ContainsKey(parentActivity.GetType()))
                    ActivityStartEventVisualizationMethods[parentActivity.GetType()](parentActivity, currentTime);
            } // end foreach

            // visualization methods for all events are called (if provided)
            foreach (Event ev in currentEvents.Where(p => p.EventType == EventType.Standalone))
            {
                // if a handling method for a standalone event is specified it is called
                if (EventStandaloneDrawingMethods.ContainsKey(ev.GetType()))
                    EventStandaloneDrawingMethods[ev.GetType()](ev);
            } // end foreach

            #endregion

            #region VisualizationDuringActivities

            foreach (Activity activity in parentControlUnit.CurrentActivities)
            {
                // intermediate visualization is only done for times strictly larger
                // than the start time
                if (activity.StartTime == currentTime)
                    continue;

                // if a method has been specified for an activity type it is called
                if (ActivityIntermediateVisualizationMethods.ContainsKey(activity.GetType()))
                    ActivityIntermediateVisualizationMethods[activity.GetType()](activity, currentTime);

            } // end foreach

            #endregion

            #region VisualizeDynamicHoldingEntities

            foreach (IDynamicHoldingEntity holdingEntity in parentControlUnit.AllControlledEntities.Where(p => p is IDynamicHoldingEntity).Cast<IDynamicHoldingEntity>())
            {
                if (HoldingEntitiesVisualizationMethods.ContainsKey(holdingEntity.GetType()))
                    HoldingEntitiesVisualizationMethods[holdingEntity.GetType()](holdingEntity);
            } // end foreach

            #endregion

            AdditionalDynamicVisualization(currentTime, simModel, parentControlUnit, currentEvents);

        } // end of VisualizeDynamicModel

        #endregion

        #region IntializeVisualizationAtTime

        /// <summary>
        /// Initilializes the visualization for the control unit, can be at start of simulation (e.g. static visualization), at re-enabling
        /// visualization during a visualization run or re-runs after completion. If the model is running the dynamic visualization is updated.
        /// </summary>
        /// <param name="initializeTime">The time the model is initialized</param>
        /// <param name="simulationModel">The model at the initilization time</param>
        /// <param name="parentControlUnit">The control unit at the initilization time</param>
        public void IntializeVisualizationAtTime(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            _drawingObjectPerEntity = new Dictionary<Entity, DrawingObject>();

            List<Event> actitivityStartEvents = parentControlUnit.CurrentActivities.Select(p => p.StartEvent).Cast<Event>().ToList();

            AdditionalStaticVisualization(initializationTime, simModel, parentControlUnit);

            VisualizeDynamicModel(initializationTime, simModel, parentControlUnit, actitivityStartEvents);

        } // end of IntializeVisualizationAtTime

        #endregion

        #region AdditionalDynamicVisualization

        /// <summary>
        /// Additional dynamic visualization, to be overwritten
        /// </summary>
        /// <param name="currentTime">Time the model is visualized</param>
        public virtual void AdditionalDynamicVisualization(DateTime currentTime, SimulationModel simModel, ControlUnit parentControlUnit, IEnumerable<Event> currentEvents)
        {
        } // end of AdditionalDynamicVisualization

        #endregion

        #region AdditionalStaticVisualization

        /// <summary>
        /// Creates additional static visualization, to be overwritten
        /// </summary>
        /// <param name="initializationTime">Time at which static visualization is generated</param>
        /// <param name="simModel">Simulation model for which the visuslization is generated</param>
        /// <param name="parentControlUnit">Control unit that is visualized</param>
        public virtual void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
        } // end of AdditionalStaticVisualization

        #endregion

    } // end of
}

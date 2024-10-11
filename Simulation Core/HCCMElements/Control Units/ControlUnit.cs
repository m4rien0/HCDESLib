using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Base class of control untis. Provides basic functionality of conrol units and
    /// deals with adding/removing of entities and requests. Also manages currently controlled
    /// entities and activities
    /// </summary>
    public abstract class ControlUnit
    {
        #region Constructor

        /// <summary>
        /// Basic constructor. Initializes members of control unit
        /// </summary>
        /// <param name="name">String representer of control unit</param>
        /// <param name="parentControlUnit">If there exists a parent control unit (in case it is not the root control unit), it should be passed here</param>
        public ControlUnit(string name,
                           ControlUnit parentControlUnit,
                           SimulationModel parentSimulationModel)
        {
            _name = name;
            _parentControlUnit = parentControlUnit;
            _parentSimulationModel = parentSimulationModel;
            _rael = new List<ActivityRequest>();
            _currentActivitesPerType = new Dictionary<string, List<Activity>>();
            _childControlUnits = new ControlUnit[] { };
            _delegateInbox = new List<IDelegate>();
            _delegateOutBox = new List<IDelegate>();
            _currentActivities = new List<Activity>();
            _delegateHandlingMethods = new Dictionary<Type, HandleDelegateOfType>();

            _controlledEntities = new Dictionary<Type, HashSet<Entity>>();
            _allControlledEntities = new List<Entity>();

            _stateData = new Dictionary<DateTime, object>();
        } // end of ControlUnit

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // General Members
        //--------------------------------------------------------------------------------------------------

        #region Name

        private string _name;

        public string Name
        {
            get
            {
                return _name;
            }
        } // end of Name

        #endregion Name

        #region ParentSimulationModel

        private SimulationModel _parentSimulationModel;

        public SimulationModel ParentSimulationModel
        {
            get
            {
                return _parentSimulationModel;
            }
        } // end of ParentSimulationModel

        #endregion ParentSimulationModel

        //--------------------------------------------------------------------------------------------------
        // General Methods
        //--------------------------------------------------------------------------------------------------

        #region CustomInitialize

        /// <summary>
        /// Should be overwritten if a custom initialization is required.
        /// </summary>
        /// <param name="startTime">Start time of the model</param>
        /// <param name="simEngine">SimEngine handling the simulation execution</param>
        protected virtual void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
        } // end of CustomInitialize

        #endregion CustomInitialize

        #region Initialize

        /// <summary>
        /// Initializes the control unit, this may happen after creation of object, e.g. when start and end times
        /// of the model are adjusted after instanciation. Calls a custom initialize that can be overwritten by
        /// the user and initializes sub trees
        /// </summary>
        /// <param name="startTime">Time the simulation starts</param>
        /// <param name="simEngine">SimEngine that handles the simulation run</param>
        public virtual void Initialize(DateTime startTime, ISimulationEngine simEngine)
        {
            CustomInitialize(startTime, simEngine);

            foreach (ControlUnit childControl in ChildControlUnits)
            {
                childControl.Initialize(startTime, simEngine);
            } // end foreach
        } // end of InitializeActiveEntities

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Connection to rest of model
        //--------------------------------------------------------------------------------------------------

        #region ParentControlUnit

        protected ControlUnit _parentControlUnit;

        public ControlUnit ParentControlUnit
        {
            get
            {
                return _parentControlUnit;
            }
        } // end of ParentControlUnit

        #endregion ParentControlUnit

        #region ChildControlUnits

        private ControlUnit[] _childControlUnits;

        public ControlUnit[] ChildControlUnits
        {
            get
            {
                return _childControlUnits;
            }
        } // end of ChildControlUnits

        #endregion ChildControlUnits

        #region SetChildControlUnits

        /// <summary>
        /// Sets the child control units of the control unit. These may not be available upon instanciation of the
        /// control unit, hence there needs to be a possiblity to set them after the constructor call
        /// </summary>
        /// <param name="childs">Child control units of the current control unit</param>
        public void SetChildControlUnits(ControlUnit[] childs)
        {
            _childControlUnits = childs;
        } // end of SetChildControlUnits

        #endregion SetChildControlUnits

        #region FindSmallestJointControl

        /// <summary>
        /// Helping function that returns the lowest joint control unit in the control tree that contains
        /// both control units in its sub-tree.
        /// </summary>
        /// <param name="otherControl"> the other control unit that the smallest joint control is used for</param>
        /// <returns>Control unit that is lowest in the control tree and has both control units in its subtree,
        ///          returns null if no joint control has been found</returns>
        public ControlUnit FindSmallestJointControl(ControlUnit otherControl)
        {
            if (ChildControlUnits.Contains(otherControl))
                return this;
            else
            {
                if (ParentControlUnit == null)
                    return null;

                return ParentControlUnit.FindSmallestJointControl(otherControl);
            } // end if
        } // end of FindSmallestJointControl

        #endregion FindSmallestJointControl

        //--------------------------------------------------------------------------------------------------
        // Request Handling
        //--------------------------------------------------------------------------------------------------

        #region RAEL

        protected List<ActivityRequest> _rael;

        /// <summary>
        /// List of all requests currently filed at control unit.
        /// </summary>
        public List<ActivityRequest> RAEL
        {
            get
            {
                return _rael;
            }
        } // end of RAEL

        #endregion RAEL

        #region AddRequest

        /// <summary>
        /// Adds a request to the RAEL list. Sets also behavior occured flag of the control unit to true, means rule execution is required.
        /// </summary>
        /// <param name="req">Request to add</param>
        public virtual void AddRequest(ActivityRequest req)
        {
            _rael.Add(req);
            _behaviorOccured = true;
        } // end of AddRequest

        #endregion AddRequest

        #region RemoveRequest

        /// <summary>
        /// Removes a request from the RAEL list. Sets also behavior occured flag of the control unit to true, means rule execution is required.
        /// </summary>
        /// <param name="req">Request to remove</param>
        public virtual void RemoveRequest(ActivityRequest req)
        {
            _rael.Remove(req);
            _behaviorOccured = true;
        } // end of RemoveRequest

        #endregion RemoveRequest

        #region ClearRAEL

        /// <summary>
        /// Clears the whole RAEL List
        /// </summary>
        public virtual void ClearRAEL()
        {
            _rael.Clear();
            _behaviorOccured = true;
        } // end of ClearRAEL

        #endregion ClearRAEL

        //--------------------------------------------------------------------------------------------------
        // Own State
        //--------------------------------------------------------------------------------------------------

        #region StateData

        private Dictionary<DateTime, object> _stateData;

        /// <summary>
        /// Holds any sort of state data that might be collected after rule execution for output generation, e.g. the length of RAEL list
        /// </summary>
        public Dictionary<DateTime, object> StateData
        {
            get
            {
                return _stateData;
            }
            set
            {
                _stateData = value;
            }
        } // end of StateData

        #endregion StateData

        #region AllControlledEntities

        private List<Entity> _allControlledEntities;

        public IReadOnlyList<Entity> AllControlledEntities
        {
            get
            {
                return _allControlledEntities;
            }
        } // end of AllControlledEntities

        #endregion AllControlledEntities

        #region ControlledEntities

        private Dictionary<Type, HashSet<Entity>> _controlledEntities;

        /// <summary>
        /// A dictionary that holds sets of entities per entitiy types that are controlled at a point in time by.
        /// </summary>
        public IReadOnlyDictionary<Type, HashSet<Entity>> ControlledEntities
        {
            get
            {
                return _controlledEntities;
            }
        } // end of ControlledDefaultEntities

        #endregion ControlledEntities



        #region GetEntitiesOfType

        /// <summary>
        /// Returns all controlled entities of a certain type.
        /// </summary>
        /// <param name="type">Type of entities that should be returned</param>
        /// <returns>List of all entities of passed type</returns>
        public List<Entity> GetEntitiesOfType(Type type)
        {
            if (!ControlledEntities.ContainsKey(type))
                throw new InvalidOperationException("Entity Type not controlled by Control Unit");

            return ControlledEntities[type].ToList();
        } // end of GetDefaultEntitiesOfType

        #endregion GetEntitiesOfType

        #region CurrentActivitiesPerType

        private Dictionary<string, List<Activity>> _currentActivitesPerType;

        /// <summary>
        /// All activities of a certain type (represented by string)
        /// </summary>
        public Dictionary<string, List<Activity>> CurrentActivitiesPerType
        {
            get
            {
                return _currentActivitesPerType;
            }
        } // end of CurrentActivities

        #endregion CurrentActivitiesPerType

        #region CurrentActivities

        private List<Activity> _currentActivities;

        /// <summary>
        /// All activities that are currently performed
        /// </summary>
        public List<Activity> CurrentActivities
        {
            get
            {
                return _currentActivities;
            }
        } // end of CurrentActivities

        #endregion CurrentActivities

        //--------------------------------------------------------------------------------------------------
        // Change State
        //--------------------------------------------------------------------------------------------------

        #region AddEntity

        /// <summary>
        /// Entitiy is added to the control unit, entity is automatically added to the controlled entities
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public virtual void AddEntity(Entity entity)
        {
            // if entity type is not controlled yet, type is added
            if (!_controlledEntities.ContainsKey(entity.GetType()))
            {
                _controlledEntities.Add(entity.GetType(), new HashSet<Entity>());
            } // end if

            // entity is added to controlled entities and parent control unit is set
            _controlledEntities[entity.GetType()].Add(entity);
            _allControlledEntities.Add(entity);
            entity.ParentControlUnit = this;

            _behaviorOccured = true;
        } // end of AddEntity

        #endregion AddEntity

        #region AddEntities

        /// <summary>
        /// Adds a range of entities, adding to corresponding holding member is done automatically.
        /// </summary>
        /// <param name="entities">Entities to add</param>
        public void AddEntities(IEnumerable<Entity> entities)
        {
            foreach (Entity entity in entities)
            {
                AddEntity(entity);
            } // end foreach

            _behaviorOccured = true;
        } // end AddEntities

        #endregion AddEntities

        #region RemoveEntity

        /// <summary>
        /// Removes entities from the controll units and all corresponding members
        /// </summary>
        /// <param name="entity"></param>
        public virtual void RemoveEntity(Entity entity)
        {
            if (!_controlledEntities.ContainsKey(entity.GetType()))
            {
                throw new InvalidOperationException("Control Unit does not control Entity");
            } // end if
            else
            {
                _controlledEntities[entity.GetType()].Remove(entity);
                entity.ParentControlUnit = null;
            } // end if

            _allControlledEntities.Remove(entity);
        } // end of RemoveEntity

        #endregion RemoveEntity

        #region AddActivity

        /// <summary>
        /// Adds an activity to current activities of control unit.
        /// </summary>
        /// <param name="activity">Activity to add</param>
        public void AddActivity(Activity activity)
        {
            if (!_currentActivitesPerType.ContainsKey(activity.GetType().Name))
            {
                _currentActivitesPerType.Add(activity.GetType().Name, new List<Activity>());
            } // end if

            _currentActivitesPerType[activity.GetType().Name].Add(activity);
            _currentActivities.Add(activity);
        } // end of AddActivity

        #endregion AddActivity

        #region RemoveActivity

        /// <summary>
        /// Removes an activity of the control unit.
        /// </summary>
        /// <param name="activity">Activity to remove</param>
        public void RemoveActivity(Activity activity)
        {
            if (_currentActivitesPerType.ContainsKey(activity.GetType().Name))
            {
                _currentActivitesPerType[activity.GetType().Name].Remove(activity);
            } // end if

            _currentActivities.Remove(activity);
        } // end of RemoveActivity

        #endregion RemoveActivity

        //--------------------------------------------------------------------------------------------------
        // Rule Sets
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Abstract method to be overwritten by the user that implements the modeled rule set of control unit, e.g.
        /// triggering of controlled behavior or request handling.
        /// </summary>
        /// <param name="time">Time the rule set is performed</param>
        /// <param name="simEngine">SimEngine that handles the simulation execution</param>
        /// <returns>Should return true if re-evaluation is required</returns>
        protected abstract bool PerformCustomRules(DateTime time, ISimulationEngine simEngine);

        #endregion PerformCustomRules

        #region PerformRules

        /// <summary>
        /// Rule set of control unit. Calls Custom rule sets, child control units rules and delegate handling
        /// until no further evaluation is required.
        /// </summary>
        /// <param name="time">Time of rule execution</param>
        /// <param name="eventLaunched">Out parameter that indicates if new behavior has occured and re-evaluation has to be performed.</param>
        /// <param name="simEngine"> SimEngine that handles the simulation execution</param>
        public void PerformRules(DateTime time, out bool eventLaunched, ISimulationEngine simEngine)
        {
            eventLaunched = false;

            if (BehaviorOccured == false)
            {
                // Perform rules of all child control units
                foreach (ControlUnit childUnit in ChildControlUnits)
                {
                    bool childLaunched = false;
                    childUnit.PerformRules(time, out childLaunched, simEngine);
                    eventLaunched = eventLaunched || childLaunched;
                } // end foreach

                return;
            } // end if

            // Perform differentRule sets of current control unit
            eventLaunched = PerformCustomRules(time, simEngine);

            bool delegateSent = true;

            List<IDelegate> handledDelegates = HandleReceivedDelegates(time, simEngine);

            if (handledDelegates.Count > 0)
            {
                eventLaunched = true;
                RemoveHandledDelegatesFromInbox(handledDelegates);
            } // end if

            delegateSent = SendAllDelegates(time, simEngine);
            eventLaunched = eventLaunched || delegateSent;

            if (ChildControlUnits == null)
                return;

            // Perform rules of all child control units
            foreach (ControlUnit childUnit in ChildControlUnits)
            {
                bool childLaunched = false;
                childUnit.PerformRules(time, out childLaunched, simEngine);
                eventLaunched = eventLaunched || childLaunched;
            } // end foreach
        } // end of PerformRules

        #endregion PerformRules

        #region BehaviorOccured

        private bool _behaviorOccured = false;

        /// <summary>
        /// Indicates if some sort of beavior (add/remove of requests, delegate or triggering of events) has occured.
        /// </summary>
        public virtual bool BehaviorOccured
        {
            get
            {
                return _behaviorOccured;
            }
            set
            {
                _behaviorOccured = value;
            }
        } // end of NeedsUpdate

        #endregion BehaviorOccured

        #region SetRecursiveNeedsUpdateToFalse

        /// <summary>
        /// Method that sets all behavior occured flags of all control units to false, recursively calls itself on child
        /// controls.
        /// </summary>
        public void SetRecursiveNeedsUpdateToFalse()
        {
            BehaviorOccured = false;

            foreach (ControlUnit childControl in ChildControlUnits)
            {
                childControl.SetRecursiveNeedsUpdateToFalse();
            } // end foreach
        } // end of SetRecursiveNeedsUpdateToFalse

        #endregion SetRecursiveNeedsUpdateToFalse

        //--------------------------------------------------------------------------------------------------
        // Delegates
        //--------------------------------------------------------------------------------------------------

        #region DelegateInbox

        /// <summary>
        /// Inbox of all delegates sent to control unit
        /// </summary>
        private List<IDelegate> _delegateInbox;

        #endregion DelegateInbox



        #region RemoveHandledDelegatesFromInbox

        /// <summary>
        /// Removes all handled delegates from inbox
        /// </summary>
        /// <param name="handledDelegates">All delegates that have been handled and need not to be kept.</param>
        private void RemoveHandledDelegatesFromInbox(IEnumerable<IDelegate> handledDelegates)
        {
            foreach (IDelegate del in handledDelegates)
            {
                _delegateInbox.Remove(del);
            } // end foreach
        } // end of RemoveHandledDelegatesFromInbox

        #endregion RemoveHandledDelegatesFromInbox

        #region DelegateOutBox

        private List<IDelegate> _delegateOutBox;

        /// <summary>
        /// Outbox of delegates to be sent after rule execution
        /// </summary>
        public List<IDelegate> DelegateOutBox
        {
            get
            {
                return _delegateOutBox;
            }
        } // end of DelegateOutBox

        #endregion DelegateOutBox

        #region SendDelegates

        /// <summary>
        /// Sends all delegates in the outbox to their destination control unit.
        /// </summary>
        /// <param name="time">Time of sending.</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <returns></returns>
        public virtual bool SendAllDelegates(DateTime time, ISimulationEngine simEngine)
        {
            bool delSent = false;
            foreach (IDelegate del in DelegateOutBox)
            {
                SendDelegateTo(ParentControlUnit, del);
                delSent = true;
            } // end foreach

            DelegateOutBox.Clear();

            return delSent;
        } // end of SendDelegates

        #endregion SendDelegates

        #region HandleReceivedDelegates

        /// <summary>
        /// Handles all delegates that have been received by attempting to call the corresponding mehtods (per type of delegae) that
        /// can be added by the user to DelegateHandlingMethods in the constructor of the control unit.
        /// </summary>
        /// <param name="time">Time of handling</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <returns>Returns a list of all delegates that have been handled and can be removed.</returns>
        public virtual List<IDelegate> HandleReceivedDelegates(DateTime time, ISimulationEngine simEngine)
        {
            List<IDelegate> handledDelegates = new List<IDelegate>();

            foreach (IDelegate del in _delegateInbox)
            {
                if (DelegateHandlingMethods.ContainsKey(del.GetType()))
                {
                    if (DelegateHandlingMethods[del.GetType()](del, this, time, simEngine))
                        handledDelegates.Add(del);
                }
                else
                {
                    throw new InvalidOperationException("No handling method for delegate of type" + del.GetType().ToString() + "found!");
                } // end if
            } // end foreach

            return handledDelegates;
        } // end of HandleReceiveDelegates

        #endregion HandleReceivedDelegates

        #region SendDeleateTo

        /// <summary>
        /// Sends a single delegate to a destinition control unit
        /// </summary>
        /// <param name="destination">Destinatin control unit</param>
        /// <param name="del">Delegate to send.</param>
        public void SendDelegateTo(ControlUnit destination, IDelegate del)
        {
            BehaviorOccured = true;
            destination.ReceiveDelegate(del);
        } // end of SendDelegateTo

        #endregion SendDeleateTo

        #region ReceiveDelegate

        /// <summary>
        /// Receives a sent delegate and adds it to the inbox.
        /// </summary>
        /// <param name="del">Delegate that has been sent</param>
        private void ReceiveDelegate(IDelegate del)
        {
            _delegateInbox.Add(del);
            BehaviorOccured = true;
        } // end of ReceiveDelegate

        #endregion ReceiveDelegate

        #region DelegateHandlingMethods

        /// <summary>
        /// This Delegate (C#) represents handling methods for HCCM-delegates (bad naming). Different Methods of the type
        /// specified here can be added to the DelegateHandlingMethods dictionary to hanlde different types of (HCCM) delegates.
        /// </summary>
        /// <param name="del">Delegate to hanlde</param>
        /// <param name="controlUnit">Control unit where delegate is handled</param>
        /// <param name="time">Time</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <returns>Should return true if delegate needs no further handling and can be removed.</returns>
        public delegate bool HandleDelegateOfType(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine);

        protected Dictionary<Type, HandleDelegateOfType> _delegateHandlingMethods;

        /// <summary>
        /// Holds delegate handling methods per (HCCCM) delegate types
        /// </summary>
        public Dictionary<Type, HandleDelegateOfType> DelegateHandlingMethods
        {
            get
            {
                return _delegateHandlingMethods;
            }
        } // end of DelegateHandlingMethods

        #endregion DelegateHandlingMethods

        //--------------------------------------------------------------------------------------------------
        // Enter and Leave of Entities
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        /// <summary>
        /// Abstract method that can be overwritten to specify the event that should be triggered upont the
        /// entering of an entity to the control unit. Can be helpful if e.g. entities are sent between control units
        /// with a moving activitity and the end of the move has not to be concerned with what happens with the entity
        /// after the mode
        /// </summary>
        /// <param name="time">Time entity enters</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <param name="entity">Entity that enters</param>
        /// <param name="originDelegate">The delegate that corresponds with the entering, can be null</param>
        /// <returns></returns>
        public abstract Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate);

        #endregion EntityEnterControlUnit

        #region EntityLeaveControlUnit

        /// <summary>
        /// Same principle as Entering entities.
        /// </summary>
        /// <param name="time">Time entity leaves</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <param name="entity">Entity that leaves</param>
        /// <param name="originDelegate">Delegate of sending entity</param>
        public abstract void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate);

        #endregion EntityLeaveControlUnit

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return Name;
        } // end of ToString

        #endregion ToString
    } // end of ControlUnit
}
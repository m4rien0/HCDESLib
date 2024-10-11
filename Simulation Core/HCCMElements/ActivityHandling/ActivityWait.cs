using SimulationCore.SimulationClasses;
using System;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Waiting actitivities are often used to model idle states in a simulation.
    /// This class iplements a basic waiting activitiy of an entity
    /// </summary>
    public class ActivityWait : Activity
    {
        #region Constructor

        /// <summary>
        /// Constructor that sets corresponding members of class
        /// </summary>
        /// <param name="parentControlUnit"> Parent control of waiting activity</param>
        /// <param name="affectedEntity">Entity that is waiting</param>
        /// <param name="waitingArea">Optional parameter for a holding entity such as waiting rooms, or storage rooms</param>
        public ActivityWait(ControlUnit parentControlUnit,
            Entity affectedEntity,
            IDynamicHoldingEntity waitingArea = null)
            : base(parentControlUnit, "ActivityWait", true)
        {
            _waitingEntity = affectedEntity;
            _waitingArea = waitingArea;
        } // end of Activity

        #endregion Constructor

        #region Name

        public static string Name = "ActivityWait";

        #endregion Name

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region WaitingArea

        private IDynamicHoldingEntity _waitingArea;

        public IDynamicHoldingEntity WaitingArea
        {
            get
            {
                return _waitingArea;
            }
        } // end of WaitingArea

        #endregion WaitingArea

        #region WaitingEntity

        private Entity _waitingEntity;

        public Entity WaitingEntity
        {
            get
            {
                return _waitingEntity;
            }
        } // end of WaitingEntity

        #endregion WaitingEntity

        #region AffectedEntites

        public override Entity[] AffectedEntities
        {
            get
            {
                return WaitingEntity.ToArray();
            }
        } // end of AffectedEntities

        #endregion AffectedEntites

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// Overrides the state change at start. If an holding entity was passed as parameter
        /// the waiting entity is added
        /// </summary>
        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            if (WaitingArea != null)
                WaitingArea.HoldedEntities.Add(WaitingEntity);
        } // end of TriggerStartEvent

        #endregion TriggerStartEvent

        #region TriggerEndEvent

        /// <summary>
        /// Overrides the state change at end. If an holding entity was passed as parameter
        /// the waiting entity is removed
        /// </summary>
        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            if (WaitingArea != null)
                WaitingArea.HoldedEntities.Remove(WaitingEntity);
        } // end of TriggerEndEvent

        #endregion TriggerEndEvent

        #region ToString

        /// <summary>
        /// Creates string of activity name and representation of the waiting entity
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name + ": " + WaitingEntity.ToString();
        } // end of ToString

        #endregion ToString

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Clone

        public override Activity Clone()
        {
            return new ActivityWait(ParentControlUnit, WaitingEntity.Clone());
        } // end of Clone

        #endregion Clone
    }
}
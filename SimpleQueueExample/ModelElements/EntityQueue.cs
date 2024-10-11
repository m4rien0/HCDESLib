using SimulationCore.HCCMElements;
using System.Collections.Generic;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Entity representing a queue, implements holding entity as multiple clients may be holded
    /// </summary>
    public class EntityQueue : Entity, IDynamicHoldingEntity
    {
        public static int RunningID = 0;

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        public EntityQueue()
            : base(RunningID++)
        {
            _holdedEntities = new List<Entity>();
        } // end of EntityClient

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region HoldedEntities

        private List<Entity> _holdedEntities;

        /// <summary>
        /// Implementing interface of holding entity
        /// </summary>
        public List<Entity> HoldedEntities
        {
            get
            {
                return _holdedEntities;
            }
            set
            {
                _holdedEntities = value;
            }
        } // end of HoldedEntities

        #endregion HoldedEntities

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Queue: " + Identifier.ToString();
        } // end of

        #endregion ToString

        #region Clone

        public override Entity Clone()
        {
            return new EntityQueue();
        } // end of Clone

        #endregion Clone
    } // end of EntityQueue
}
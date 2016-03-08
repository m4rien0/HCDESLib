using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        #endregion

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

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Queue: " + Identifier.ToString();
        } // end of

        #endregion

        #region Clone

        public override Entity Clone()
        {
            return new EntityQueue();

        } // end of Clone

        #endregion

    } // end of EntityQueue
}

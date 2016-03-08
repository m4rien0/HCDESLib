using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Entity for a waiting area, inheriting from dynamic holding entity
    /// </summary>
    public class EntityWaitingArea : Entity, IDynamicHoldingEntity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="identifier">Entity identifier</param>
        /// <param name="name">String representation of waiting area</param>
        /// <param name="position">Position within structural layout of parent structural area</param>
        /// <param name="size">Size within structural layout of parent structural area</param>
        public EntityWaitingArea(int identifier, string name, Point position, Size size)
            :base(identifier)
        {
            _position = position;
            _size = size;
            _name = name;
            _waitingEntities = new List<Entity>();
        } // end of EntitiyWaitingArea

        #endregion

        #region Name

        private string _name;

        /// <summary>
        /// String representation of waiting area
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        } // end of Name

        #endregion

        #region WaitingEntities

        private List<Entity> _waitingEntities;

        /// <summary>
        /// List of entities waiting in the area
        /// </summary>
        public List<Entity> HoldedEntities
        {
            get
            {
                return _waitingEntities;
            }
            set
            {
                _waitingEntities = value;
            }
        } // end of WaitingEntities

        #endregion

        #region ToString

        public override string ToString()
        {
            return Name;
        } // end of ToString

        #endregion

        #region Clone

        public override Entity Clone()
        {
            return new EntityWaitingArea(Identifier, Name, Position, Size);
        } // end of Clone

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Drawing Methods and Properties 
        //--------------------------------------------------------------------------------------------------

        #region Position

        private Point _position;

        /// <summary>
        /// Position within structural layout of parent structural area
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
        } // end of Position

        #endregion

        #region Size

        private Size _size;

        /// <summary>
        /// Size within structural layout of parent structural area
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }
        } // end of Size

        #endregion

        #region ParentControlUnit

        /// <summary>
        /// This changes the parent control unit property as a waiting area does not change
        /// its parent control by activities
        /// </summary>
        public override ControlUnit ParentControlUnit
        {
            get
            {
                return base.ParentControlUnit;
            }
            set
            {
                if (base.ParentControlUnit == null)
                    base.ParentControlUnit = value;
            }
        } // end of ParentControlUnit

        #endregion

    } // end of EntitiyWaitingRoom
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Base class for all entities in simulation models.
    /// </summary>
    abstract public class Entity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="identifier">ID of entity </param>
        public Entity(int identifier)
        {
            _identifier = identifier;

            _dataEntries = new Dictionary<string, object>();

            _addedToTracker = false;
        } // end of Entity

        #endregion

        #region AddedToTracker

        private bool _addedToTracker;

        /// <summary>
        /// Flag if the entity has already been added to the entityTracker of the parent simulation model.
        /// </summary>
        public bool AddedToTracker
        {
            get
            {
                return _addedToTracker;
            }
        } // end of AddedToTracker

        #endregion

        #region ParentControlUnit

        private ControlUnit _parentControlUnit;

        /// <summary>
        /// Parent control unit of the entity, this is not set via the constructor, but upon adding of the entity
        /// to a control unit
        /// </summary>
        public virtual ControlUnit ParentControlUnit
        {
            get
            {
                return _parentControlUnit;
            }
            set
            {
                _parentControlUnit = value;

                if (!AddedToTracker)
                {
                    ParentControlUnit.ParentSimulationModel.AddEntityToTracker(this);
                    _addedToTracker = true;
                } // end if
            }
        } // end of ParentControlUnit

        #endregion

        #region Identifier

        private int _identifier;

        /// <summary>
        /// Int identifier of the entity
        /// </summary>
        public int Identifier
        {
            get
            {
                return _identifier;
            }
        } // end of Identifier

        #endregion

        #region DataEntries

        private Dictionary<string,object> _dataEntries;

        /// <summary>
        /// Data entries for output generation my be stored here.
        /// </summary>
        public Dictionary<string,object> DataEntries
        {
            get
            {
                return _dataEntries;
            }
            set
            {
                _dataEntries = value;
            }
        } // end of DataEntries

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToArray

        public Entity[] ToArray()
        {
            return new Entity[] { this };
        } // end of ToArray

        #endregion

        #region ToString

        public override abstract string ToString();

        #endregion

        #region Clone

        public abstract Entity Clone();

        #endregion

    } // end of Entity
}

using System;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Base class for staff entities. Includes some functionality that is typical for staff entities.
    /// </summary>
    public abstract class EntityStaff : ActiveEntityWithSkill
    {
        #region Constructor

        /// <summary>
        /// Basic constructor with identifiers and skill set.
        /// </summary>
        /// <param name="identifier">Identifier of entity</param>
        /// <param name="skillSet"></param>
        public EntityStaff(int identifier, SkillSet skillSet)
            : base(identifier, skillSet)
        {
            _onHold = false;
            _blockedForDispatching = false;
            _isIdle = true;
            _isAbsent = false;
            _busyFactor = 0;
        } // end of EntityStaff

        #endregion Constructor

        #region IsAbsent

        private bool _isAbsent;

        /// <summary>
        /// Flag that might be set if entity is absent (e.g. during a break)
        /// </summary>
        public bool IsAbsent
        {
            get
            {
                return _isAbsent;
            }
        } // end of IsAbsent

        #endregion IsAbsent

        #region BaseControlUnit

        private ControlUnit _baseControlUnit;

        /// <summary>
        /// Base control unit of entity, can be helpful if enties are sent between control units.
        /// </summary>
        public ControlUnit BaseControlUnit
        {
            get
            {
                return _baseControlUnit;
            }
            set
            {
                _baseControlUnit = value;
            }
        } // end of BaseControlUnit

        #endregion BaseControlUnit



        #region OnHold

        private bool _onHold;

        /// <summary>
        /// On hold status describes the waiting for a certain activity to start, may include waiting for
        /// a sequential activities
        /// </summary>
        public bool OnHold
        {
            get
            {
                return _onHold;
            }
            set
            {
                _onHold = value;
            }
        } // end of OnHold

        #endregion OnHold

        #region BlockedForDispatching

        private bool _blockedForDispatching;

        /// <summary>
        /// Flag to enable the blocking of entities for dispatching.
        /// </summary>
        public bool BlockedForDispatching
        {
            get
            {
                return _blockedForDispatching;
            }
            set
            {
                _blockedForDispatching = value;
            }
        } // end of BlockedForDispatching

        #endregion BlockedForDispatching

        #region IsIdle

        private bool _isIdle;

        /// <summary>
        /// Flag for entities that are idle, may also occur during moving activities
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return _isIdle;
            }
            set
            {
                _isIdle = value;
            }
        } // end of IsIdle

        #endregion IsIdle

        #region BusyFactor

        private double _busyFactor;

        /// <summary>
        /// THe busy factor of an entity describes the percentage work load. Hence, it must be
        /// between 0 and 1.
        /// </summary>
        public double BusyFactor
        {
            get
            {
                return _busyFactor;
            }
            set
            {
                if (value > 1 || value < 0)
                    throw new InvalidOperationException("BusyFactor must be between 0 and 1");
                else
                    _busyFactor = value;
            }
        } // end of BusyFactor

        #endregion BusyFactor
    } // end of EntityStaff
}
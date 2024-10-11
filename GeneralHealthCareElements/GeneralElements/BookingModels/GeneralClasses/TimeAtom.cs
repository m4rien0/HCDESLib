using SimulationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Class for the smallest time interval considered in a booking model
    /// </summary>
    public class TimeAtom
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentTimeLine"></param>
        /// <param name="config"></param>
        /// <param name="date"></param>
        public TimeAtom(SinglePerDayTimeLine parentTimeLine,
            TimeAtomConfig config,
            DateTime date)
        {
            _parentTimeLine = parentTimeLine;
            _startTime = date.Date + TimeSpan.FromHours(config.StartTime);
            _endTime = date.Date + TimeSpan.FromHours(config.EndTime);
            _slotsAtTime = new List<Slot>();
            _blocked = config.Blocked;
            _maxCapacity = config.MaxCapacity;
            _nonBookable = config.NonStartBookable;
        } // end of TimeAtom

        #endregion Constructor

        #region InitializePreviousAtom

        /// <summary>
        /// Method to intialize the pervious atom for the creation of the atom chain
        /// </summary>
        /// <param name="previousAtom"></param>
        public void InitializePreviousAtom(TimeAtom previousAtom)
        {
            _previousAtom = previousAtom;

            if (previousAtom != null)
            {
                if (previousAtom.Blocked)
                {
                    _previousBlockedAtom = previousAtom;
                    _previousAvailableAtom = previousAtom._previousAvailableAtom;
                }
                else
                {
                    _previousAvailableAtom = previousAtom;
                    _previousBlockedAtom = previousAtom._previousBlockedAtom;
                } // end if
            } // end if
        } // end of InitializeChain

        #endregion InitializePreviousAtom

        #region InitializeNextAtom

        /// <summary>
        /// Method to intialize the pervious atom for the creation of the atom chain
        /// </summary>
        /// <param name="nextAtom"></param>
        public void InitializeNextAtom(TimeAtom nextAtom)
        {
            _nextAtom = nextAtom;

            if (nextAtom != null)
            {
                if (nextAtom.Blocked)
                {
                    _nextBlockedAtom = nextAtom;
                    _nextAvailableAtom = nextAtom._nextAvailableAtom;
                }
                else
                {
                    _nextAvailableAtom = nextAtom;
                    _nextBlockedAtom = nextAtom._nextBlockedAtom;
                } // end if
            } // end if
        } // end of InitializeChain

        #endregion InitializeNextAtom

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartTime

        private DateTime _startTime;

        /// <summary>
        /// Start time of the atom
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of Time

        #endregion StartTime

        #region EndTime

        private DateTime _endTime;

        /// <summary>
        /// End time of the atom
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                _endTime = value;
            }
        } // end of EndTime

        #endregion EndTime

        #region ParentTimeLine

        private SinglePerDayTimeLine _parentTimeLine;

        /// <summary>
        /// Parent time line that the atom is hosted by
        /// </summary>
        public SinglePerDayTimeLine ParentTimeLine
        {
            get
            {
                return _parentTimeLine;
            }
        } // end of ParentTimeLine

        #endregion ParentTimeLine

        //--------------------------------------------------------------------------------------------------
        // Connection in TimeInterval
        //--------------------------------------------------------------------------------------------------

        #region NextAtom

        private TimeAtom _nextAtom;

        /// <summary>
        /// Next atom in the time atom chain
        /// </summary>
        public TimeAtom NextAtom
        {
            get
            {
                return _nextAtom;
            }
        } // end of NextAtom

        #endregion NextAtom

        #region PreviousAtom

        private TimeAtom _previousAtom;

        /// <summary>
        /// Previous atom in the time atom chain
        /// </summary>
        public TimeAtom PreviousAtom
        {
            get
            {
                return _previousAtom;
            }
        } // end of PreviousAtom

        #endregion PreviousAtom

        #region NextAvailableAtom

        protected TimeAtom _nextAvailableAtom;

        /// <summary>
        /// Next atom in the atom chain that is still bookable
        /// </summary>
        public TimeAtom NextAvailableAtom
        {
            get
            {
                return _nextAvailableAtom;
            }
        } // end of NextAvailableAtom

        #endregion NextAvailableAtom

        #region PreviousAvailableAtome

        protected TimeAtom _previousAvailableAtom;

        /// <summary>
        /// Previous atom in the atom chain that is still bookable
        /// </summary>
        public TimeAtom PreviousAvailableAtome
        {
            get
            {
                return _previousAvailableAtom;
            }
        } // end of PreviousAvailableAtome

        #endregion PreviousAvailableAtome

        #region NextBlockedAtom

        protected TimeAtom _nextBlockedAtom;

        /// <summary>
        /// Next atom in the atom chain that is not bookable
        /// </summary>
        public TimeAtom NextBlockedAtom
        {
            get
            {
                return _nextBlockedAtom;
            }
        } // end of NextBlockedAtom

        #endregion NextBlockedAtom

        #region PreviousBlockedAtom

        protected TimeAtom _previousBlockedAtom;

        /// <summary>
        /// Previous atom in the atom chain that is not bookable
        /// </summary>
        public TimeAtom PreviousBlockedAtom
        {
            get
            {
                return _previousBlockedAtom;
            }
        } // end of PreviousBlockedAtom

        #endregion PreviousBlockedAtom

        //--------------------------------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------------------------------

        #region MaxCapacity

        private double _maxCapacity;

        /// <summary>
        /// Max capacity of the atom that can be booked
        /// </summary>
        public double MaxCapacity
        {
            get
            {
                return _maxCapacity;
            }
        } // end of MaxCapacity

        #endregion MaxCapacity

        #region Capacity

        private double _capacity;

        /// <summary>
        /// Current booked capacity
        /// </summary>
        public double Capacity
        {
            get
            {
                return _capacity;
            }
        } // end of Capacity

        #endregion Capacity

        #region Blocked

        private bool _blocked;

        /// <summary>
        /// Flag if atom is blocked for booking
        /// </summary>
        public bool Blocked
        {
            get
            {
                return _blocked;
            }
        } // end of Blocked

        #endregion Blocked

        #region NonBookable

        private bool _nonBookable;

        /// <summary>
        /// Flag that prevents an atom to be the first atom in a slot booking. It can be used for slots but
        /// a slot is not allowed to start at this atom
        /// </summary>
        public bool NonBookable
        {
            get
            {
                return _nonBookable;
            }
            set
            {
                _nonBookable = value;
            }
        } // end of NonBookable

        #endregion NonBookable

        #region SlotsAtTime

        private List<Slot> _slotsAtTime;

        /// <summary>
        /// Currently booked slosts
        /// </summary>
        public IReadOnlyList<Slot> SlotsAtTime
        {
            get
            {
                return _slotsAtTime;
            }
        } // end of SlotsAtTime

        #endregion SlotsAtTime

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region BookSlot

        /// <summary>
        /// Booking of a slot to this atom
        /// </summary>
        /// <param name="slot"></param>
        public void BookSlot(Slot slot)
        {
            _slotsAtTime.Add(slot);
            _capacity += slot.Capacity;

            //--------------------------------------------------------------------------------------------------
            // Re-arranging the chain
            //--------------------------------------------------------------------------------------------------
            if (Helpers<double>.EqualsWithNumericalPrecission(Capacity, MaxCapacity))
            {
                _blocked = true;

                if (_previousAtom != null)
                {
                    _previousAtom._nextAvailableAtom = NextAvailableAtom;
                    _previousAtom._nextBlockedAtom = this;
                } // end if

                if (_nextAtom != null)
                {
                    _nextAtom._previousAvailableAtom = _previousAvailableAtom;
                    _nextAtom._previousBlockedAtom = this;
                } // end if
            } // end if
        } // end of BookSlot

        #endregion BookSlot

        #region RemoveSlot

        /// <summary>
        /// Removing a slot previously booked from this atom
        /// </summary>
        /// <param name="slot"></param>
        public void RemoveSlot(Slot slot)
        {
            if (SlotsAtTime.Contains(slot))
            {
                _slotsAtTime.Remove(slot);

                if (Helpers<double>.EqualsWithNumericalPrecission(_capacity, 1) && slot.Capacity > 0)
                {
                    if (_previousAtom != null)
                    {
                        _previousAtom._nextAvailableAtom = this;
                        _previousAtom._nextBlockedAtom = _nextBlockedAtom;
                    } // end if

                    if (_nextAtom != null)
                    {
                        _nextAtom._previousAvailableAtom = this;
                        _nextAtom._previousBlockedAtom = _previousBlockedAtom;
                    } // end if

                    _blocked = false;
                } // end if

                _capacity -= slot.Capacity;
            } // end if
        } // end of RemoveSlot

        #endregion RemoveSlot

        #region ToString

        public override string ToString()
        {
            return StartTime.ToShortTimeString();
        } // end of ToString

        #endregion ToString
    } // end of TimeAtom
}
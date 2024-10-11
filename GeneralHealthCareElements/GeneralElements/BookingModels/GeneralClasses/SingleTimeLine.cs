using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Classs that represents a single time line for a single day
    /// </summary>
    public class SinglePerDayTimeLine
    {
        //--------------------------------------------------------------------------------------------------
        // Constructors
        //--------------------------------------------------------------------------------------------------

        #region ConstructorWithTimeLine

        /// <summary>
        /// Construtor for the time line
        /// </summary>
        /// <param name="configs">Time atom configs that define the structure of the time line</param>
        /// <param name="date">The date the time line is generated for</param>
        public SinglePerDayTimeLine(List<TimeAtomConfig> configs, DateTime date)
        {
            List<TimeAtom> atoms = new List<TimeAtom>();

            // in case no atoms are provided for that day it is considered as full
            if (configs.Count == 0)
            {
                _startTime = date;
                _endTime = date;
                _timeAtoms = atoms.ToArray();
                return;
            } // end if

            _slotsBooked = new List<Slot>();
            _blockedAtoms = new List<TimeAtom>();

            // setting start and end times
            _startTime = date.Date + TimeSpan.FromHours(configs.First().StartTime);
            _endTime = date.Date + TimeSpan.FromHours(configs.Last().EndTime);

            // creating atoms from the provided configs
            for (int i = 0; i < configs.Count; i++)
            {
                atoms.Add(new TimeAtom(this, configs[i], date));
            } // end for

            _timeAtoms = atoms.ToArray();

            // creating the chain of atoms by linking them
            for (int i = 1; i < TimeAtoms.Length; i++)
            {
                TimeAtoms[i].InitializePreviousAtom(TimeAtoms[i - 1]);
            } // end for

            for (int i = TimeAtoms.Length - 2; i >= 0; i--)
            {
                TimeAtoms[i].InitializeNextAtom(TimeAtoms[i + 1]);
            } // end for

            // setting the first available atom
            FirstAvailableAtom = TimeAtoms.Where(p => !(p.Blocked || p.NonBookable)).OrderBy(q => q.StartTime).First();
        } // end of SingleTimeLine

        #endregion ConstructorWithTimeLine

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region SlotsBooked

        private List<Slot> _slotsBooked;

        /// <summary>
        /// Holds all slots that are booked, not necearrily fully booked
        /// </summary>
        public List<Slot> SlotsBooked
        {
            get
            {
                return _slotsBooked;
            }
            set
            {
                _slotsBooked = value;
            }
        } // end of SlotsBooked

        #endregion SlotsBooked

        #region BlockedAtoms

        private List<TimeAtom> _blockedAtoms;

        /// <summary>
        /// List of blocked atoms, e.g. breaks or fully booked atoms
        /// </summary>
        public IReadOnlyList<TimeAtom> BlockedAtoms
        {
            get
            {
                return _blockedAtoms;
            }
        } // end of BlockedAtoms

        #endregion BlockedAtoms

        #region FirstAvailableAtom

        private TimeAtom _firstAvailableAtom;

        /// <summary>
        /// Gets the first atom of the time line that is still bookable
        /// </summary>
        public TimeAtom FirstAvailableAtom
        {
            get
            {
                return _firstAvailableAtom;
            }
            set
            {
                _firstAvailableAtom = value;
            }
        } // end of FirstAvailableAtome

        #endregion FirstAvailableAtom

        #region TimeAtoms

        private TimeAtom[] _timeAtoms;

        /// <summary>
        /// holds all time atoms of the time line
        /// </summary>
        public TimeAtom[] TimeAtoms
        {
            get
            {
                return _timeAtoms;
            }
        } // end of TimeAtoms

        #endregion TimeAtoms

        #region StartTime

        private DateTime _startTime;

        /// <summary>
        /// Start time of the time line
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of StartTime

        #endregion StartTime

        #region EndTime

        private DateTime _endTime;

        /// <summary>
        /// End time of the time line
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        } // end of EndTime

        #endregion EndTime

        #region Full

        /// <summary>
        /// Flag if the time line if fully booked
        /// </summary>
        public bool Full
        {
            get
            {
                if (TimeAtoms.Length == 0)
                    return true;

                return BlockedAtoms.Count == TimeAtoms.Length;
            }
        } // end of Full

        #endregion Full

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Constraints

        /// <summary>
        /// Delegate to define methods that can act as slot constraints for certain atoms, thereby for
        ///  example the share of bookings per atom can be balanced or certain requests are only
        ///  allowed during certain periods of the time line
        /// </summary>
        /// <param name="request">The request to be booked</param>
        /// <param name="timeLine">The time line for the booking</param>
        /// <param name="atom">The atom that would be covered by the booking</param>
        /// <returns></returns>
        public delegate bool SingleTimeLineConstraints(SlotRequest request, SinglePerDayTimeLine timeLine, TimeAtom atom);

        #endregion Constraints

        #region GetEarliestSlotTime

        /// <summary>
        /// Earliest slot for a request
        /// </summary>
        /// <param name="request">The request for the slot</param>
        /// <param name="constraints">Constraints associated with the request</param>
        /// <returns></returns>
        public Slot GetEarliestSlotTime(SlotRequest request, SingleTimeLineConstraints constraints)
        {
            if (Full)
                return null;

            return GetFirstSlotAtMinAtom(FirstAvailableAtom, request, constraints);
        } // end if

        #endregion GetEarliestSlotTime

        #region GetAllSlotTimes

        /// <summary>
        /// Gets all slots on the time line for a request
        /// </summary>
        /// <param name="request">The request for the slot</param>
        /// <param name="constraints">Constraints associated with the request</param>
        /// <returns></returns>
        public List<Slot> GetAllSlotTime(SlotRequest request, SingleTimeLineConstraints constraints)
        {
            List<Slot> allSlots = new List<Slot>();

            if (Full)
                return allSlots;

            TimeAtom currentAtom = FirstAvailableAtom;

            // loops through all possible start atoms
            while (currentAtom != null)
            {
                allSlots.Add(GetFirstSlotAtMinAtom(currentAtom, request, constraints));
                currentAtom = currentAtom.NextAvailableAtom;
            } // end while

            return allSlots;
        } // end of GetAllSlotTime

        #endregion GetAllSlotTimes

        #region GetFirstSlotAtMinAtom

        /// <summary>
        /// Gets the first slot after a minimum atom that can be booked
        /// </summary>
        /// <param name="firstAtom">First atom to consider</param>
        /// <param name="request">Request for slot</param>
        /// <param name="constraints">Constraints associated with the slot</param>
        /// <returns>The first slot with a starting atom later than the first atom that can be booked</returns>
        private Slot GetFirstSlotAtMinAtom(TimeAtom firstAtom, SlotRequest request, SingleTimeLineConstraints constraints)
        {
            TimeAtom currentAtom = firstAtom;
            DateTime availableTo;

            // setting start atom for search
            while (currentAtom != null && (currentAtom.Blocked || currentAtom.NonBookable))
            {
                currentAtom = currentAtom.NextAvailableAtom;
            } // while

            // looping through possible start atoms
            while (currentAtom != null)
            {
                TimeAtom loopingAtom;

                // calc the availability period for the current atom
                if (currentAtom.NextBlockedAtom == null)
                {
                    availableTo = EndTime;
                }
                else
                {
                    availableTo = currentAtom.NextBlockedAtom.StartTime;
                } // end if

                // checking if the availability length is sufficient
                if (availableTo - currentAtom.StartTime >= request.Length)
                {
                    // checking constraints for booking
                    // this is currently only done for the start atom of a possible slot
                    // could be easily extended to the whole stretch
                    if (constraints(request, this, currentAtom))
                    {
                        DateTime endTime = currentAtom.StartTime + request.Length;

                        loopingAtom = currentAtom;

                        bool noViolationDetected = true;

                        List<TimeAtom> coveredAtoms = new List<TimeAtom>();

                        // checking the capacitity of all covered atoms
                        while (loopingAtom != null && loopingAtom.StartTime < endTime && noViolationDetected)
                        {
                            coveredAtoms.Add(loopingAtom);

                            if (loopingAtom.Capacity > loopingAtom.MaxCapacity - request.Capacity)
                                noViolationDetected = false;

                            loopingAtom = loopingAtom.NextAtom;
                        } // end while

                        // in case a booking would be possible the atom is returned
                        if (noViolationDetected)
                            return new Slot(
                                this,
                                currentAtom,
                                coveredAtoms.ToArray(),
                                endTime,
                                request.Capacity,
                                request.Type.AdmissionType.Identifier);
                    } // end if
                } // end if

                currentAtom = currentAtom.NextAvailableAtom;
            } // end while

            return null;
        } // end of GetFirstSlotAfterTime

        #endregion GetFirstSlotAtMinAtom

        #region BookSlot

        /// <summary>
        /// Books a slot on the time line
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public bool BookSlot(Slot slot)
        {
            foreach (TimeAtom atomCoveredBySlot in slot.CoveredAtoms)
            {
                atomCoveredBySlot.BookSlot(slot);
                if (atomCoveredBySlot.Blocked && BlockedAtoms.Contains(atomCoveredBySlot))
                    _blockedAtoms.Add(atomCoveredBySlot);
            } // end foreach

            _slotsBooked.Add(slot);

            return true;
        } // end of BookSlot

        #endregion BookSlot

        #region CoversTime

        /// <summary>
        /// Checks if a certain time is covered by the time line
        /// </summary>
        /// <param name="time">Time to check</param>
        /// <returns>Returns true if the passed time is covered by the time line</returns>
        public bool CoversTime(DateTime time)
        {
            return StartTime <= time && time <= EndTime;
        } // end of CoversTime

        #endregion CoversTime

        #region CancelSlot

        /// <summary>
        /// Cancels a booked slot
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        public void CancelSlot(Slot slot)
        {
            _slotsBooked.Remove(slot);
            foreach (TimeAtom atom in slot.CoveredAtoms)
            {
                atom.RemoveSlot(slot);
            } // end foreach
        } // end of

        #endregion CancelSlot

        #region ToString

        public override string ToString()
        {
            return StartTime.ToShortDateString() + ": NumberSlots: " + SlotsBooked.Count;
        } // end of ToString

        #endregion ToString
    } // end of SingleTimeLine
}
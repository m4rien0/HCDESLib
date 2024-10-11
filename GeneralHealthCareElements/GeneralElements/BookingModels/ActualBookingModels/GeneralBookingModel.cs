using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// This class implements a general booking model based on time line
    /// It is not concerned how these time lines are generated and hence
    /// they can be arbitrarily created
    /// </summary>
    public class GeneralBookingModel : IBookingModel
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="numberMinimumTimeLines">The minimum number of time lines kept in the future to make bookings, e.g. 28 for 4 weeks</param>
        /// <param name="timeLineConfiguration">The time line configurator responsible for creation of time lines</param>
        public GeneralBookingModel(int numberMinimumTimeLines,
                                   ITimeLineConfiguration timeLineConfiguration)
        {
            if (numberMinimumTimeLines <= 0)
                throw new InvalidOperationException("At least one time line has to be kept for booking");

            _timeLineConfiguration = timeLineConfiguration;
            _minimumActiveTimeLinesKept = numberMinimumTimeLines;

            _timeLines = new List<SinglePerDayTimeLine>();
        } // end of GeneralBookingModel

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Initialization of the booking model, the time lines are filled with the minimum number
        /// </summary>
        /// <param name="startTime">Start time of the booking model</param>
        public void Initialize(DateTime startTime)
        {
            _currentStartTime = startTime;

            DateTime currentTime = startTime;

            SinglePerDayTimeLine currentTimeLine = TimeLineConfiguration.GetFirstTimeLine(currentTime);

            _timeLines.Add(currentTimeLine);

            while (TimeLines.Count < MinimumActiveTimeLinesKept)
            {
                currentTimeLine = TimeLineConfiguration.GetNextTimeLine(currentTimeLine);
                _timeLines.Add(currentTimeLine);
            } // end while
            _currentEndTime = TimeLines.Last().EndTime;
        } // end of Initialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region CurrentStartTime

        private DateTime _currentStartTime;

        /// <summary>
        /// Start time of the period that is currently available for booking
        /// </summary>
        public DateTime CurrentStartTime
        {
            get
            {
                return _currentStartTime;
            }
        } // end of CurrentStartTime

        #endregion CurrentStartTime

        #region CurrentEndTime

        private DateTime _currentEndTime;

        /// <summary>
        /// End time of the period that is currently available for booking
        /// </summary>
        public DateTime CurrentEndTime
        {
            get
            {
                return _currentEndTime;
            }
            set
            {
                _currentEndTime = value;
            }
        } // end of CurrentEndTime

        #endregion CurrentEndTime

        #region MinimumActiveTimeLinesKept

        private int _minimumActiveTimeLinesKept;

        /// <summary>
        /// The minimum number of time lines kept in the future to make bookings, e.g. 28 for 4 weeks
        /// </summary>
        public int MinimumActiveTimeLinesKept
        {
            get
            {
                return _minimumActiveTimeLinesKept;
            }
        } // end of MinimumActiveTimeLinesKept

        #endregion MinimumActiveTimeLinesKept

        #region TimeLines

        private List<SinglePerDayTimeLine> _timeLines;

        /// <summary>
        /// Time lines that are currently used for booking
        /// </summary>
        public IReadOnlyList<SinglePerDayTimeLine> TimeLines
        {
            get
            {
                return _timeLines;
            }
        } // end of TimeLines

        #endregion TimeLines

        #region TimeLineConfiguration

        private ITimeLineConfiguration _timeLineConfiguration;

        /// <summary>
        /// Configurator that is responsible for creation of time lines
        /// </summary>
        public ITimeLineConfiguration TimeLineConfiguration
        {
            get
            {
                return _timeLineConfiguration;
            }
        } // end of TimeLineConfiguration

        #endregion TimeLineConfiguration

        //--------------------------------------------------------------------------------------------------
        // Public Methods implementing the booking model interface
        //--------------------------------------------------------------------------------------------------

        #region GetEarliestSlot

        /// <summary>
        /// Loops through time lines and looks for the earliest slot
        /// for the request
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <returns>The earliest slot for the request</returns>
        public Slot GetEarliestSlot(DateTime currentTime, SlotRequest request)
        {
            CutTimeLines(currentTime);

            // if earliest time is later than latest time of all time lines
            // new time lines are added
            while (TimeLines.Last().EndTime < request.EarliestTime)
            {
                AddSingleTimeLine();
            } // end while

            Slot slotFound = null;

            // loop through all time lines to find a slot
            for (int i = 0; i < TimeLines.Count; i++)
            {
                if (TimeLines[i].StartTime >= request.EarliestTime)
                {
                    if ((slotFound = TimeLines[i].GetEarliestSlotTime(request, ConstraintsWithinTimeLine)) != null)
                        break;
                } // end if
            } // end for

            // if all time lines are full or constraints do not allow extra bookings
            // new time lines are added as long as booking is possible
            while (slotFound == null)
            {
                AddSingleTimeLine();
                slotFound = TimeLines.Last().GetEarliestSlotTime(request, ConstraintsWithinTimeLine);
            } // end while

            return slotFound;
        } // end of GetEarliestSlot

        #endregion GetEarliestSlot

        #region GetAllSlotTimes

        /// <summary>
        /// Loops through all time lines in the window and returns all slots possible in these time lines
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <param name="latestTime">Latest time for a booking</param>
        /// <returns>All available slots for the request within the specified time window</returns>
        public List<Slot> GetAllSlotTimes(DateTime currentTime, SlotRequest request, DateTime latestTime)
        {
            CutTimeLines(currentTime);

            // if current time lines do not cover the entire period extra time lines are added
            while (TimeLines.Last().EndTime < latestTime)
            {
                AddSingleTimeLine();
            } // end while

            List<Slot> allSlots = new List<Slot>();

            // loop through all time lines and add possible slots
            for (int i = 0; i < TimeLines.Count; i++)
            {
                if (TimeLines[i].StartTime >= latestTime)
                    break;

                if (TimeLines[i].StartTime <= request.EarliestTime)
                {
                    allSlots.AddRange(TimeLines[i].GetAllSlotTime(request, ConstraintsWithinTimeLine));
                }
                else
                {
                    break;
                } // end if
            } // end for

            return allSlots;
        } // end of GetAllSlotTimes

        #endregion GetAllSlotTimes

        #region BookSlot

        /// <summary>
        /// Action to actually book a slot
        /// </summary>
        /// <param name="slot">Slot to be booled</param>
        public bool BookSlot(Slot slot)
        {
            slot.ParentTimeLine.BookSlot(slot);

            return true;
        } // end of BookSlot

        #endregion BookSlot

        #region CancelSlot

        /// <summary>
        /// Action to cancel a booking
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        public void CancelSlot(Slot slot)
        {
            slot.StartTimeAtom.ParentTimeLine.CancelSlot(slot);
        } // end of CancelSlot

        #endregion CancelSlot

        //--------------------------------------------------------------------------------------------------
        // Constraints
        //--------------------------------------------------------------------------------------------------

        #region TimeLineConstraints

        /// <summary>
        /// This method my be overwritten to specify constraints on time lines and requests, e.g. certain bookings
        /// only on specific days
        /// </summary>
        /// <param name="request"></param>
        /// <param name="timeLine"></param>
        /// <returns></returns>
        public virtual bool TimeLineConstraints(SlotRequest request, SinglePerDayTimeLine timeLine)
        {
            return true;
        } // end of TimeLineConstraints

        #endregion TimeLineConstraints

        #region ConstraintsWithinTimeLine

        /// <summary>
        /// Method that defines constraints within a single time line, e.g. booking of different types to be
        /// balanced
        /// </summary>
        /// <param name="request">Request to be booked</param>
        /// <param name="timeLine">Time line for the booking</param>
        /// <param name="atom">Atom to be used as a start atom for a slot</param>
        /// <returns></returns>
        public virtual bool ConstraintsWithinTimeLine(SlotRequest request, SinglePerDayTimeLine timeLine, TimeAtom atom)
        {
            return true;
        } // end of ConstraintsWithinTimeLine

        #endregion ConstraintsWithinTimeLine

        //--------------------------------------------------------------------------------------------------
        // Private Methods
        //--------------------------------------------------------------------------------------------------

        #region AddSingleTimeLine

        /// <summary>
        /// Extendes the list of time lines by a new single time line using the time line generator
        /// </summary>
        private void AddSingleTimeLine()
        {
            SinglePerDayTimeLine currentTimeLine;

            if (TimeLines.Count == 0)
                currentTimeLine = TimeLineConfiguration.GetFirstTimeLine(CurrentEndTime);
            else
                currentTimeLine = TimeLines.Last();

            _timeLines.Add(TimeLineConfiguration.GetNextTimeLine(currentTimeLine));
        } // end of AddSingleTimeLine

        #endregion AddSingleTimeLine

        #region CutTimeLines

        /// <summary>
        /// Cuts time lines that are in the past and hence not bookable any more
        /// Retains a minimum number of time lines
        /// </summary>
        /// <param name="currentTime"></param>
        private void CutTimeLines(DateTime currentTime)
        {
            if (TimeLines.Count == 0)
                return;

            while (TimeLines.First().EndTime <= currentTime)
            {
                _timeLines.RemoveAt(0);

                if (TimeLines.Count < MinimumActiveTimeLinesKept)
                    AddSingleTimeLine();
            } // end while
        } // end of AddSingleTimeLine

        #endregion CutTimeLines
    } // end of GeneralBookingModel
}
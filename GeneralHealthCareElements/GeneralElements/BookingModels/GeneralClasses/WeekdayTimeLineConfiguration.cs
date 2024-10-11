using SimulationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// A time line configuration for slots bookable over weekdays
    /// </summary>
    public class PerWeekdayTimeLineConfiguration : ITimeLineConfiguration
    {
        //--------------------------------------------------------------------------------------------------
        // Constructors
        //--------------------------------------------------------------------------------------------------

        #region WeekdayDetailedConstructor

        /// <summary>
        /// Basic constructor that works with an increment atom configuration or
        /// manually defined atoms. The user is responsible for specifiying only
        /// one config per day (either increment or list of configs)
        /// </summary>
        /// <param name="timeConfigPerDayStartEndIncremnet">Increment configs for time atoms</param>
        /// <param name="timeConfigPerDay">Manually defined configs</param>
        /// <param name="blockedDays">Days that are not bookable, e.g. holidays</param>
        public PerWeekdayTimeLineConfiguration(
            Dictionary<DayOfWeek, TimeAtomStartEndIncrementConfig> timeConfigPerDayStartEndIncremnet,
            Dictionary<DayOfWeek, List<TimeAtomConfig>> timeConfigPerDay,
                                     List<DateTime> blockedDays)
        {
            _blockedDates = blockedDays;

            Dictionary<DayOfWeek, List<TimeAtomConfig>> configsPerDay = new Dictionary<DayOfWeek, List<TimeAtomConfig>>();

            // in this case configs are defined per increment
            if (timeConfigPerDayStartEndIncremnet != null)
            {
                // foreach day in the passed increment configs a list of time atom configs is generated
                // with respect to start- end time and increment
                foreach (DayOfWeek day in timeConfigPerDayStartEndIncremnet.Keys)
                {
                    TimeAtomStartEndIncrementConfig dayTimeConfig = timeConfigPerDayStartEndIncremnet[day];

                    TimeSpan currenTime = TimeSpan.FromHours(dayTimeConfig.StartHour);
                    TimeSpan endTime = TimeSpan.FromHours(dayTimeConfig.EndHour);

                    List<TimeAtomConfig> atomConfigs = new List<TimeAtomConfig>();

                    while (currenTime < endTime)
                    {
                        TimeSpan nextTime = currenTime + TimeSpan.FromHours(dayTimeConfig.Increment);
                        atomConfigs.Add(new TimeAtomConfig(currenTime.TotalHours, nextTime.TotalHours, false, 1, false));
                        currenTime = nextTime;
                    } // end while

                    configsPerDay.Add(day, atomConfigs);
                } // end foreach
            } // end if

            _timeConfigPerDay = configsPerDay;

            // manually defined configs are also added
            if (timeConfigPerDay != null)
            {
                foreach (DayOfWeek day in timeConfigPerDay.Keys)
                {
                    if (_timeConfigPerDay.ContainsKey(day))
                        throw new InvalidOperationException("Per day only one type of definition allowed");

                    _timeConfigPerDay.Add(day, timeConfigPerDay[day]);
                } // end foreach
            } // end if

            // filling up missing days with empty configs (will not be bookable)
            foreach (DayOfWeek day in Helpers<DayOfWeek>.GetEnumValues())
            {
                if (!_timeConfigPerDay.ContainsKey(day))
                    _timeConfigPerDay.Add(day, new List<TimeAtomConfig>());
            } // end foreach
        } // end of TimeLineConfiguration

        #endregion WeekdayDetailedConstructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region TimeConfigPerDay

        private Dictionary<DayOfWeek, List<TimeAtomConfig>> _timeConfigPerDay;

        /// <summary>
        /// Time atom configs for each weekday, possibly empty list if day is not
        /// specified
        /// </summary>
        public Dictionary<DayOfWeek, List<TimeAtomConfig>> TimeConfigPerDay
        {
            get
            {
                return _timeConfigPerDay;
            }
            set
            {
                _timeConfigPerDay = value;
            }
        } // end of TimeConfigPerDay

        #endregion TimeConfigPerDay

        #region BlockedDates

        private List<DateTime> _blockedDates;

        /// <summary>
        /// List of blocked days, e.g. holidays that are not bookable
        /// </summary>
        public IReadOnlyList<DateTime> BlockedDates
        {
            get
            {
                return _blockedDates;
            }
        } // end of BlockedDates

        #endregion BlockedDates

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetFirstTimeLine

        /// <summary>
        /// Creates the time line of the start day or the next day that is not blocked
        /// </summary>
        /// <param name="startTime">Start time of the booking model</param>
        /// <returns>The time line on the start day of the booking model, or the first non-blocked day</returns>
        public SinglePerDayTimeLine GetFirstTimeLine(DateTime startTime)
        {
            DateTime currentTime = startTime.Date;

            while (BlockedDates.Contains(currentTime))
            {
                currentTime += TimeSpan.FromDays(1);
            } // end while

            List<TimeAtomConfig> configsForDay = TimeConfigPerDay[startTime.DayOfWeek];

            while (configsForDay.Count > 0 && configsForDay.First().EndTime < startTime.TimeOfDay.TotalHours)
            {
                configsForDay.RemoveAt(0);
            } // end while

            return new SinglePerDayTimeLine(configsForDay, startTime.Date);
        } // end of GetFirstTimeLine

        #endregion GetFirstTimeLine

        #region GetNextTimeLine

        /// <summary>
        /// The next time line, counted from the last time line held in a booking model
        /// </summary>
        /// <param name="lastTimeLine">The last time line</param>
        /// <returns>Next time line on a day that is specified and not blocked</returns>
        public SinglePerDayTimeLine GetNextTimeLine(SinglePerDayTimeLine lastTimeLine)
        {
            DateTime nextDate = lastTimeLine.StartTime.Date + TimeSpan.FromDays(1);

            return GetFirstTimeLine(nextDate);
        } // end of GetNextTimeLine

        #endregion GetNextTimeLine
    } // end of TimeLineConfiguration
}
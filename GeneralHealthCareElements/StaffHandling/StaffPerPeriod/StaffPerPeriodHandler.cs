using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.StaffHandling
{
    #region StaffChangingPoint

    /// <summary>
    /// Represents a point in time (for one day) where a change of staff occurs, i.e.
    /// a staff availability period ends or a new period starts
    /// </summary>
    public class StaffChangingPoint
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="day">Weekday of change</param>
        /// <param name="timeOfDay">Time during day of change</param>
        public StaffChangingPoint(DayOfWeek day, TimeSpan timeOfDay)
        {
            _dayOfWeek = day;
            _timeOfDay = timeOfDay;
            _periodsStarting = new List<StaffAvailabilityPeriod>();
            _periodsEnding = new List<StaffAvailabilityPeriod>();
        } // end of StaffChangingPoint

        #endregion Constructor

        #region DayOfWeek

        private DayOfWeek _dayOfWeek;

        /// <summary>
        /// Weekday of change
        /// </summary>
        public DayOfWeek DayOfWeek
        {
            get
            {
                return _dayOfWeek;
            }
            set
            {
                _dayOfWeek = value;
            }
        } // end of DayOfWeek

        #endregion DayOfWeek

        #region TimeOfDay

        private TimeSpan _timeOfDay;

        /// <summary>
        /// Time during day of change
        /// </summary>
        public TimeSpan TimeOfDay
        {
            get
            {
                return _timeOfDay;
            }
            set
            {
                _timeOfDay = value;
            }
        } // end of TimeOfDay

        #endregion TimeOfDay

        #region PeriodsStarting

        private List<StaffAvailabilityPeriod> _periodsStarting;

        /// <summary>
        /// List of periods that start at the change point
        /// </summary>
        public List<StaffAvailabilityPeriod> PeriodsStarting
        {
            get
            {
                return _periodsStarting;
            }
            set
            {
                _periodsStarting = value;
            }
        } // end of PeriodsStarting

        #endregion PeriodsStarting

        #region PeriodsEnding

        private List<StaffAvailabilityPeriod> _periodsEnding;

        /// <summary>
        /// List of periods that end at change point
        /// </summary>
        public List<StaffAvailabilityPeriod> PeriodsEnding
        {
            get
            {
                return _periodsEnding;
            }
            set
            {
                _periodsEnding = value;
            }
        } // end of PeriodsEnding

        #endregion PeriodsEnding
    } // end of

    #endregion StaffChangingPoint

    /// <summary>
    /// Handles for each weekday the number and skills of doctors and nurses available
    /// </summary>
    public class StaffPerPeriodHandler : IStaffHandling
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that creates a staff handler for a day time line config for different weeekdays
        /// </summary>
        /// <param name="daysPerWeekConfigs">Configs defining the staff handler per weekday</param>
        public StaffPerPeriodHandler(Dictionary<DayOfWeek, DayTimeLineConfig> daysPerWeekConfigs)
        {
            _daysPerWeekConfig = daysPerWeekConfigs;
            _staffChangingPerDayAndTime = new Dictionary<DayOfWeek, Dictionary<TimeSpan, StaffChangingPoint>>();
            _staffPerPeriod = new Dictionary<DayOfWeek, Dictionary<StaffAvailabilityPeriod, ResourceAssignmentStaff[]>>();
            _sortedChangeTimesPerDay = new Dictionary<DayOfWeek, List<TimeSpan>>();

            // in case day was not specified empty list is added
            StaffAvailabilityPeriod emptyPeriod = new StaffAvailabilityPeriod(0,
                24,
                Helpers<ResourceAssignment<SkillSet>>.EmptyArray(),
                Helpers<ResourceAssignment<SkillSet>>.EmptyArray());
            foreach (DayOfWeek day in Helpers<DayOfWeek>.GetEnumValues())
            {
                if (!DaysPerWeekConfigs.Keys.Contains(day))
                {
                    DaysPerWeekConfigs.Add(day, new DayTimeLineConfig(emptyPeriod));
                } // end if

                // adding all period changes
                _staffChangingPerDayAndTime.Add(day, new Dictionary<TimeSpan, StaffChangingPoint>());
                _staffPerPeriod.Add(day, new Dictionary<StaffAvailabilityPeriod, ResourceAssignmentStaff[]>());

                foreach (StaffAvailabilityPeriod period in DaysPerWeekConfigs[day].StaffAvailabilityPeriods)
                {
                    if (!StaffChangingPerDayAndTime[day].ContainsKey(period.StartTime))
                    {
                        StaffChangingPerDayAndTime[day].Add(period.StartTime, new StaffChangingPoint(day, period.StartTime));
                    } // end if

                    StaffChangingPerDayAndTime[day][period.StartTime].PeriodsStarting.Add(period);

                    if (!StaffChangingPerDayAndTime[day].ContainsKey(period.EndTime))
                    {
                        StaffChangingPerDayAndTime[day].Add(period.EndTime, new StaffChangingPoint(day, period.EndTime));
                    } // end if

                    StaffChangingPerDayAndTime[day][period.EndTime].PeriodsEnding.Add(period);
                } // end foreach

                _sortedChangeTimesPerDay.Add(day, StaffChangingPerDayAndTime[day].Values.Select(p => p.TimeOfDay).OrderBy(q => q.Ticks).ToList());
            } // end foreach

            // initializing staff lists
            _doctors = new List<EntityDoctor>();
            _nurses = new List<EntityNurse>();

            // -----------------------------------------------------------------------------
            // we assume here that for each availability period new staff members join
            // availability periods can overlap, so to avoid unrealistic shift changes
            // overlaping periods can be used
            // further, staff members are generated separately for each period, to avoid that
            // they can be directly passed to availability periods and will be used here
            // -----------------------------------------------------------------------------

            foreach (KeyValuePair<DayOfWeek, DayTimeLineConfig> dayConfig in DaysPerWeekConfigs)
            {
                _staffPerPeriod[dayConfig.Key] = new Dictionary<StaffAvailabilityPeriod, ResourceAssignmentStaff[]>();

                foreach (StaffAvailabilityPeriod period in dayConfig.Value.StaffAvailabilityPeriods)
                {
                    List<ResourceAssignmentStaff> stafferPeriod = new List<ResourceAssignmentStaff>();

                    if (period.DoctorsAvailable != null)
                    {
                        stafferPeriod.AddRange(period.DoctorsAvailable);
                    }
                    else
                    {
                        foreach (ResourceAssignment<SkillSet> skillAssignment in period.DoctorSkillsAvailable)
                        {
                            stafferPeriod.Add(new ResourceAssignmentStaff(new EntityDoctor(skillAssignment.Resource), skillAssignment.OrganizationalUnit, skillAssignment.AssignmentType));
                        } // end foreach
                    } // end if

                    if (period.NursesAvailable != null)
                    {
                        stafferPeriod.AddRange(period.NursesAvailable);
                    }
                    else
                    {
                        foreach (ResourceAssignment<SkillSet> skillAssignment in period.NurseSkillsAvailable)
                        {
                            stafferPeriod.Add(new ResourceAssignmentStaff(new EntityNurse(skillAssignment.Resource), skillAssignment.OrganizationalUnit, skillAssignment.AssignmentType));
                        } // end foreach
                    } // end if

                    _staffPerPeriod[dayConfig.Key].Add(period, stafferPeriod.ToArray());
                } // end foreach
            } // end foreach
        } // end of StaffPerPeriodHandler

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region DaysPerWeekConfigs

        private Dictionary<DayOfWeek, DayTimeLineConfig> _daysPerWeekConfig;

        /// <summary>
        /// Configs defining the staff handler per weekday
        /// </summary>
        public Dictionary<DayOfWeek, DayTimeLineConfig> DaysPerWeekConfigs
        {
            get
            {
                return _daysPerWeekConfig;
            }
            set
            {
                _daysPerWeekConfig = value;
            }
        } // end of DaysPerWeekConfigs

        #endregion DaysPerWeekConfigs

        #region Doctors

        private List<EntityDoctor> _doctors;

        /// <summary>
        /// List of doctors that are assinged during all periods
        /// </summary>
        public List<EntityDoctor> Doctors
        {
            get
            {
                return _doctors;
            }
            set
            {
                _doctors = value;
            }
        } // end of Doctors

        #endregion Doctors

        #region Nurses

        private List<EntityNurse> _nurses;

        /// <summary>
        /// List of nurses that are assinged during all periods
        /// </summary>
        public List<EntityNurse> Nurses
        {
            get
            {
                return _nurses;
            }
            set
            {
                _nurses = value;
            }
        } // end of Nurses

        #endregion Nurses

        #region StaffPerPeriod

        private Dictionary<DayOfWeek, Dictionary<StaffAvailabilityPeriod, ResourceAssignmentStaff[]>> _staffPerPeriod;

        /// <summary>
        /// Staff assignments for each period for each day
        /// </summary>
        public Dictionary<DayOfWeek, Dictionary<StaffAvailabilityPeriod, ResourceAssignmentStaff[]>> StaffPerPeriod
        {
            get
            {
                return _staffPerPeriod;
            }
        } // end of StaffPerPeriod

        #endregion StaffPerPeriod

        #region StaffChangingPerDayAndTime

        private Dictionary<DayOfWeek, Dictionary<TimeSpan, StaffChangingPoint>> _staffChangingPerDayAndTime;

        /// <summary>
        /// Gets a staff changing point for a weekday and time
        /// </summary>
        public Dictionary<DayOfWeek, Dictionary<TimeSpan, StaffChangingPoint>> StaffChangingPerDayAndTime
        {
            get
            {
                return _staffChangingPerDayAndTime;
            }
            set
            {
                _staffChangingPerDayAndTime = value;
            }
        } // end of StaffChangingPerDay

        #endregion StaffChangingPerDayAndTime

        #region SortedChangeTimesPerDay

        private Dictionary<DayOfWeek, List<TimeSpan>> _sortedChangeTimesPerDay;

        /// <summary>
        /// All changes that occur for a day of week in a sorted order
        /// </summary>
        public Dictionary<DayOfWeek, List<TimeSpan>> SortedChangeTimesPerDay
        {
            get
            {
                return _sortedChangeTimesPerDay;
            }
        } // end of SortedChangeTimesPerDay

        #endregion SortedChangeTimesPerDay

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetStartingStaff

        /// <summary>
        /// Creates a list of staff assingments at the start time
        /// </summary>
        /// <param name="startTime">Start time of the handler</param>
        /// <returns></returns>
        public List<ResourceAssignmentStaff> GetStartingStaff(DateTime startTime)
        {
            // look for day of the week
            DayTimeLineConfig currentDayConfig = DaysPerWeekConfigs[startTime.DayOfWeek];

            // scan through periods of day to find all periods in and staff of periods
            List<ResourceAssignmentStaff> startingStaff = new List<ResourceAssignmentStaff>();

            foreach (StaffAvailabilityPeriod period in currentDayConfig.StaffAvailabilityPeriods)
            {
                if (period.StartTime <= startTime.TimeOfDay
                    && startTime.TimeOfDay < period.EndTime)
                    startingStaff.AddRange(StaffPerPeriod[startTime.DayOfWeek][period]);
            } // end foreach

            foreach (EntityHealthCareStaff staff in startingStaff.Select(p => p.Resource))
            {
                staff.StaffOutsideShift = false;
            } // end foreach

            return startingStaff;
        } // end of GetStartingStaff

        #endregion GetStartingStaff

        #region GetNextStaffChangingEvent

        /// <summary>
        /// Gets an event that represents actions for the next point in time
        /// where staffing levels change
        /// </summary>
        /// <param name="parentControl">Control for which the staff hanlder is called</param>
        /// <param name="time">Time for which the next event (in the future with respect to this time) is requested</param>
        /// <param name="timeToOccur">Out parameter to represent the time the next change will occur</param>
        /// <returns>An event that triggers the staff change</returns>
        public Event GetNextStaffChangingEvent(ControlUnitHealthCareDepartment parentControl, DateTime time, out DateTime timeToOccur)
        {
            timeToOccur = time;

            // look for change later on the same day
            List<TimeSpan> changesOfCurrentDay = SortedChangeTimesPerDay[time.DayOfWeek];
            StaffChangingPoint closestChangingPoint = null;
            StaffChangingPoint pointOnNextDayWithSameTime = null;

            if (changesOfCurrentDay.Count > 0 && changesOfCurrentDay.Last() > time.TimeOfDay)
            {
                for (int i = 0; i < changesOfCurrentDay.Count; i++)
                {
                    if (changesOfCurrentDay[i] > time.TimeOfDay)
                    {
                        closestChangingPoint = StaffChangingPerDayAndTime[time.DayOfWeek][changesOfCurrentDay[i]];
                        timeToOccur = time.Date + closestChangingPoint.TimeOfDay;
                        break;
                    } // end if
                } // end for

                // check if changing point was at end of day
                if (closestChangingPoint.TimeOfDay.Days == 1)
                {
                    DayOfWeek nextDay = (time.Date + TimeSpan.FromDays(1)).DayOfWeek;

                    //checking if there is a changing point at the beginning of the next day
                    if (StaffChangingPerDayAndTime[nextDay].ContainsKey(TimeSpan.FromHours(0)))
                    {
                        pointOnNextDayWithSameTime = StaffChangingPerDayAndTime[nextDay][TimeSpan.FromHours(0)];
                    } // end if
                } // end if
            }
            else
            {
                DateTime nextTime = time.Date;

                while (closestChangingPoint == null)
                {
                    nextTime += TimeSpan.FromDays(1);

                    List<TimeSpan> changesOfNextDay = SortedChangeTimesPerDay[nextTime.DayOfWeek];

                    if (changesOfNextDay.Count > 0)
                    {
                        closestChangingPoint = StaffChangingPerDayAndTime[nextTime.DayOfWeek][changesOfNextDay.First()];
                        timeToOccur = nextTime + closestChangingPoint.TimeOfDay;
                    } // end if
                } // end while
            } // end if

            List<ResourceAssignmentStaff> allStaffLeaving = new List<ResourceAssignmentStaff>();
            List<ResourceAssignmentStaff> allStaffArriving = new List<ResourceAssignmentStaff>();

            foreach (StaffAvailabilityPeriod period in closestChangingPoint.PeriodsEnding)
            {
                allStaffLeaving.AddRange(StaffPerPeriod[closestChangingPoint.DayOfWeek][period]);
            } // end foreach

            foreach (StaffAvailabilityPeriod period in closestChangingPoint.PeriodsStarting)
            {
                allStaffArriving.AddRange(StaffPerPeriod[closestChangingPoint.DayOfWeek][period]);
            } // end foreach

            // staff change on the beginning of next day
            if (pointOnNextDayWithSameTime != null)
            {
                foreach (StaffAvailabilityPeriod period in pointOnNextDayWithSameTime.PeriodsEnding)
                {
                    allStaffLeaving.AddRange(StaffPerPeriod[pointOnNextDayWithSameTime.DayOfWeek][period]);
                } // end foreach

                foreach (StaffAvailabilityPeriod period in pointOnNextDayWithSameTime.PeriodsStarting)
                {
                    allStaffArriving.AddRange(StaffPerPeriod[pointOnNextDayWithSameTime.DayOfWeek][period]);
                } // end foreach
            } // end if

            List<EntityHealthCareStaff> arrivingStaff = allStaffArriving.Select(p => p.Resource).ToList();
            List<EntityHealthCareStaff> leavingStaff = allStaffLeaving.Select(p => p.Resource).ToList();
            List<EntityHealthCareStaff> jointStaff = arrivingStaff.Where(p => leavingStaff.Contains(p)).ToList();

            return new EventStaffChange(parentControl,
                allStaffLeaving.Where(p => !jointStaff.Contains(p.Resource)).ToArray(),
                allStaffArriving.Where(p => !jointStaff.Contains(p.Resource)).ToArray(),
                this);
        } // end of GetNextStaffChangingEvent

        #endregion GetNextStaffChangingEvent
    } // end of StaffPerPeriodHandler
}
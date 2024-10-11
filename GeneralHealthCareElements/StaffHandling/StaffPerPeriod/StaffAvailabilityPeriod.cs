using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using System;

namespace GeneralHealthCareElements.StaffHandling
{
    /// <summary>
    /// Staff available can be defined via this class, either by specifying skill sets
    /// or actual staff members if they need to be distinct
    /// </summary>
    public class StaffAvailabilityPeriod
    {
        #region Constructor

        /// <summary>
        /// Constructor that defines availability using skills
        /// </summary>
        /// <param name="startHour">Start hour of period</param>
        /// <param name="endHour">End hour of period</param>
        /// <param name="doctorSkillsAvailable">Doctor skills available in period</param>
        /// <param name="nurseSkillAvailable">Nurse skills available in period</param>
        public StaffAvailabilityPeriod(
            double startHour,
            double endHour,
            ResourceAssignment<SkillSet>[] doctorSkillsAvailable,
            ResourceAssignment<SkillSet>[] nurseSkillAvailable)
        {
            _startHour = startHour;
            _endHour = endHour;
            _doctorSkillAvailable = doctorSkillsAvailable;
            _nurseSkillAvailable = nurseSkillAvailable;
        } // end of StaffAvailabilityPeriod

        /// <summary>
        /// Constructor that defines availability using entities
        /// </summary>
        /// <param name="startHour">Start hour of period</param>
        /// <param name="endHour">End hour of period</param>
        /// <param name="doctorSkillsAvailable">Doctors available in period</param>
        /// <param name="nurseSkillAvailable">Nurses available in period</param>
        public StaffAvailabilityPeriod(
            double startHour,
            double endHour,
            ResourceAssignmentStaff[] doctorAvailable,
            ResourceAssignmentStaff[] nurseAvailable)
        {
            _startHour = startHour;
            _endHour = endHour;
            _doctorsAvailable = doctorAvailable;
            _nursesAvailable = nurseAvailable;
        } // end of StaffAvailabilityPeriod

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartHour

        private double _startHour;

        /// <summary>
        /// Start hour of period
        /// </summary>
        public double StartHour
        {
            get
            {
                return _startHour;
            }
            set
            {
                _startHour = value;
            }
        } // end of StartHour

        #endregion StartHour

        #region EndHour

        private double _endHour;

        /// <summary>
        /// End hour of period
        /// </summary>
        public double EndHour
        {
            get
            {
                return _endHour;
            }
            set
            {
                _endHour = value;
            }
        } // end of EndHour

        #endregion EndHour

        #region StartTime

        /// <summary>
        /// Transforms start hour to timespan
        /// </summary>
        public TimeSpan StartTime
        {
            get
            {
                return TimeSpan.FromHours(StartHour);
            }
        } // end of StartTime

        #endregion StartTime

        #region EndTime

        /// <summary>
        /// Transforms end hour to timespan
        /// </summary>
        public TimeSpan EndTime
        {
            get
            {
                return TimeSpan.FromHours(EndHour);
            }
        } // end of EndTime

        #endregion EndTime

        #region DoctorSkillsAvailable

        private ResourceAssignment<SkillSet>[] _doctorSkillAvailable;

        /// <summary>
        /// Doctor skills available in period
        /// </summary>
        public ResourceAssignment<SkillSet>[] DoctorSkillsAvailable
        {
            get
            {
                return _doctorSkillAvailable;
            }
            set
            {
                _doctorSkillAvailable = value;
            }
        } // end of DoctorSkillsAvailable

        #endregion DoctorSkillsAvailable

        #region NurseSkillsAvailable

        private ResourceAssignment<SkillSet>[] _nurseSkillAvailable;

        /// <summary>
        /// Nurse skills available in period
        /// </summary>
        public ResourceAssignment<SkillSet>[] NurseSkillsAvailable
        {
            get
            {
                return _nurseSkillAvailable;
            }
            set
            {
                _nurseSkillAvailable = value;
            }
        } // end of NurseSkillsAvailable

        #endregion NurseSkillsAvailable

        #region DoctorsAvailable

        private ResourceAssignmentStaff[] _doctorsAvailable;

        /// <summary>
        /// Doctors available in period
        /// </summary>
        public ResourceAssignmentStaff[] DoctorsAvailable
        {
            get
            {
                return _doctorsAvailable;
            }
        } // end of DoctorsAvailable

        #endregion DoctorsAvailable

        #region NursesAvailable

        private ResourceAssignmentStaff[] _nursesAvailable;

        /// <summary>
        /// Nurse available in period
        /// </summary>
        public ResourceAssignmentStaff[] NursesAvailable
        {
            get
            {
                return _nursesAvailable;
            }
        } // end of NursesAvailable

        #endregion NursesAvailable
    } // end of StaffAvailabilityPeriod
}
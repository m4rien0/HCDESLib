namespace GeneralHealthCareElements.StaffHandling
{
    /// <summary>
    /// Configures the periods of a day during that different staff availabilities may occur
    /// </summary>
    public class DayTimeLineConfig
    {
        #region Constructor

        /// <summary>
        /// Basic constructor for a time line config that consists of mutliple staff
        /// availability periods
        /// </summary>
        /// <param name="staffAvailablities">Staff availability periods</param>
        public DayTimeLineConfig(StaffAvailabilityPeriod[] staffAvailablities)
        {
            _staffAvailabilityPeriods = staffAvailablities;
        } // end of DayTimeLineConfig

        /// <summary>
        /// Basic constructor for a time line config that consists of a single staff
        /// availability period
        /// </summary>
        /// <param name="staffAvailablities">Staff availability period</param>
        public DayTimeLineConfig(StaffAvailabilityPeriod staffAvailablities)
        {
            _staffAvailabilityPeriods = new StaffAvailabilityPeriod[] { staffAvailablities };
        } // end of DayTimeLineConfig

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StaffAvailabilityPeriods

        private StaffAvailabilityPeriod[] _staffAvailabilityPeriods;

        /// <summary>
        /// Staff availability periods that define the day time line config
        /// </summary>
        public StaffAvailabilityPeriod[] StaffAvailabilityPeriods
        {
            get
            {
                return _staffAvailabilityPeriods;
            }
        } // end of StaffAvailabilityPeriods

        #endregion StaffAvailabilityPeriods
    } // end of
}
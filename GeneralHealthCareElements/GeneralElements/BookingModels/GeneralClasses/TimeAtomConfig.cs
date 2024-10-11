using System.Xml.Serialization;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Config class to define a time atom, used for input, xml serializable
    /// </summary>
    public class TimeAtomConfig
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startTime">Start time of atom</param>
        /// <param name="endTime">End time of atom</param>
        /// <param name="isBlocked">Flag if atom is blocked</param>
        /// <param name="maxCapacity">Max capacity to booked in atom</param>
        /// <param name="nonStartBookable">Can atom be used to start a slot</param>
        public TimeAtomConfig(
            double startTime,
            double endTime,
            bool isBlocked,
            double maxCapacity,
            bool nonStartBookable = false)
        {
            _startTime = startTime;
            _endTime = endTime;
            _blocked = isBlocked;
            _maxCapacity = maxCapacity;
        } // end of TimeAtomConfig

        /// <summary>
        /// Empty constructor for xml serialization, capacity is set to 1 per default
        /// </summary>
        public TimeAtomConfig()
        {
            _maxCapacity = 1;
        } // end of TimeAtomConfig

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartTime

        private double _startTime;

        /// <summary>
        /// Start time of atom
        /// </summary>
        [XmlAttribute]
        public double StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of Time

        #endregion StartTime

        #region EndTime

        private double _endTime;

        /// <summary>
        /// End time of atom
        /// </summary>
        [XmlAttribute]
        public double EndTime
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

        //--------------------------------------------------------------------------------------------------
        // State
        //--------------------------------------------------------------------------------------------------

        #region MaxCapacity

        private double _maxCapacity;

        /// <summary>
        /// Max capacity to booked in atom
        /// </summary>
        [XmlAttribute]
        public double MaxCapacity
        {
            get
            {
                return _maxCapacity;
            }
            set
            {
                _maxCapacity = value;
            }
        } // end of MaxCapacity

        #endregion MaxCapacity

        #region Blocked

        private bool _blocked;

        /// <summary>
        /// Flag if atom is blocked
        /// </summary>
        [XmlAttribute]
        public bool Blocked
        {
            get
            {
                return _blocked;
            }
            set
            {
                _blocked = value;
            }
        } // end of Blocked

        #endregion Blocked

        #region NonBookable

        private bool _nonStartBookable;

        /// <summary>
        /// Flag if atom can be used to start a slot
        /// </summary>
        [XmlAttribute]
        public bool NonStartBookable
        {
            get
            {
                return _nonStartBookable;
            }
            set
            {
                _nonStartBookable = value;
            }
        } // end of NonBookable

        #endregion NonBookable
    } // end of TimeAtonConfig
}
using System.Xml.Serialization;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Time atom configs for a time line with a start, end time and increment
    /// </summary>
    public class TimeAtomStartEndIncrementConfig
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startHour">Start of config</param>
        /// <param name="endHour">End of Config</param>
        /// <param name="increment">Increment of config</param>
        public TimeAtomStartEndIncrementConfig(double startHour, double endHour, double increment)
        {
            _startHour = startHour;
            _endHour = endHour;
            _increment = increment;
        } // end of TimeAtomStartEndIncrementConfig

        /// <summary>
        /// Empty constructor for xml serialization
        /// </summary>
        public TimeAtomStartEndIncrementConfig()
        {
        } // end of TimeAtomStartEndIncrementConfig

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartHour

        private double _startHour;

        /// <summary>
        /// Start of config
        /// </summary>
        [XmlAttribute]
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
        /// End of Config
        /// </summary>
        [XmlAttribute]
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

        #region Increment

        private double _increment;

        /// <summary>
        /// Increment of config
        /// </summary>
        [XmlAttribute]
        public double Increment
        {
            get
            {
                return _increment;
            }
            set
            {
                _increment = value;
            }
        } // end of Increment

        #endregion Increment
    }
}
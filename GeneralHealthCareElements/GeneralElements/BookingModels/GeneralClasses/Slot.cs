using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Class to represent a booking slot
    /// </summary>
    public class Slot
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentTimeLine">Time line that the slot is hosted by</param>
        /// <param name="startTimeAtom">Start atom of the slot</param>
        /// <param name="coveredAtoms">All time atoms covered by the slot</param>
        /// <param name="endTime">End time of the slot</param>
        /// <param name="capacity">Capacity consumed by the slot</param>
        /// <param name="type">Type of slot</param>
        public Slot(SinglePerDayTimeLine parentTimeLine,
                    TimeAtom startTimeAtom,
                    TimeAtom[] coveredAtoms,
                    DateTime endTime,
                    double capacity,
                    string type)
        {
            _parentTimeLine = parentTimeLine;
            _startTimeAtom = startTimeAtom;
            _startTime = startTimeAtom.StartTime;
            _coveredAtoms = coveredAtoms;
            _endTime = endTime;
            _capacity = capacity;
            _type = type;
            _immediate = false;
        } // end of Slot

        /// <summary>
        /// Constructor for immediate slots
        /// </summary>
        /// <param name="startTime">Start time of slot</param>
        public Slot(DateTime startTime)
        {
            _startTime = startTime;
            _immediate = true;
        } // end of Slot

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region ParentTimeLine

        private SinglePerDayTimeLine _parentTimeLine;

        /// <summary>
        /// Time line that the slot is hosted by
        /// </summary>
        public SinglePerDayTimeLine ParentTimeLine
        {
            get
            {
                return _parentTimeLine;
            }
        } // end of ParentTimeLine

        #endregion

        #region StartTimeAtom

        private TimeAtom _startTimeAtom;

        /// <summary>
        /// Start atom of the slot
        /// </summary>
        public TimeAtom StartTimeAtom
        {
            get
            {
                return _startTimeAtom;
            }
        } // end of StartTimeAtom

        #endregion

        #region CoveredAtoms

        private TimeAtom[] _coveredAtoms;

        /// <summary>
        /// All time atoms covered by the slot
        /// </summary>
        public TimeAtom[] CoveredAtoms
        {
            get
            {
                return _coveredAtoms;
            }
        } // end of CoveredAtoms

        #endregion

        #region StartTime

        private DateTime _startTime;

        /// <summary>
        /// Start time of the slot
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of StartTime

        #endregion

        #region EndTime

        private DateTime _endTime;

        /// <summary>
        /// End time of the slot
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
        } // end of EndTime

        #endregion

        #region Length

        /// <summary>
        /// Length of slot
        /// </summary>
        public TimeSpan Length
        {
            get
            {
                return _endTime - _startTime;
            }
        } // end of Length

        #endregion

        #region Type

        private string _type;

        /// <summary>
        /// Type of slot
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }
        } // end of Type

        #endregion

        #region Immediate

        private bool _immediate;

        /// <summary>
        /// Flag if slot is booked immediatly with no real booking model
        /// </summary>
        public bool Immediate
        {
            get
            {
                return _immediate;
            }
            set
            {
                _immediate = value;
            }
        } // end of Immediate

        #endregion

        #region Capacity

        private double _capacity;

        /// <summary>
        /// Capacity consumed by the slot
        /// </summary>
        public double Capacity
        {
            get
            {
                return _capacity;
            }
        } // end of Capacity

        #endregion

        #region ToString

        public override string ToString()
        {
            return "From: " + StartTime.ToShortTimeString() + " To: " + (StartTime + Length).ToShortTimeString();
        } // end of ToString

        #endregion

    } // end of Slot
}

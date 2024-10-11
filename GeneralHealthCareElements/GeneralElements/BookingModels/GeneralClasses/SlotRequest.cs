using GeneralHealthCareElements.TreatmentAdmissionTypes;
using System;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Class that implements a request for a slot
    /// </summary>
    public class SlotRequest
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="earliestTime">Earliest time the request can be booked</param>
        /// <param name="length">Required length of the slot to be booked</param>
        /// <param name="type">Type of request</param>
        /// <param name="capacity">Capacity to be consumed by the booking</param>
        public SlotRequest(DateTime earliestTime,
                          TimeSpan length,
                          Admission type,
                          double capacity)
        {
            _earliestTime = earliestTime;
            _length = length;
            _type = type;
            _capacity = capacity;
        } // end of SlotRequest

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region EarliestTime

        private DateTime _earliestTime;

        /// <summary>
        /// Earliest time the request can be booked
        /// </summary>
        public DateTime EarliestTime
        {
            get
            {
                return _earliestTime;
            }
        } // end of EarliestTime

        #endregion EarliestTime

        #region Length

        private TimeSpan _length;

        /// <summary>
        /// Required length of the slot to be booked
        /// </summary>
        public TimeSpan Length
        {
            get
            {
                return _length;
            }
        } // end of Length

        #endregion Length

        #region Type

        private Admission _type;

        /// <summary>
        /// Type of request
        /// </summary>
        public Admission Type
        {
            get
            {
                return _type;
            }
        } // end of Type

        #endregion Type

        #region Capacity

        private double _capacity;

        /// <summary>
        /// Capacity to be consumed by the booking
        /// </summary>
        public double Capacity
        {
            get
            {
                return _capacity;
            }
        } // end of Capacity

        #endregion Capacity
    } // end of SlotRequest
}
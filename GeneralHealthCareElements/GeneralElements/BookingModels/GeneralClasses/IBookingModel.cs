using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Interface the defines the requirements for a booking model
    /// </summary>
    public interface IBookingModel
    {
        /// <summary>
        /// Method to initialize the booking model, e.g. build slots from start time
        /// </summary>
        /// <param name="startTime">Time the booking model starts</param>
        void Initialize(DateTime startTime);

        /// <summary>
        /// Basic functionality of a booking model to get the earliest available time for a slot request
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <returns>The earliest slot for the request</returns>
        Slot GetEarliestSlot(DateTime currentTime, SlotRequest request);

        /// <summary>
        /// Basic functionality of a booking model to get the all available times for a slot request
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <param name="latestTime">Latest time for a booking</param>
        /// <returns>All available slots for the request within the specified time window</returns>
        List<Slot> GetAllSlotTimes(DateTime currentTime, SlotRequest request, DateTime latestTime);

        /// <summary>
        /// Action to actually book a slot
        /// </summary>
        /// <param name="slot">Slot to be booked</param>
        bool BookSlot(Slot slot);

        /// <summary>
        /// Action to cancel a booking
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        void CancelSlot(Slot slot);
    } // end of IBookingModel
}

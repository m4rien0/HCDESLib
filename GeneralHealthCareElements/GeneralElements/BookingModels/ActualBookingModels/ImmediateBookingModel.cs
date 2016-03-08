using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Default booking model that returns a slot at the time the booking is requested
    /// </summary>
    public class ImmediateBookingModel : IBookingModel
    {
        //--------------------------------------------------------------------------------------------------
        // Public Methods 
        //--------------------------------------------------------------------------------------------------

        #region GetEarliestSlot

        /// <summary>
        /// Returns current time as no actual booking is made
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <returns>The earliest slot for the request</returns>
        public Slot GetEarliestSlot(DateTime currentTime, SlotRequest request)
        {

            return new Slot(currentTime);

        } // end of GetEarliestSlot

        #endregion

        #region GetAllSlotTimes

        /// <summary>
        /// Does not make sense for this model and throws an exception
        /// </summary>
        /// <param name="currentTime">Time the request is made</param>
        /// <param name="request">slot request to be booked</param>
        /// <param name="latestTime">Latest time for a booking</param>
        /// <returns>All available slots for the request within the specified time window</returns>
        public List<Slot> GetAllSlotTimes(DateTime currentTime, SlotRequest request, DateTime latestTime)
        {
            throw new NotImplementedException();
        } // end of GetAllSlotTimes

        #endregion

        #region BookSlot

        /// <summary>
        /// No action required for this model to book a slot
        /// </summary>
        /// <param name="slot">Slot to be booked</param>
        public bool BookSlot(Slot slot)
        {
            return true;
        } // end of BookSlot

        #endregion

        #region CancelSlot

        /// <summary>
        /// No action required for this model to cancel a slot
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        public void CancelSlot(Slot slot)
        {
            
        } // end of CancelSlot

        #endregion

        #region Initialize

        /// <summary>
        /// No action required
        /// </summary>
        /// <param name="startTime">Time the booking model starts</param>
        public void Initialize(DateTime startTime)
        {
            
        } // end of Initialize

        #endregion
        
    } // end of ImmediateBookingModel
}

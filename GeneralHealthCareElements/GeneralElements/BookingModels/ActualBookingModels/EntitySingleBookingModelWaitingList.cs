using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Implements a waiting list with a single booking model
    /// </summary>
    public class EntitySingleBookingModelWaitingList : EntityWaitingListSchedule
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">Entity ID</param>
        /// <param name="input">Required input for the booking model</param>
        /// <param name="bookingModel">The booking model for the waiting list</param>
        public EntitySingleBookingModelWaitingList(
            int ID,
            IInputBookingModel input,
            IBookingModel bookingModel)
            : base(ID, input)
        {
            _bookingModel = bookingModel;
        } // end of EntityMultipleBookingWaitingListSchedule

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Intializes the booking model at the start time of the schedule
        /// </summary>
        /// <param name="startTime">Start time of the schedule</param>
        public override void Initialize(DateTime startTime)
        {
            BookingModel.Initialize(startTime);
        } // end Initialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region BookingModel

        private IBookingModel _bookingModel;

        /// <summary>
        /// Booking model used by the schedule
        /// </summary>
        public IBookingModel BookingModel
        {
            get
            {
                return _bookingModel;
            }
        } // end of BookingModel

        #endregion BookingModel

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetEarliestSlotTime

        /// <summary>
        /// Basic functionality of a booking model to get the earliest available time for a slot request
        /// Obtained from the only booking model
        /// </summary>
        /// <param name="time">Time the request is made</param>
        /// <param name="earliestTime">The earliest time a slot can be booked</param>
        /// <param name="patient">Patient the slot is booked for</param>
        /// <param name="admissionType">The admission type corresponding to the request</param>
        /// <returns>The earliest available slot for the request</returns>
        public override Slot GetEarliestSlotTime(DateTime time, DateTime earliestTime, EntityPatient patient, Admission admission)
        {
            Slot earliestSlot = BookingModel.GetEarliestSlot(time,
                new SlotRequest(earliestTime,
                    InputData.GetSlotLengthPerAdmission(admission),
                    admission,
                    InputData.GetSlotCapacityPerAdmission(admission)));

            return earliestSlot;
        } // end of GetEarliestSlotTime

        #endregion GetEarliestSlotTime

        #region GetAllSlotTimes

        /// <summary>
        /// Basic functionality of a booking model to get the all available times for a slot request
        /// Obtained from the only booking model
        /// </summary>
        /// <param name="time">Time the request is made</param>
        /// <param name="earliestTime">The earliest time a slot can be booked</param>
        /// <param name="latestTime">The latest time a slot can be booked</param>
        /// <param name="patient">Patient the slot is booked for</param>
        /// <param name="admissionType">The admission type corresponding to the request</param>
        /// <returns>All available slots for the request within the specified time window</returns>
        public override List<Slot> GetAllSlotTimes(DateTime time,
            DateTime earliestTime,
            DateTime latestTime,
            EntityPatient patient,
            Admission admission)
        {
            List<Slot> allSlots = BookingModel.GetAllSlotTimes(time,
                new SlotRequest(earliestTime,
                    InputData.GetSlotLengthPerAdmission(admission),
                    admission,
                    InputData.GetSlotCapacityPerAdmission(admission)), latestTime);

            return allSlots;
        } // end of GetAllSlotTimes

        #endregion GetAllSlotTimes

        #region BookSlot

        /// <summary>
        /// Action to actually book a slot
        /// </summary>
        /// <param name="slot">Slot to be booled</param>
        /// <param name="admission">Corresponding admission</param>
        public override void BookSlot(Slot slot, Admission admission)
        {
            BookingModel.BookSlot(slot);
        } // end of BookSlot

        #endregion BookSlot

        #region CancelSlot

        /// <summary>
        /// Action to cancel a booking
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        /// <param name="admission">Corresponding admission</param>
        public override void CancelSlot(Slot slot, Admission admission)
        {
            BookingModel.CancelSlot(slot);
        } // end of CancelSlot

        #endregion CancelSlot

        #region Clone

        public override Entity Clone()
        {
            throw new NotImplementedException();
        } // end of Clone

        #endregion Clone
    } // end of EntitySingleBookingModelWaitingList
}
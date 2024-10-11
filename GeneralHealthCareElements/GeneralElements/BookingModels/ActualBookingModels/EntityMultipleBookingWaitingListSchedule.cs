using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Implements a waiting list schedule that uses multiple booking models for certain
    /// objects, e.g. per doctor or per facility
    /// </summary>
    /// <typeparam name="T">The object type for which the booking models are held</typeparam>
    public class EntityMultipleBookingWaitingListSchedule<T> : EntityWaitingListSchedule
    {
        #region Constructor

        // <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">Entity ID</param>
        /// <param name="input">Required input for the booking model</param>
        /// <param name="bookingModelsPerType">A Dictionary of instances of the specified type and booking mdeols</param>
        /// <param name="extractBookingReferenceFromRequest">A method that finds a booking model specifier for a request</param>
        public EntityMultipleBookingWaitingListSchedule(
            int ID,
            IInputOutpatient input,
            Dictionary<T, IBookingModel> bookingModelsPerType,
            FindBookingModelForRequest extractBookingReferenceFromRequest)
            : base(ID, input)
        {
            _bookingModelsPerType = bookingModelsPerType;
            BookingModelReference = extractBookingReferenceFromRequest;
        } // end of EntityMultipleBookingWaitingListSchedule

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Intializes the booking model at the start time of the schedule
        /// </summary>
        /// <param name="startTime">Start time of the schedule</param>
        public override void Initialize(DateTime startTime)
        {
            foreach (IBookingModel bookingModel in BookingModelsPerType.Values)
            {
                bookingModel.Initialize(startTime);
            } // end foreach
        } // end Initialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region BookingModelsPerType

        private Dictionary<T, IBookingModel> _bookingModelsPerType;

        /// <summary>
        /// Dictionary of key objects and corresponding booking models
        /// </summary>
        public IReadOnlyDictionary<T, IBookingModel> BookingModelsPerType
        {
            get
            {
                return _bookingModelsPerType;
            }
        } // end of BookingModelsPerType

        #endregion BookingModelsPerType

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region FindBookingModelForRequest

        /// <summary>
        /// Delegate for methods to find a booking model type for a request
        /// </summary>
        /// <param name="requestToAssign">Admission request</param>
        /// <returns>Returns the object for which the booking model is used for booking</returns>
        public delegate T FindBookingModelForRequest(Admission requestToAssign);

        private FindBookingModelForRequest BookingModelReference;

        #endregion FindBookingModelForRequest

        #region GetEarliestSlotTime

        /// <summary>
        /// Basic functionality of a booking model to get the earliest available time for a slot request
        /// Obtained from the booking model that is responsible for the booking
        /// </summary>
        /// <param name="time">Time the request is made</param>
        /// <param name="earliestTime">The earliest time a slot can be booked</param>
        /// <param name="patient">Patient the slot is booked for</param>
        /// <param name="admissionType">The admission type corresponding to the request</param>
        /// <returns>The earliest available slot for the request</returns>
        public override Slot GetEarliestSlotTime(DateTime time, DateTime earliestTime, EntityPatient patient, Admission admissionType)
        {
            IBookingModel bookingModel = BookingModelsPerType[BookingModelReference(admissionType)];

            Slot earliestSlot = bookingModel.GetEarliestSlot(time,
                new SlotRequest(earliestTime,
                    InputData.GetSlotLengthPerAdmission(admissionType),
                    admissionType,
                    InputData.GetSlotCapacityPerAdmission(admissionType)));

            return earliestSlot;
        } // end of GetEarliestSlotTime

        #endregion GetEarliestSlotTime

        #region GetAllSlotTimes

        /// <summary>
        /// Basic functionality of a booking model to get the all available times for a slot request
        /// Obtained from the booking model that is responsible for the booking
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
            IBookingModel bookingModel = BookingModelsPerType[BookingModelReference(admission)];

            List<Slot> allSlots = bookingModel.GetAllSlotTimes(time,
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
            IBookingModel bookingModel = BookingModelsPerType[BookingModelReference(admission)];

            bookingModel.BookSlot(slot);
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
            IBookingModel bookingModel = BookingModelsPerType[BookingModelReference(admission)];

            bookingModel.CancelSlot(slot);
        } // end of CancelSlot

        #endregion CancelSlot

        #region Clone

        public override Entity Clone()
        {
            throw new NotImplementedException();
        } // end of Clone

        #endregion Clone
    } // end of EntityMultipleBookingWaitingListSchedule
}
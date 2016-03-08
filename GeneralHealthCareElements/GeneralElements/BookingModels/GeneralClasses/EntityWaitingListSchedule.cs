using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.BookingModels
{
    /// <summary>
    /// Base class representing a waiting list entity
    /// </summary>
    abstract public class EntityWaitingListSchedule : Entity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">Entity ID</param>
        /// <param name="input">Input required for the waiting list</param>
        public EntityWaitingListSchedule(
            int ID, 
            IInputBookingModel input)
            : base(ID)
        {
            _readyForDispatch = false;
            _inputData = input;
        } // end of EntityTreatmentBooth

        #endregion

        #region Initialize

        /// <summary>
        /// Abstract method to initialize the schedule, e.g. build slots from start time
        /// </summary>
        /// <param name="startTime">Time the waiting list starts (is initialized</param>
        public abstract void Initialize(DateTime startTime);

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region ReadyForDispatch

        private bool _readyForDispatch;

        /// <summary>
        /// Flag if the schedule is ready to dispatch slot requests. Plays a role if dispatching is done only at discrete points in time.
        /// </summary>
        public bool ReadyForDispatch
        {
            get
            {
                return _readyForDispatch;
            }
            set
            {
                _readyForDispatch = value;
            }
        } // end of ReadyForDispatch

        #endregion

        #region StartTime

        private DateTime _startTime;

        public DateTime StartTime
        {
            get
            {
                return _startTime;
            }
        } // end of StartTime

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetEarliestSlotTime
        
        /// <summary>
        /// Basic functionality of a schedule to get the earliest available time for a slot request
        /// </summary>
        /// <param name="time">Time the request is made</param>
        /// <param name="earliestTime">The earliest time a slot can be booked</param>
        /// <param name="patient">Patient the slot is booked for</param>
        /// <param name="admissionType">The admission type corresponding to the request</param>
        /// <returns>The earliest available slot for the request</returns>
        public abstract Slot GetEarliestSlotTime(DateTime time, 
            DateTime earliestTime, 
            EntityPatient patient, 
            Admission admissionType);

        #endregion

        #region GetAllSlotTimes

        /// <summary>
        /// Basic functionality of a schedule to get the all available times for a slot request
        /// </summary>
        /// <param name="time">Time the request is made</param>
        /// <param name="earliestTime">The earliest time a slot can be booked</param>
        /// <param name="latestTime">The latest time a slot can be booked</param>
        /// <param name="patient">Patient the slot is booked for</param>
        /// <param name="admissionType">The admission type corresponding to the request</param>
        /// <returns>All available slots for the request within the specified time window</returns>
        public abstract List<Slot> GetAllSlotTimes(DateTime time, 
            DateTime earliestTime,
            DateTime latestTime,
            EntityPatient patient, 
            Admission admissionType);

        #endregion

        #region BookSlot

        /// <summary>
        /// Action to actually book a slot
        /// </summary>
        /// <param name="slot">Slot to be booled</param>
        /// <param name="admission">Corresponding admission</param>
        public abstract void BookSlot(Slot slot, Admission admission);

        #endregion

        #region CancelSlot

        /// <summary>
        /// Action to cancel a booking
        /// </summary>
        /// <param name="slot">Slot to cancel</param>
        /// <param name="admission">Corresponding admission</param>
        public abstract void CancelSlot(Slot slot, Admission admission);

        #endregion

        #region ToString

        public override string ToString()
        {
            return "OutpatientWaitingListSchedule: " + Identifier.ToString();
        } // end of

        #endregion

        #region InputData

        private IInputBookingModel _inputData;

        /// <summary>
        /// Input data that is basis for the booking model
        /// </summary>
        public IInputBookingModel InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion

        #region NextWeekDay

        /// <summary>
        /// helper function to get the next weekday for a given date
        /// </summary>
        /// <param name="day">Date of request</param>
        /// <returns>Next weekday after the requested day, if the request day is a weekday its own date is returned</returns>
        public static DateTime NextWeekDay(DateTime day)
        {
            int daysToAdd = 0;

            if (day.DayOfWeek == DayOfWeek.Friday)
                daysToAdd = 3;
            else if (day.DayOfWeek == DayOfWeek.Saturday)
                daysToAdd = 2;
            else if (day.DayOfWeek == DayOfWeek.Sunday)
                daysToAdd = 1;

            return (day + TimeSpan.FromDays(daysToAdd)).Date;
        } // end of NextWeekDay

        #endregion
    }
}

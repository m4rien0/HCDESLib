using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Input.XMLInputClasses;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Input
{
    /// <summary>
    /// This class extends the GenericXMLDepartmentInput for departments that contain a booking model
    /// </summary>
    abstract public class GenericXMLHCDepInputWithAdmissionAndBookingModel : GenericXMLDepartmentInput, IInputBookingModel 
    {
        //--------------------------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that generates a waiting list schedule
        /// </summary>
        /// <param name="xmlInput">Corresponding xml input</param>
        public GenericXMLHCDepInputWithAdmissionAndBookingModel(XMLInputHealthCareWithWaitingList xmlInput) :base(xmlInput)
        {
            #region WaitingListSchedule

            UseImmediateBookingModel = xmlInput.UseImmediateBookingModel;

            // in case a immediate booking model is used there is not much to do
            if (UseImmediateBookingModel)
            {
                _waitingListScedule = new EntitySingleBookingModelWaitingList(0, this, new ImmediateBookingModel());
            }
            else
            {
                // first time line configs for the schedule are created
                Dictionary<DayOfWeek, TimeAtomStartEndIncrementConfig> perDayConfigStartEndIncrement = new Dictionary<DayOfWeek, TimeAtomStartEndIncrementConfig>();
                Dictionary<DayOfWeek, List<TimeAtomConfig>> perDayConfigDetailedDefinition = new Dictionary<DayOfWeek, List<TimeAtomConfig>>();

                // goes through all time line defintions of specified days
                foreach (XMLTimeLinePerWeekdayDefinition dayDefinition in xmlInput.TimeLineConfigsPerDay)
                {
                    DayOfWeek weekday = ParseEnum<DayOfWeek>(dayDefinition.Weekday);

                    // checks if the time line were defines via increment or manual time atom configs
                    if (dayDefinition.StartEndIncrementConfig != null)
                    {
                        perDayConfigStartEndIncrement.Add(weekday, dayDefinition.StartEndIncrementConfig);
                    }
                    else
                    {
                        perDayConfigDetailedDefinition.Add(weekday, new List<TimeAtomConfig>(dayDefinition.TimeAtomConfigs));
                    } // end if

                } // end foreach

                // parses the blocked dates from input
                List<DateTime> blockedDates = xmlInput.BlockedDates.Select(p => DateTime.ParseExact(p, "yyyy-MM-dd", CultureInfo.InvariantCulture)).ToList();

                // creates a single booking model waiting list schedule
                // multiple booking model schedules are not supported yet
                EntitySingleBookingModelWaitingList waitingListSchedule = new EntitySingleBookingModelWaitingList(
                   0,
                   this,
                   new GeneralBookingModel(7, new PerWeekdayTimeLineConfiguration(
                       perDayConfigStartEndIncrement,
                       perDayConfigDetailedDefinition,
                       blockedDates)));

                _waitingListScedule = waitingListSchedule;
            } // end if

            #endregion

            #region WaitingListDispatching

            _waitingListDispatchTimesPerWeekday = new Dictionary<DayOfWeek, List<TimeSpan>>();

            // defines dispatching times for the waiting list, e.g. times the slots are assigned
            foreach (XMLOutpatientWaitingListDispatchingTimes disTimes in xmlInput.WaitingListDispatchingTimes)
            {
                DayOfWeek currentDay = ParseEnum<DayOfWeek>(disTimes.Weekday);

                _waitingListDispatchTimesPerWeekday.Add(currentDay, new List<TimeSpan>());
                _waitingListDispatchTimesPerWeekday[currentDay].AddRange(disTimes.HoursForDispatching.Select(p => TimeSpan.FromHours(p)));
                _waitingListDispatchTimesPerWeekday[currentDay].OrderBy(p => p.TotalHours);
            } // end foreach

            #endregion

            #region AdmissionTypes

            _admissionDefinitionPerAdmissionType = new Dictionary<string, XMLAdmissionDefinition>();

            _admissionTypes = xmlInput.AdmissionDefinitions.Select(p=> p.AdmissionType).ToArray();

            foreach (XMLAdmissionDefinition admissionType in xmlInput.AdmissionDefinitions)
            {
                _admissionDefinitionPerAdmissionType.Add(admissionType.AdmissionType, admissionType);
            } // end foreach

            #endregion

        } // end of GenericXMLHCDepInputWithAdmissionAndBookingModel

	    #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region NoShowProbabiltyPerAdmission

        private Dictionary<string, double> _noShowProbabilityPerAdmission;

        /// <summary>
        /// Stores probabilities of now-shows per admission type
        /// </summary>
        public Dictionary<string, double> NoShowProbabiltyPerAdmission
        {
            get
            {
                return _noShowProbabilityPerAdmission;
            }
        } // end of NoShowProbabiltyPerAdmission

        #endregion

        #region UseImmediateBookingModel

        private bool _useImmediateBookingModel;

        /// <summary>
        /// Flag if a immediate booking model is used
        /// </summary>
        public bool UseImmediateBookingModel
        {
            get
            {
                return _useImmediateBookingModel;
            }
            set
            {
                _useImmediateBookingModel = value;
            }
        } // end of UseImmediateBookingModel

        #endregion

        #region AdmissionDefinitionPerAdmissionType

        private Dictionary<string, XMLAdmissionDefinition> _admissionDefinitionPerAdmissionType;

        /// <summary>
        /// Stores xml admissions per string types
        /// </summary>
        public Dictionary<string, XMLAdmissionDefinition> AdmissionDefinitionPerAdmissionType
        {
            get
            {
                return _admissionDefinitionPerAdmissionType;
            }
        } // end of AdmissionDefinitionPerAdmissionType

        #endregion

        #region WaitingListSchedule

        private EntityWaitingListSchedule _waitingListScedule;

        /// <summary>
        /// The waiting list schedule that is created for booking
        /// </summary>
        public EntityWaitingListSchedule WaitingListSchedule
        {
            get
            {
                return _waitingListScedule;
            }
            set
            {
                _waitingListScedule = value;
            }
        } // end of WaitingListSchedule

        #endregion

        #region AdmissionTypes

        private string[] _admissionTypes;

        /// <summary>
        /// Handled admission types
        /// </summary>
        public string[] AdmissionTypes
        {
            get
            {
                return _admissionTypes;
            }
        } // end of AdmissionTypes

        #endregion

        #region WaitingListDispatchTimesPerWeekday

        private Dictionary<DayOfWeek, List<TimeSpan>> _waitingListDispatchTimesPerWeekday;

        /// <summary>
        /// Dispatch times of waiting lists per weekday
        /// </summary>
        public Dictionary<DayOfWeek, List<TimeSpan>> WaitingListDispatchTimesPerWeekday
        {
            get
            {
                return _waitingListDispatchTimesPerWeekday;
            }
            set
            {
                _waitingListDispatchTimesPerWeekday = value;
            }
        } // end of WaitingListDispatchTimesPerWeekday

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region NextDispatching

        /// <summary>
        /// Method to determine the next time dispatching is done for the patients on the waiting list
        /// i.e. booking is performed
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public DateTime NextDispatching(DateTime time)
        {
            DayOfWeek currentDay = time.DayOfWeek;

            int totalDaysPassed = 0;

            if (!(WaitingListDispatchTimesPerWeekday.ContainsKey(currentDay)
                && time.Date + WaitingListDispatchTimesPerWeekday[currentDay].Last() > time))
            {
                currentDay = ParseEnum<DayOfWeek>((((int)(currentDay + 1) % 7)).ToString());
                totalDaysPassed++;

                while (!WaitingListDispatchTimesPerWeekday.ContainsKey(currentDay) && totalDaysPassed <= 7)
                {
                    currentDay = ParseEnum<DayOfWeek>((((int)(currentDay + 1)%7)).ToString());
                    totalDaysPassed++;
                } // end while
            } // end if

            if (totalDaysPassed == 0)
            {
                TimeSpan currentTime = WaitingListDispatchTimesPerWeekday[currentDay].Last();

                int currentIndex = WaitingListDispatchTimesPerWeekday[currentDay].Count - 1;

                while (time.Date + currentTime > time && currentIndex >= 1)
                {
                    currentIndex--;
                    currentTime = WaitingListDispatchTimesPerWeekday[currentDay][currentIndex];
                } // end while

                return time.Date + currentTime;
            }
            else
            {
                return time.Date + TimeSpan.FromDays(totalDaysPassed) + WaitingListDispatchTimesPerWeekday[currentDay].First();
            } // end if

        } // end of LatestFollowUp

        #endregion

        #region GetSlotLengthPerAdmission

        /// <summary>
        /// Defines slot lengths required for an admission
        /// </summary>
        /// <param name="admission">Admission to book</param>
        /// <returns>Length that a slot must have to accomodate the admission</returns>
        public TimeSpan GetSlotLengthPerAdmission(Admission admission)
        {
            if (UseImmediateBookingModel)
                return TimeSpan.FromMinutes(0);

            return TimeSpan.FromMinutes(AdmissionDefinitionPerAdmissionType[admission.AdmissionType.Identifier].Length);
        } // end of GetSlotLengthPerTreatment

        #endregion

        #region GetSlotCapacityPerAdmission

        /// <summary>
        /// Defines slot capactiy required for an admission
        /// </summary>
        /// <param name="admission">Admission to book</param>
        /// <returns>Capactiy that all time atoms of a slot must have to accomodate the admission</returns>
        public double GetSlotCapacityPerAdmission(Admission admission)
        {
            if (UseImmediateBookingModel)
                return 0;

            return AdmissionDefinitionPerAdmissionType[admission.AdmissionType.Identifier].Capacity;
        } // end of GetSlotCapacityPerTreatment

        #endregion

        #region GetWaitingListSchedule

        /// <summary>
        /// Returns the waiting list schedule
        /// </summary>
        /// <returns></returns>
        public EntityWaitingListSchedule GetWaitingListSchedule()
        {
            return WaitingListSchedule;
        } // end of GetWaitingListSchedule

        #endregion

        #region NoShowForAppointment

        /// <summary>
        /// Determines if a patient shows for an appointment
        /// </summary>
        /// <param name="patient">Patient that is considered</param>
        /// <param name="admission">Admission the patient should show up to</param>
        /// <param name="slot">Booked slot for the admission</param>
        /// <param name="currentTime">Current time</param>
        /// <returns>True of the patient does not show up</returns>
        public bool NoShowForAppointment(EntityPatient patient, Admission admission, Slot slot, DateTime currentTime)
        {
            return Distributions.Instance.RandomNumberGenerator.NextDouble() < AdmissionDefinitionPerAdmissionType[admission.AdmissionType.Identifier].NoShowProbability;
        } // end of NoShowForAppointment

        #endregion

        #region PatientArrivalDeviationFromSlotTime

        /// <summary>
        /// Creates a time deviation of the patient arrival time and the original slot time, e.g. patient is late or early. Currently implements a
        /// triangular distribution with parameters defined in the admission definition
        /// </summary>
        /// <param name="patient">Patient for appointment</param>
        /// <param name="admission">Admission type the patient is showing up for</param>
        /// <returns>A positive or negative time span that determines the lateness or earliness of a patient</returns>
        public TimeSpan PatientArrivalDeviationFromSlotTime(EntityPatient patient, Admission admission)
        {
            XMLAdmissionDefinition admissionDef = AdmissionDefinitionPerAdmissionType[admission.AdmissionType.Identifier];

            return TimeSpan.FromMinutes(Distributions.Instance.TriangularDistribution(admissionDef.ShowUpDeviationTriangularEarly, 
                                                                                               admissionDef.ShowUpDeviationTriangularMean,
                                                                                               admissionDef.ShowUpDeviationTriangularLate));
        } // end of PatientArrivalDeviationFromSlotTime

        #endregion

    } // end of GenericXMLHealthCareDepartmentInputWithBookingModel
}

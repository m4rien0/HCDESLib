using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList
{
    /// <summary>
    /// Event that represents the stream of patients to the waiting list and
    /// is also used to handle referred patients from other departments
    /// </summary>
    public class EventOutpatientWaitingListPatientArrival : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent waiting list conrol</param>
        /// <param name="outpatientControlUnit">Parent outpatient department control</param>
        /// <param name="patient">Arriving patient</param>
        /// <param name="admissionType">Admission type of arriving patient</param>
        /// <param name="input">Corresponding outpatient input data</param>
        public EventOutpatientWaitingListPatientArrival(ControlUnit parentControlUnit,
                                              ControlUnitOutpatient outpatientControlUnit,
                                              EntityPatient patient,
                                              Admission admissionType,
                                              IInputOutpatient input)
            : base(EventType.Standalone, parentControlUnit)
        {
            _patient = patient;
            _admissionType = admissionType;
            _outpatientControlUnit = outpatientControlUnit;
            _inputData = input;
        } // end of Event

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// State chage of event. Checks if patient comes from outside of the model. In that case
        /// a new event is triggered. Waiting for assign slot is started.
        /// </summary>
        /// <param name="time">Arriving time</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            if (AdmissionType.IsExtern)
            {
                DateTime nextArrivalTime;
                Admission admission;
                EntityPatient newPatient = InputData.GetNextWaitingListPatient(out nextArrivalTime,
                    out admission,
                    ParentControlUnit,
                    time);

                if (newPatient != null)
                {
                    EventOutpatientWaitingListPatientArrival nextArrival =
                        new EventOutpatientWaitingListPatientArrival(ParentControlUnit,
                            OutpatientControlUnit,
                            newPatient,
                            admission,
                            InputData);

                    simEngine.AddScheduledEvent(nextArrival, nextArrivalTime);
                } // end if
            } // end if

            DateTime earliestTime = (time + TimeSpan.FromDays(AdmissionType.MinDaySpan)).Date;
            DateTime latestTime = DateTime.MaxValue;
            if (AdmissionType.MaxDaySpan < double.MaxValue)
                latestTime = (time + TimeSpan.FromDays(AdmissionType.MaxDaySpan)).Date;

            ActivityOutpatientWaitingListWaitToAssignSlot waitForAssignment
                = new ActivityOutpatientWaitingListWaitToAssignSlot(ParentControlUnit,
                    Patient,
                    AdmissionType,
                    earliestTime,
                    latestTime);

            SequentialEvents.Add(waitForAssignment.StartEvent);
        } // end of Trigger

        #endregion Trigger

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Arriving patient
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion Patient

        #region AffectedEntites

        /// <summary>
        /// Affected entities include only patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { Patient };
            }
        } // end of AffectedEntities

        #endregion AffectedEntites

        //--------------------------------------------------------------------------------------------------
        // Parameters
        //--------------------------------------------------------------------------------------------------

        #region AdmissionType

        private Admission _admissionType;

        /// <summary>
        /// Admission type of arriving patient
        /// </summary>
        public Admission AdmissionType
        {
            get
            {
                return _admissionType;
            }
        } // end of AdmissionType

        #endregion AdmissionType

        #region OutpatientControlUnit

        private ControlUnitOutpatient _outpatientControlUnit;

        /// <summary>
        /// Parent outpatient control unit
        /// </summary>
        public ControlUnitOutpatient OutpatientControlUnit
        {
            get
            {
                return _outpatientControlUnit;
            }
        } // end of OutpatientControlUnit

        #endregion OutpatientControlUnit

        #region InputData

        private IInputOutpatient _inputData;

        /// <summary>
        /// Outpatient input data
        /// </summary>
        public IInputOutpatient InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion InputData

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventWaitingListPatientArrival";
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Event Clone()
        {
            return new EventOutpatientWaitingListPatientArrival(ParentControlUnit, OutpatientControlUnit, (EntityPatient)Patient.Clone(), AdmissionType, InputData);
        } // end of Clone

        #endregion Clone
    }
}
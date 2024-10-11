using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Special event that represent arrival streams of walk in patients
    /// </summary>
    public class EventOutpatientWalkInPatientArrival : Event
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="parentControlUnit">Parent outpatient control</param>
        /// <param name="patient">Arriving patient</param>
        /// <param name="inputData">Corresponding input data</param>
        public EventOutpatientWalkInPatientArrival(ControlUnit parentControlUnit,
            IInputOutpatient inputData,
            EntityPatient patient)
            : base(EventType.Standalone, parentControlUnit)
        {
            _patient = patient;
            _inputData = inputData;
        } // end of EventOutpatientWalkInPatientArrival

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Entities
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
        // Members
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputOutpatient _inputData;

        /// <summary>
        /// Corresponding input data
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

        #region StateChange

        /// <summary>
        /// Overriden state change. Initializes a new arrival and patient paths and first action.
        /// </summary>
        /// <param name="time">Time the patient arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            Patient.OutpatientTreatmentPath =
                    InputData.CreateOutpatientTreatmentPath(Patient,
                    null,
                    time,
                    true);

            ParentControlUnit.AddEntity(Patient);

            DateTime nextTime; ;

            EntityPatient nextPatient = InputData.GetNextWalkInPatient(out nextTime, ParentControlUnit, time);

            EventOutpatientWalkInPatientArrival nextPatientArrival = new EventOutpatientWalkInPatientArrival(ParentControlUnit, InputData, nextPatient);

            simEngine.AddScheduledEvent(nextPatientArrival, nextTime);

            if (Patient.EmergencyTreatmentPath.TakeNextAction(simEngine, this, time, ParentControlUnit))
            {
                SequentialEvents.Add(Patient.StartWaitingActivity());
            } // end if
        } // end of StateChange

        #endregion StateChange

        #region ToString

        public override string ToString()
        {
            throw new NotImplementedException();
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Event Clone()
        {
            throw new NotImplementedException();
        } // end of Clone

        #endregion Clone
    } // end of EventOutpatientWalkInPatientArrival
}
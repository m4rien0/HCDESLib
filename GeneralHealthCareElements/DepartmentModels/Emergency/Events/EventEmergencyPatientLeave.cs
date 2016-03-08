using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Management;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Leaving event of patients, this event is not used for patients leaving
    /// for special services and returning afterwards
    /// </summary>
    public class EventEmergencyPatientLeave : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent emergency control unit</param>
        /// <param name="patient">Patient leaving</param>
        /// <param name="input">Emergency input data</param>
        public EventEmergencyPatientLeave(ControlUnit parentControlUnit, 
            EntityPatient patient,
            IInputEmergency input)
            : base(EventType.Standalone, parentControlUnit)
        {
            _inputData = input;
            _patient = patient;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Statechange
        //--------------------------------------------------------------------------------------------------

        #region Trigger

        /// <summary>
        /// Overriden state change of the event. If the patient's path indicates a inpatient or outpatient
        /// admission the corresponding request is sent up the control tree.
        /// </summary>
        /// <param name="time">Time the patient leaves</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            ParentControlUnit.RemoveEntity(Patient);
            Patient.CorrespondingDoctor = null;

            if (Patient.EmergencyTreatmentPath.InpatientAdmission != null)
            {
                RequestMoveInpatient reqMoveInpatient = new RequestMoveInpatient(Patient.ToArray(),
                        time,
                        Patient,
                        ParentControlUnit,
                        Patient.EmergencyTreatmentPath.InpatientAdmission);
                ParentControlUnit.DelegateOutBox.Add(reqMoveInpatient);
            } // end if

            if (Patient.EmergencyTreatmentPath.OutpatientAdmission != null)
            {
                RequestMoveOutpatient reqMoveOutPatient = new RequestMoveOutpatient(Patient.ToArray(),
                                                                                        time,
                                                                                        Patient,
                                                                                        ParentControlUnit,
                                                                                        Patient.EmergencyTreatmentPath.OutpatientAdmission);

                ParentControlUnit.DelegateOutBox.Add(reqMoveOutPatient);
            } // end if 

            Patient.EmergencyTreatmentPath = null;

        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient that leaves
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion

        #region AffectedEntites

        /// <summary>
        /// Affected entities consist only of the leaving patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { Patient };
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventEmergencyPatientLeave";
        } // end of ToString

        #endregion

        #region Clone

        public override Event Clone()
        {
            return new EventEmergencyPatientLeave(ParentControlUnit, (EntityPatient)Patient.Clone(), InputData);
        } // end of Clone

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Input 
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputEmergency _inputData;

        public IInputEmergency InputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
            }
        } // end of InputData

        #endregion

    } // end of EventPatientLeave
}

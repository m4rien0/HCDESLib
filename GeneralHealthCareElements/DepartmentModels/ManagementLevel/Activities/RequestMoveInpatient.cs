using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;

namespace GeneralHealthCareElements.Management
{
    /// <summary>
    /// Request for a patient to be moved in a inpatient model
    /// </summary>
    public class RequestMoveInpatient : RequestMovePatientActivities
    {
        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="activity">Request for activity type</param>
        /// <param name="origin"></param>
        /// <param name="time">Time request was filed</param>
        /// <param name="patient">Patient that is transfered</param>
        /// <param name="originControlUnit">Origin control unit</param>
        /// <param name="inpatientAdmission">Admission type</param>
        public RequestMoveInpatient(Entity[] origin,
                                    DateTime time,
                                    EntityPatient patient,
                                    ControlUnit originalControlUnit,
                                    Admission inpatientAdmission)
            : base("ActivityMove", origin, time, patient, originalControlUnit)
        {
            _inpatientAdmission = inpatientAdmission;
        } // end region

        #endregion Constructor

        #region InpatientAdmission

        private Admission _inpatientAdmission;

        /// <summary>
        /// Admission type
        /// </summary>
        public Admission InpatientAdmission
        {
            get
            {
                return _inpatientAdmission;
            }
        } // end of InpatientPath

        #endregion InpatientAdmission
    } // end of RequestMoveInpatient
}
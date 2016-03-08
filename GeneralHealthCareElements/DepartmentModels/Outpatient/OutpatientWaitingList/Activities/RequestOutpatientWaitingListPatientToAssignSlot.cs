using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList
{
    /// <summary>
    /// Request for getting a slot assigned
    /// </summary>
    public class RequestOutpatientWaitingListPatientToAssignSlot : ActivityRequest
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="patient">Waiting patient</param>
        /// <param name="time">Time request is filed</param>
        /// <param name="admissionType">Admission type for slot request</param>
        /// <param name="earliestTime">Earliest possible slot time, e.g. follow up not before a week passed</param>
        /// <param name="latestTime">Latest possible time for slot</param>
        public RequestOutpatientWaitingListPatientToAssignSlot(EntityPatient patient, 
            DateTime time, 
            Admission admissionType, 
            DateTime earliestTime,
            DateTime latestTime)
            : base("ActivityOutpatientWaitingListAssignPatientToSlot", patient.ToArray(), time)
        {
            _patient = patient;
            _admissionType = admissionType;
            _earliestTime = earliestTime;
            _latestTime = latestTime;
        } // end of RequestOutpatientTreatment

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient that requires a slot to be assigned
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion

        #region EarliestTime

        private DateTime _earliestTime;

        /// <summary>
        /// Earliest possible time for slot to book
        /// </summary>
        public DateTime EarliestTime
        {
            get
            {
                return _earliestTime;
            }
        } // end of EarliestTime

        #endregion

        #region LatestTime

        private DateTime _latestTime;

        /// <summary>
        /// Latest possible time for slot to book
        /// </summary>
        public DateTime LatestTime
        {
            get
            {
                return _latestTime;
            }
        } // end of LatestTime

        #endregion

        #region AdmissionType

        private Admission _admissionType;

        public Admission AdmissionType
        {
            get
            {
                return _admissionType;
            }
        } // end of AdmissionType

        #endregion
        
    }
}

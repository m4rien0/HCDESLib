using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Request of an outpatient action, can be specified if the patient was a walk-in or
    /// what the original slot time was, as this might affect dispathing decisions.
    /// </summary>
    public class RequestOutpatientAction : RequestHealthCareAction<OutpatientActionTypeClass>
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="patient">Patient requesting the service</param>
        /// <param name="degreeOfCompletion">In case a activity was pre-empted</param>
        /// <param name="actionType">The requested action type</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="resources">There maybe a pre-defined resource set for the activity, e.g. corresponding doctors</param>
        /// <param name="slotTime">Original slot time</param>
        /// <param name="isWalkIn">Flag if patient is a walk-in patient</param>
        public RequestOutpatientAction(EntityPatient patient,
            double degreeOfCompletion,
            OutpatientActionTypeClass actionType,
            DateTime time,
            ResourceSet resources,
            DateTime slotTime,
            bool isWalkIn)
            : base(patient, degreeOfCompletion, actionType, time, resources)
        {
            _slotTime = slotTime;
            _isWalkIn = isWalkIn;
        } // end of RequestEmergencyAssistedTreatment

        #endregion Constructor

        #region SlotTime

        private DateTime _slotTime;

        /// <summary>
        /// Original slot time
        /// </summary>
        public DateTime SlotTime
        {
            get
            {
                return _slotTime;
            }
            set
            {
                _slotTime = value;
            }
        } // end of SlotTime

        #endregion SlotTime

        #region IsWalkIn

        private bool _isWalkIn;

        /// <summary>
        /// Flag if patient was a walk in
        /// </summary>
        public bool IsWalkIn
        {
            get
            {
                return _isWalkIn;
            }
        } // end of IsWalkIn

        #endregion IsWalkIn
    } // end of RequestOutpatientAction
}
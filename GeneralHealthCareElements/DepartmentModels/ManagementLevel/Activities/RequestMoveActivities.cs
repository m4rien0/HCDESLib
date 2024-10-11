using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using System;

namespace GeneralHealthCareElements.Management
{
    /// <summary>
    /// Base request for patient transfers between departments
    /// </summary>
    public abstract class RequestMovePatientActivities : ActivityRequest, IDelegate
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
        public RequestMovePatientActivities(string activity,
                                     Entity[] origin,
                                     DateTime time,
                                     EntityPatient patient,
                                     ControlUnit originControlUnit)
            : base(activity, origin, time)
        {
            _patient = patient;
            _originControlUnit = originControlUnit;
        } // end of RequestMoveActivities

        #endregion Constructor

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient that is transfered
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion Patient

        #region OriginControlUnit

        private ControlUnit _originControlUnit;

        /// <summary>
        /// Control unit filing the request
        /// </summary>
        public ControlUnit OriginControlUnit
        {
            get
            {
                return _originControlUnit;
            }
        } // end of OriginControlUnit

        #endregion OriginControlUnit
    } // end of RequestMoveActivities
}
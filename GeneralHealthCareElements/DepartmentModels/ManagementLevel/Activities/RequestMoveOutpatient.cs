using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Management
{
    public class RequestMoveOutpatient : RequestMovePatientActivities
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
        public RequestMoveOutpatient(Entity[] origin,
                                    DateTime time,
                                    EntityPatient patient,
                                    ControlUnit originalControlUnit,
                                    Admission admission)
            : base("ActivityMove", origin, time, patient, originalControlUnit)
        {

            _outpatientAdmission = admission;
        } // end region

        #endregion

        #region OutpatientAdmission

        private Admission _outpatientAdmission;

        /// <summary>
        /// Admission type
        /// </summary>
        public Admission OutpatientAdmission
        {
            get
            {
                return _outpatientAdmission;
            }
        } // end of OutpatientAdmission

        #endregion

    } // end of RequestMoveOutpatient
}

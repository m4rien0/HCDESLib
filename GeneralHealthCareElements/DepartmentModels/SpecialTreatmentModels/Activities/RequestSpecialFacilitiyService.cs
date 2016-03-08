using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Request that is filed by other departments to request a special service
    /// </summary>
    public class RequestSpecialFacilitiyService : IDelegate
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="originControlUnit">Control unit that filed request</param>
        /// <param name="patient">Patient associated with request</param>
        /// <param name="time">Time request was filed</param>
        /// <param name="type">Type of service requested</param>
        public RequestSpecialFacilitiyService(ControlUnit originControlUnit,
            EntityPatient patient,
            DateTime time,
            SpecialServiceAdmissionTypes type)
        {
            _specialFacilityAdmissionTypes = type;
            _patient = patient;
            _originControlUnit = originControlUnit;
        } // end of RequestDiagnosticTreatment

        #endregion

        #region OriginControlUnit

        private ControlUnit _originControlUnit;

        /// <summary>
        /// Control unit that filed request
        /// </summary>
        public ControlUnit OriginControlUnit
        {
            get
            {
                return _originControlUnit;
            }
            set
            {
                _originControlUnit = value;
            }
        } // end of OriginControlUnit

        #endregion

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient associated with request
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
            set
            {
                _patient = value;
            }
        } // end of Patient

        #endregion

        #region SpecialFacilityAdmissionTypes

        private SpecialServiceAdmissionTypes _specialFacilityAdmissionTypes;

        /// <summary>
        /// Type of service requested
        /// </summary>
        public SpecialServiceAdmissionTypes SpecialFacilityAdmissionTypes
        {
            get
            {
                return _specialFacilityAdmissionTypes;
            }
        } // end of DiagnosticTreatment

        #endregion

    } // end of ActivityRequest
}

using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.TreatmentAdmissionTypes
{

    #region InpatientTreatmentTypes

    public class InpatientTreatmentTypes : TreatmentType
    {
        #region Constructor

        public InpatientTreatmentTypes(string identifier, bool isEmergency)
            : base(TreatmentTypeClass.Inpatient, identifier)
        {
            _isEmergencyTreatment = isEmergency;
        } // end of InpatientTreatmentTypes

        #endregion

        #region IsEmergencyTreatment

        private bool _isEmergencyTreatment;

        public bool IsEmergencyTreatment
        {
            get
            {
                return _isEmergencyTreatment;
            }
        } // end of IsEmergencyTreatment

        #endregion

    } // end of InpatientTreatmentTypes

    #endregion

}

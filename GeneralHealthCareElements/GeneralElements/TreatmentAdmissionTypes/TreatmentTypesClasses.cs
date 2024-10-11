using Enums;

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

        #endregion Constructor

        #region IsEmergencyTreatment

        private bool _isEmergencyTreatment;

        public bool IsEmergencyTreatment
        {
            get
            {
                return _isEmergencyTreatment;
            }
        } // end of IsEmergencyTreatment

        #endregion IsEmergencyTreatment
    } // end of InpatientTreatmentTypes

    #endregion InpatientTreatmentTypes
}
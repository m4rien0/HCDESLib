using Enums;

namespace GeneralHealthCareElements.TreatmentAdmissionTypes
{
    #region OutpatientAdmissionTypes

    /// <summary>
    /// Defines an admission type for outpatient departments
    /// </summary>
    public class OutpatientAdmissionTypes : AdmissionType
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="identifier">Identifier of admission</param>
        public OutpatientAdmissionTypes(string identifier)
            : base(AdmissionTypeClass.Outpatient, identifier)
        {
        } // end of OutpatientAdmissionTypes

        #endregion Constructor
    } // end of OutpatientAdmissionTypes

    #endregion OutpatientAdmissionTypes

    #region InpatientAdmissionTypes

    /// <summary>
    /// Defines an admission type for inpatient departments
    /// </summary>
    public class InpatientAdmissionTypes : AdmissionType
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, allows whole admission types to be extern
        /// </summary>
        /// <param name="identifier">Identifier of admission</param>
        public InpatientAdmissionTypes(string identifier, bool isExtern)
            : base(AdmissionTypeClass.Inpatient, identifier)
        {
            _isExtern = isExtern;
        } // end of InpatientAdmissionTypes

        public InpatientAdmissionTypes()
            : base(AdmissionTypeClass.Inpatient, "")
        {
            _isExtern = true;
        } // end of InpatientAdmissionTypes

        #endregion Constructor

        #region IsExtern

        private bool _isExtern;

        public bool IsExtern
        {
            get
            {
                return _isExtern;
            }
        } // end of IsExtern

        #endregion IsExtern
    } // end of InpatientAdmissionTypes

    #endregion InpatientAdmissionTypes

    #region SpecialFacilityAdmissionTypes

    /// <summary>
    /// Defines an admission type for special service departments
    /// </summary>
    public class SpecialServiceAdmissionTypes : AdmissionType
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="identifier">Identifier of admission</param>
        public SpecialServiceAdmissionTypes(string identifier)
            : base(AdmissionTypeClass.SpecialService, identifier)
        {
        } // end of InpatientAdmissionTypes

        #endregion Constructor
    } // end of InpatientAdmissionTypes

    #endregion SpecialFacilityAdmissionTypes
}

namespace Enums
{
    #region TreatmentTypeClass

    /// <summary>
    /// Defines treatment types of different department base models
    /// </summary>
    public enum TreatmentTypeClass
    {
        Emergency,
        Inpatient,
        Outpatient,
        SpecialFacilityTreatment
    } // end of TreatmentTypeClass

    #endregion

    #region SpecialTreatmentBookingTypes

    /// <summary>
    /// Special service booking types, either a actual booking or default.immediate booking
    /// </summary>
    public enum SpecialServiceBookingTypes
    {
        Immediate,
        Date
    } // end of SpecialServiceBookingTypes

    #endregion

    #region ControlUnitType

    /// <summary>
    /// Defines control unit types of different base department models and sub-units
    /// </summary>
    public enum ControlUnitType
    { 
        Emergency,
        Inpatient,
        Outpatient,
        SpecialFacilityModel,
        Management,
        OutpatientWaitingList,
        InpatientWaitingList,
        OrganizationalUnit
    } // end of ControlUnitType

    #endregion

    #region AssignmentType

    /// <summary>
    /// Describes how resources are assigned to organizational units
    /// </summary>
    public enum AssignmentType
    {
        Shared,
        Fixed
    } // end of AssignmentType

    #endregion

    #region AdmissionTypeClass

    /// <summary>
    /// Defines the base type of an admission, i.e. for which deparmtne type the admission is made
    /// </summary>
    public enum AdmissionTypeClass
    {
        Inpatient,
        Outpatient,
        SpecialService

    } // end of TreatmentTypeClass

    #endregion
}

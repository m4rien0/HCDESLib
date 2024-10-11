using GeneralHealthCareElements.Entities;

namespace GeneralHealthCareElements.TreatmentAdmissionTypes
{
    /// <summary>
    /// Class for an admission to a department model that supports admissions
    /// </summary>
    public class Admission
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="patient">Patient to be admitted</param>
        /// <param name="admissionType">The type of admission, e.g. follow up</param>
        /// <param name="minDaySpan">The minimum number of days before patient can be admitted</param>
        /// <param name="mayDaySpan">The maximum number of days before a patient should/must be admitted</param>
        /// <param name="isExtern">Flag if the admission is generated within the model, e.g. emergency patient admitted to an outpatient clinic</param>
        /// <param name="correspondingDoctor">Admissions can be associated with a doctor to ensure the patient is seen by the same doctor again</param>
        public Admission(
            EntityPatient patient,
            AdmissionType admissionType,
            double minDaySpan = 0,
            double mayDaySpan = double.MaxValue,
            bool isExtern = false,
            EntityDoctor correspondingDoctor = null)
        {
            _patient = patient;
            _admissionType = admissionType;
            _minDaySpan = minDaySpan;
            _maxDaySpan = mayDaySpan;
            _isExtern = isExtern;
            _correspondingDoctor = correspondingDoctor;
        } // end of OutpatientAdmission

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region IsExtern

        private bool _isExtern;

        /// <summary>
        /// Flag if the admission is generated within the model, e.g. emergency patient admitted to an outpatient clinic
        /// </summary>
        public bool IsExtern
        {
            get
            {
                return _isExtern;
            }
            set
            {
                _isExtern = value;
            }
        } // end of IsExtern

        #endregion IsExtern

        #region AdmissionType

        private AdmissionType _admissionType;

        /// <summary>
        /// The type of admission, e.g. follow up
        /// </summary>
        public AdmissionType AdmissionType
        {
            get
            {
                return _admissionType;
            }
        } // end of AdmissionType

        #endregion AdmissionType

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient to be admitted
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion Patient

        #region CorrespondingDoctor

        private EntityDoctor _correspondingDoctor;

        /// <summary>
        /// Admissions can be associated with a doctor to ensure the patient is seen by the same doctor again
        /// </summary>
        public EntityDoctor CorrespondingDoctor
        {
            get
            {
                return _correspondingDoctor;
            }
            set
            {
                _correspondingDoctor = value;
            }
        } // end of CorrespondingDoctor

        #endregion CorrespondingDoctor

        #region MinDaySpan

        private double _minDaySpan;

        /// <summary>
        /// The minimum number of days before patient can be admitted
        /// </summary>
        public double MinDaySpan
        {
            get
            {
                return _minDaySpan;
            }
        } // end of MinDaySpan

        #endregion MinDaySpan

        #region MaxDaySpan

        private double _maxDaySpan;

        /// <summary>
        /// The maximum number of days before a patient should/must be admitted
        /// </summary>
        public double MaxDaySpan
        {
            get
            {
                return _maxDaySpan;
            }
        } // end of MaxDaySpan

        #endregion MaxDaySpan
    } // end of OutpatientAdmission
}
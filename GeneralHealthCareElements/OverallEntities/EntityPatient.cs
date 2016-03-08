using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.SpecialFacility;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Entities
{
   
    /// <summary>
    /// Class for patient entity
    /// </summary>
    public class EntityPatient : ActiveEntityWithSkill
    {
        public static int RunningPatientID = 0;

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">Patient ID</param>
        /// <param name="patientClass">Patient class of patient</param>
        public EntityPatient(int ID, PatientClass patientClass) 
            : base(ID)
        {
            _patientClass = patientClass;
            _correspondingDoctor = null;
            _staysInBed = false;

            DataEntries.Add("TotalWait", 0d);
        } // end of EntityPatient

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region PatientClass

        private PatientClass _patientClass;

        /// <summary>
        /// Corresponding patient class
        /// </summary>
        public PatientClass PatientClass
        {
            get
            {
                return _patientClass;
            }
        } // end of PatientClass

        #endregion
   
        #region BaseControlUnit

        private ControlUnit _baseControlUnit;

        /// <summary>
        /// Base control unit of patient, e.g. the control unit the patient returns to after
        /// a diagnostic assessment
        /// </summary>
        public ControlUnit BaseControlUnit
        {
            get
            {
                return _baseControlUnit;
            }
            set
            {
                _baseControlUnit = value;
            }
        } // end of BaseControlUnit

        #endregion

        #region CorrespondingDoctor

        private EntityDoctor _correspondingDoctor;

        /// <summary>
        /// Doctor responsible for the patient
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

        #endregion       
        
        #region CorrespondingNurse

        private EntityNurse _correspondingNurse;

        /// <summary>
        /// Nurse responsible for the patient
        /// </summary>
        public EntityNurse CorrespondingNurse
        {
            get
            {
                return _correspondingNurse;
            }
            set
            {
                _correspondingNurse = value;
            }
        } // end of CorrespondingNurse

        #endregion

        #region OccupiedFacility

        private EntityTreatmentFacility _occupiedFacility;

        /// <summary>
        /// Facility that is occupied by the patient,
        /// even if not currently in the facility
        /// </summary>
        public EntityTreatmentFacility OccupiedFacility
        {
            get
            {
                return _occupiedFacility;
            }
            set
            {
                _occupiedFacility = value;
            }
        } // end of OccupiedFacility

        #endregion

        #region EmergencyTreatmentPath

        private EmergencyPatientPath _emergencyTreatmentPath;

        /// <summary>
        /// Path patient takes in a emergency department
        /// </summary>
        public EmergencyPatientPath EmergencyTreatmentPath
        {
            get
            {
                return _emergencyTreatmentPath;
            }
            set
            {
                _emergencyTreatmentPath = value;
            }
        } // end of EmergencyTreatmentPath

        #endregion

        #region SpecialFacilityPath

        private SpecialServicePatientPath _specialFacilityPath;

        /// <summary>
        /// Path patient takes in a special service department
        /// </summary>
        public SpecialServicePatientPath SpecialFacilityPath
        {
            get
            {
                return _specialFacilityPath;
            }
            set
            {
                _specialFacilityPath = value;
            }
        } // end of SpecialFacilityPath

        #endregion

        #region OutpatientTreatmentPath
        
        private OutpatientPath _outpatientTreatmentPath;

        /// <summary>
        /// Path patient takes in a outpatient department
        /// </summary>
        public OutpatientPath OutpatientTreatmentPath
        {
          get
          {
            return _outpatientTreatmentPath;
          }
          set
          {
            _outpatientTreatmentPath = value;
          }
        } // end of OutpatientTreatmentPath
        
        #endregion

        #region StaysInBed

        private bool _staysInBed;

        public bool StaysInBed
        {
            get
            {
                return _staysInBed;
            }
            set
            {
                _staysInBed = value;
            }
        } // end of StaysInBed

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Patient: " + Identifier.ToString();
        } // end of

        #endregion

        #region Clone

        public override Entity Clone()
        {
            EntityPatient newPatient = new EntityPatient(Identifier, PatientClass);
            newPatient.CorrespondingDoctor = CorrespondingDoctor;

            return newPatient;
        } // end of Clone

        #endregion

        #region ResetID

        public static void ResetID()
        {
            RunningPatientID = 0;
        } // end of ResetID

        #endregion

    } // end of EntityPatient
}

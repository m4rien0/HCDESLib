using Enums;
using GeneralHealthCareElements.ControlUnits;
using SimulationCore.HCCMElements;
using System.Windows;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Basic entity for a treatment facility
    /// </summary>
    public class EntityTreatmentFacility : EntityWithSkill
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">Entity ID</param>
        /// <param name="skillSet">Skill set defining the facility</param>
        /// <param name="type">Facility type</param>
        /// <param name="position">Position within structural layout of parent structural area</param>
        /// <param name="size">Size within structural layout of parent structural area</param>
        public EntityTreatmentFacility(int ID, SkillSet skillSet, string type = "General", Point position = new Point(), Size size = new Size())
            : base(ID, skillSet)
        {
            _occupied = false;
            _position = position;
            _size = size;
            _facilityType = type;
        } // end of EntityTreatmentBooth

        #endregion Constructor

        #region Position

        private Point _position;

        /// <summary>
        /// Position within structural layout of parent structural area
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
        } // end of Position

        #endregion Position

        #region Size

        private Size _size;

        /// <summary>
        /// Size within structural layout of parent structural area
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }
        } // end of Size

        #endregion Size

        #region ParentDepartmentControl

        private ControlUnitHealthCareDepartment _parentDepartmentControl;

        /// <summary>
        /// Parent department of facility
        /// </summary>
        public ControlUnitHealthCareDepartment ParentDepartmentControl
        {
            get
            {
                return _parentDepartmentControl;
            }
        } // end of ParentDepartmentControl

        #endregion ParentDepartmentControl

        #region SetParentDepartmentControl

        /// <summary>
        /// Sets the parent department control
        /// </summary>
        /// <param name="parentDepartment"></param>
        public void SetParentDepartmentControl(ControlUnitHealthCareDepartment parentDepartment)
        {
            _parentDepartmentControl = parentDepartment;
        } // end of SetParentDepartmentControl

        #endregion SetParentDepartmentControl

        #region FacilityType

        private string _facilityType;

        /// <summary>
        /// String to represent the facility type
        /// </summary>
        public string FacilityType
        {
            get
            {
                return _facilityType;
            }
            set
            {
                _facilityType = value;
            }
        } // end of FacilityType

        #endregion FacilityType

        #region Occupied

        private bool _occupied;

        /// <summary>
        /// Flag if facility is currently occupied by a patient
        /// </summary>
        public bool Occupied
        {
            get
            {
                return _occupied;
            }
            set
            {
                _occupied = value;
            }
        } // end of Occupied

        #endregion Occupied

        #region BlockedForPatient

        /// <summary>
        /// Flag if facility is not occupied but blocked for a patient
        /// </summary>
        public bool BlockedForPatient
        {
            get
            {
                return _patientBlocking != null;
            }
        } // end of BlockedForPatient

        #endregion BlockedForPatient

        #region PatientBlocking

        private EntityPatient _patientBlocking;

        /// <summary>
        /// Patient the facility is blocked for
        /// </summary>
        public EntityPatient PatientBlocking
        {
            get
            {
                return _patientBlocking;
            }
            set
            {
                _patientBlocking = value;
            }
        } // end of PatientBlocking

        #endregion PatientBlocking

        #region AssignmentType

        private AssignmentType _assignmentType;

        /// <summary>
        /// Assignement type with which the facility is assigned to a organizational unit
        /// </summary>
        public AssignmentType AssignmentType
        {
            get
            {
                return _assignmentType;
            }
            set
            {
                _assignmentType = value;
            }
        } // end of AssignmentType

        #endregion AssignmentType

        #region CurrentlyAssignedOrganizationalUnit

        private ControlUnitOrganizationalUnit _currentlyAssignedOrganizationalUnit;

        /// <summary>
        /// The organizational control the facility is currently assigned to, parent control
        /// is always the root department control
        /// </summary>
        public ControlUnitOrganizationalUnit CurrentlyAssignedOrganizationalUnit
        {
            get
            {
                return _currentlyAssignedOrganizationalUnit;
            }
            set
            {
                _currentlyAssignedOrganizationalUnit = value;
            }
        } // end of CurrentlyAssignedOrganizationalUnit

        #endregion CurrentlyAssignedOrganizationalUnit

        #region ToString

        public override string ToString()
        {
            return FacilityType + ": " + Identifier.ToString();
        } // end of

        #endregion ToString

        #region Clone

        public override Entity Clone()
        {
            EntityTreatmentFacility newBooth = new EntityTreatmentFacility(Identifier, SkillSet);
            newBooth.Occupied = Occupied;
            return newBooth;
        } // end of Clone

        #endregion Clone
    } // end of EntitiyTreatmentFacility
}
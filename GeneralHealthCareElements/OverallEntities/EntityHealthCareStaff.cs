using Enums;
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Summarizes functionality used by doctors, nurses and possible other future health care staff
    /// </summary>
    abstract public class EntityHealthCareStaff : EntityStaff
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="id">ID of staff</param>
        /// <param name="skillSet">Skill of staff</param>
        /// <param name="assignmentType">Assignment type of staff to organizational unit</param>
        public EntityHealthCareStaff(int id, SkillSet skillSet, AssignmentType assignmentType = AssignmentType.Fixed)
            : base(id, skillSet)
        {
            _currentPatients = new List<EntityPatient>();
            _staffOutsideShift = true;
            _assignmentType = assignmentType;
        } // end of EntityDoctor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region AssignmentType

        private AssignmentType _assignmentType;

        /// <summary>
        /// Assignment type of staff to organizational unit
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

        #endregion

        #region CurrentPatients

        private List<EntityPatient> _currentPatients;

        /// <summary>
        /// List of patients that is currently handled by staff
        /// </summary>
        public IReadOnlyList<EntityPatient> CurrentPatients
        {
            get
            {
                return _currentPatients;
            }
        } // end of CurrentPatients

        #endregion

        #region StaffOutsideShift

        private bool _staffOutsideShift;

        /// <summary>
        /// Flag that indiciates if shift of staff has already ended, to avoid further dispatching
        /// </summary>
        public bool StaffOutsideShift
        {
            get
            {
                return _staffOutsideShift;
            }
            set
            {
                _staffOutsideShift = value;
            }
        } // end of StaffOutsideShift

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region AddPatient

        /// <summary>
        /// Adds a patient to the current patient list
        /// </summary>
        /// <param name="patient">Patient to add</param>
        public void AddPatient(EntityPatient patient)
        {
            _currentPatients.Add(patient);
        } // end of AddPatient

        #endregion

        #region RemovePatient

        /// <summary>
        /// Removes a patient to the current patient list
        /// </summary>
        /// <param name="patient">Patient to remove</param>
        public void RemovePatient(EntityPatient patient)
        {
            _currentPatients.Remove(patient);
        } // end of RemovePatient

        #endregion

        #region GetPossibleMovingActivity

        /// <summary>
        /// Checks if staff is currently on the move
        /// </summary>
        /// <returns></returns>
        public ActivityMove GetPossibleMovingActivity()
        {
            if (GetCurrentActivities().Count() == 1
                && GetCurrentActivities().First() is ActivityMove)
            {
                return (ActivityMove)GetCurrentActivities().First();
            } // end if

            return null;
        } // end of GetPossibleMovingActivity

        #endregion

    } // end of EntityHealthCareStaff
}

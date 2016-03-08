using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Special patient path class for outpatient paths
    /// </summary>
    public class OutpatientPath : PatientPath<OutpatientActionTypeClass>
    {

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="actions">Action types that form path</param>
        /// <param name="inpatientAdmission">Possible inpatient admission of path</param>
        /// <param name="outpatientAdmission">Possible outpatient admission of path</param>
        /// <param name="patient">Parent patient</param>
        /// <param name="scheduled">Scheduled time of patient arrival</param>
        /// <param name="walkIn">Flag if patient is a walk-in patient</param>
        public OutpatientPath(List<OutpatientActionTypeClass> actions,
                                    Admission inpatientAdmission,
                                    Admission outpatientAdmission,
                                    EntityPatient patient,
                                    DateTime scheduled,
                                    bool walkIn)
            :base(actions, patient)
        {
            _inpatientAdmission = inpatientAdmission;
            _outpatientAdmission = outpatientAdmission;
            _walkIn = walkIn;
            _scheduledTime = scheduled;
        } // end of EmergencyPatientPath

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region ScheduledTime

        private DateTime _scheduledTime;

        /// <summary>
        /// Scheduled time of patient arrival
        /// </summary>
        public DateTime ScheduledTime
        {
            get
            {
                return _scheduledTime;
            }
        } // end of ScheduledTime

        #endregion

        #region WalkIn

        private bool _walkIn;

        /// <summary>
        /// Flag is patient is a walk-in
        /// </summary>
        public bool WalkIn
        {
            get
            {
                return _walkIn;
            }
        } // end of WalkIn

        #endregion

        #region InpatientAdmission

        private Admission _inpatientAdmission;

        /// <summary>
        /// Inpatient admission, equals null if patient is not admitted
        /// </summary>
        public Admission InpatientAdmission
        {
            get
            {
                return _inpatientAdmission;
            }
            set
            {
                _inpatientAdmission = value;
            }
        } // end of InpatientAdmission

        #endregion

        #region OutpatientAdmission

        private Admission _outpatientAdmission;

        /// <summary>
        /// Outpatient admission, equals null if patient is not admitted
        /// </summary>
        public Admission OutpatientAdmission
        {
            get
            {
                return _outpatientAdmission;
            }
            set
            {
                _outpatientAdmission = value;
            }
        } // end of OutpatientAdmission

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region TakeNextAction

        /// <summary>
        /// Action taking along path. Different actions for diagnostics, leaving and
        /// actions within the outpatient department
        /// </summary>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <param name="patient">Patient of path</param>
        /// <param name="currentEvent">Event after which next action is taken</param>
        /// <param name="time">Time next action is taken</param>
        /// <param name="parentControlUnit">Control unit that hosts the next action</param>
        /// <returns>Returns true if Patient goes in an waiting activitiy before next action, else false </returns>
        public override bool TakeNextAction(ISimulationEngine simEngine,
            Event currentEvent,
            DateTime time,
            ControlUnit parentControlUnit)
        {
            ControlUnit parentDepartmentControl = parentControlUnit;

            if (parentControlUnit is ControlUnitOrganizationalUnit)
                parentDepartmentControl = ((ControlUnitOrganizationalUnit)parentControlUnit).ParentDepartmentControl;

            if (GetCurrentActionType().Type == "Diagnostics")
            {
                RequestSpecialFacilitiyService reqeustForDiagnostics
                        = new RequestSpecialFacilitiyService(parentDepartmentControl,
                            ParentPatient,
                            time,
                            new SpecialServiceAdmissionTypes(GetCurrentActionType().Identifier));
                parentDepartmentControl.DelegateOutBox.Add(reqeustForDiagnostics);
                return false;
            } // end if

            if (GetCurrentActionType().Type == "Leave")
            {
                EventOutpatientPatientLeave patientLeave = new EventOutpatientPatientLeave(parentDepartmentControl, ParentPatient);
                currentEvent.SequentialEvents.Add(patientLeave);
                return false;
            } // end if

            OutpatientActionTypeClass currentType = GetCurrentActionType();

            RequestOutpatientAction req =
                new RequestOutpatientAction(ParentPatient, 
                    0, 
                    currentType, 
                    time,
                    new ResourceSet(ParentPatient.CorrespondingDoctor, ParentPatient.CorrespondingNurse, null), 
                    ScheduledTime, 
                    WalkIn);

            parentControlUnit.AddRequest(req);

            if (currentType.IsHold)
                return false;

            return true;
        } // end of TakeNextAction

        #endregion
       
    } // end of OutpatientPath
}

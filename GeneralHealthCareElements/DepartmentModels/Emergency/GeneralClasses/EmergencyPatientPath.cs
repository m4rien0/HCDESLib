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

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Special patient path class for emergency paths
    /// </summary>
    public class EmergencyPatientPath : PatientPath<EmergencyActionTypeClass>
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="actions">Action types that form path</param>
        /// <param name="inpatientAdmission">Possible inpatient admission of path</param>
        /// <param name="outpatientAdmission">Possible outpatient admission of path</param>
        /// <param name="patient">Parent patient</param>
        public EmergencyPatientPath(List<EmergencyActionTypeClass> actions,
                                    Admission inpatientAdmission,
                                    Admission outpatientAdmission,
                                    EntityPatient patient)
            : base(actions, patient)
        {
            _inpatientAdmission = inpatientAdmission;
            _outpatientAdmission = outpatientAdmission;
        } // end of EmergencyPatientPath

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

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

        #endregion InpatientAdmission

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

        #endregion OutpatientAdmission

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region TakeNextAction

        /// <summary>
        /// Action taking along path. Different actions for diagnostics, leaving and
        /// actions within the emergency department
        /// </summary>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <param name="currentEvent">Event after which next action is taken</param>
        /// <param name="time">Time next action is taken</param>
        /// <param name="parentControlUnit">Control unit that hosts the next action</param>
        /// <returns>Returns true if Patient goes in an waiting activitiy before next action, else false </returns>
        public override bool TakeNextAction(
            ISimulationEngine simEngine,
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
                EventEmergencyPatientLeave patientLeave = new EventEmergencyPatientLeave(parentDepartmentControl, ParentPatient, ((ControlUnitEmergency)parentDepartmentControl).InputData);
                currentEvent.SequentialEvents.Add(patientLeave);
                return false;
            } // end if

            EmergencyActionTypeClass currentType = GetCurrentActionType();

            RequestEmergencyAction req =
                new RequestEmergencyAction(ParentPatient,
                                           0,
                                           currentType,
                                           time,
                                           new ResourceSet(ParentPatient.CorrespondingDoctor,
                                                       ParentPatient.CorrespondingNurse, null));

            parentControlUnit.AddRequest(req);

            if (currentType.IsHold)
                return false;

            return true;
        } // end of TakeNextAction

        #endregion TakeNextAction
    } // end of EmergencyPatientPath
}
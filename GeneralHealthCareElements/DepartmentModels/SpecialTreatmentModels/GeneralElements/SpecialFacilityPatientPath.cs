using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Management;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;

namespace GeneralHealthCareElements.SpecialFacility
{
    /// <summary>
    /// Special patient path class for special service paths
    /// </summary>
    public class SpecialServicePatientPath : PatientPath<SpecialServiceActionTypeClass>
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="actions">Action types that form path</param>
        /// <param name="originalRequest">Service request associated</param>
        /// <param name="patient">Parent patient</param>
        public SpecialServicePatientPath(
            List<SpecialServiceActionTypeClass> actions,
            RequestSpecialFacilitiyService originalRequest,
            EntityPatient patient)
            : base(actions, patient)
        {
            _originalRequest = originalRequest;
        } // end of EmergencyPatientPath

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region OriginalRequest

        private RequestSpecialFacilitiyService _originalRequest;

        /// <summary>
        /// Service request associated
        /// </summary>
        public RequestSpecialFacilitiyService OriginalRequest
        {
            get
            {
                return _originalRequest;
            }
        } // end of OriginalRequest

        #endregion OriginalRequest

        #region TakeNextAction

        /// <summary>
        /// Action taking along path. Different actions for return and
        /// actions within the special service department
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

            if (GetCurrentActionType().Type == "Return")
            {
                ControlUnitManagement jointControl = (ControlUnitManagement)parentControlUnit.FindSmallestJointControl(OriginalRequest.OriginControlUnit);

                ActivityMove movePatientBack = new ActivityMove(parentControlUnit,
                    ParentPatient,
                    parentDepartmentControl,
                    OriginalRequest.OriginControlUnit,
                    OriginalRequest,
                    jointControl.InputData.DurationMove(ParentPatient,
                                                        parentControlUnit,
                                                        OriginalRequest.OriginControlUnit));

                movePatientBack.StartEvent.Trigger(time, simEngine);

                return false;
            } // end if

            SpecialServiceActionTypeClass currentType = GetCurrentActionType();

            RequestSpecialFacilityAction req =
                new RequestSpecialFacilityAction(ParentPatient, 0, currentType, time, new ResourceSet());
            parentControlUnit.AddRequest(req);

            return true;
        } // end if TakeNextAction

        #endregion TakeNextAction
    } // end of SpecialFacilityPatientPath
}
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleHospitalModel.Emergency
{
    /// <summary>
    /// Organizational control unit that handles all requests for triage and register
    /// </summary>
    public class ControlUnitEmergencyRegisterTriage : ControlUnitOrganizationalUnit
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control, either a department control or another organizational control</param>
        /// <param name="parentDepartmentControl">Parent department control</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Parent department input</param>
        public ControlUnitEmergencyRegisterTriage(string name,
                           ControlUnit parentControlUnit,
                           ControlUnitHealthCareDepartment parentDepartmentControl,
                           SimulationModel parentSimulationModel,
                           IInputHealthCareDepartment inputData)
            : base(name, parentControlUnit, parentDepartmentControl, parentSimulationModel, inputData)
        {
        } // end of

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Custom rules of control, assigns all requests in specified order, patient priority and FIFO principle is
        /// used for requests of same type
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            #region Register

            List<RequestEmergencyAction> registerRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Register").Cast<RequestEmergencyAction>());

            while (registerRequests.Count > 0)
            {
                // Get Register request Triage-FIFO
                RequestEmergencyAction requestRegister = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(registerRequests);

                ResourceSet chosenResources;

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    requestRegister,
                    out chosenResources))
                {
                    break;
                } // enf if

                RemoveRequest(requestRegister);
                registerRequests.Remove(requestRegister);
                EntityPatient patient = (EntityPatient)requestRegister.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> register = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestRegister.ActionType,
                    patient.EmergencyTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                register.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion Register

            #region Triage

            List<RequestEmergencyAction> triageRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Triage").Cast<RequestEmergencyAction>());

            while (triageRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestEmergencyAction requestTriage = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(triageRequests);

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    requestTriage,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(requestTriage);
                triageRequests.Remove(requestTriage);
                EntityPatient patient = (EntityPatient)requestTriage.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> triage = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestTriage.ActionType,
                    patient.EmergencyTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                triage.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion Triage

            return false;
        } // end of PerformCustomRules

        #endregion PerformCustomRules
    } // end of ControlUnitRegisterTriage
}
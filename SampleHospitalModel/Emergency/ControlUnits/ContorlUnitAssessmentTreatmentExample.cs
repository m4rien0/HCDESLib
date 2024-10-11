using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Delegates;
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
    /// Organizational control unit that handles all requests for assessment or treatment
    /// </summary>
    public class ContorlUnitAssessmentTreatmentExample : ControlUnitOrganizationalUnit
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
        public ContorlUnitAssessmentTreatmentExample(string name,
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
            #region Consultation

            List<RequestEmergencyAction> consultationRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Consultation").Cast<RequestEmergencyAction>());

            List<RequestEmergencyAction> possibleConsultationRequests = new List<RequestEmergencyAction>();

            #region CheckForControllingAllRequiredResources

            foreach (RequestEmergencyAction request in consultationRequests)
            {
                if (request.ReadyForDispatch)
                {
                    possibleConsultationRequests.Add(request);
                }
                else
                {
                    List<SkillSet> nonAvailableDoctorSkills = ((ControlUnitEmergencyExample)ParentDepartmentControl).CheckAvailabilityOfDoctors(request.ActionType.MainDoctorRequirements, request.ActionType.AssistingDoctorRequirements);

                    if (nonAvailableDoctorSkills.Count() == 0)
                    {
                        possibleConsultationRequests.Add(request);
                        request.ReadyForDispatch = true;
                    }
                    else
                    {
                        if (nonAvailableDoctorSkills.Count > 0 && !request.StaffRequested)
                            ((ControlUnitEmergencyExample)ParentDepartmentControl).DelegateOutBox.Add(new DelegateRequestDocsForAssisting(this, nonAvailableDoctorSkills));
                        request.StaffRequested = true;
                    } // end if
                } // end if
            } // end foreach

            #endregion CheckForControllingAllRequiredResources

            while (possibleConsultationRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestEmergencyAction requestConsultation = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(possibleConsultationRequests);

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    requestConsultation,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(requestConsultation);
                possibleConsultationRequests.Remove(requestConsultation);
                EntityPatient patient = (EntityPatient)requestConsultation.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> consultation = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestConsultation.ActionType,
                    patient.EmergencyTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                consultation.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion Consultation

            #region BedPlacement

            List<RequestEmergencyAction> bedPlacementRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "BedPlacement").Cast<RequestEmergencyAction>());

            while (bedPlacementRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestEmergencyAction bedPlacementRequest = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(bedPlacementRequests);

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    bedPlacementRequest,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(bedPlacementRequest);
                bedPlacementRequests.Remove(bedPlacementRequest);
                EntityPatient patient = (EntityPatient)bedPlacementRequest.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> bedPlacement = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    bedPlacementRequest.ActionType,
                    patient.EmergencyTreatmentPath
                    );

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                bedPlacement.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion BedPlacement

            #region Assessment

            List<RequestEmergencyAction> assessmentRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Assessment").Cast<RequestEmergencyAction>());

            while (assessmentRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestEmergencyAction requestAssessment = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(assessmentRequests);

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    requestAssessment,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(requestAssessment);
                assessmentRequests.Remove(requestAssessment);
                EntityPatient patient = (EntityPatient)requestAssessment.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> assessment = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestAssessment.ActionType,
                    patient.EmergencyTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                assessment.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion Assessment

            #region Treatment

            List<RequestEmergencyAction> treatmentRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestEmergencyAction)p).ActionType.Type == "Treatment").Cast<RequestEmergencyAction>());

            while (treatmentRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestEmergencyAction requestTreatment = PatientPriorityPlusFIFO<RequestEmergencyAction, EmergencyActionTypeClass>(treatmentRequests);

                if (!((ControlUnitEmergencyExample)ParentDepartmentControl).ChooseResourcesForAction(
                    ControlledDoctors,
                    ControlledNurses,
                    AssignedTreatmentFacilities,
                    requestTreatment,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list and treatmentlist
                RemoveRequest(requestTreatment);
                treatmentRequests.Remove(requestTreatment);
                EntityPatient patient = (EntityPatient)requestTreatment.Origin.First();

                ActivityHealthCareAction<EmergencyActionTypeClass> assessment = new ActivityHealthCareAction<EmergencyActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestTreatment.ActionType,
                    patient.EmergencyTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                assessment.StartEvent.Trigger(time, simEngine);
            } // end while

            #endregion Treatment

            return false;
        } // end of PerformCustomRules

        #endregion PerformCustomRules
    } // end of ContorlUnitAssessmentTreatmentExample
}
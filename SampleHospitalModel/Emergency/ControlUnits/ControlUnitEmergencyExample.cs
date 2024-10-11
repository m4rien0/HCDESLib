using Enums;
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Management;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.StaffHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleHospitalModel.Emergency
{
    /// <summary>
    /// Sample control unit implementing a basic emergency department including surgical and internal units as
    /// well as triage and register facilities
    /// </summary>
    public class ControlUnitEmergencyExample : ControlUnitEmergency
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, just calling base constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent management control unit</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Emergency input data</param>
        public ControlUnitEmergencyExample(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            InputEmergency input)
            : base(name, parentControlUnit, parentSimulationModel, input)
        {
        } // end of ControlUnit

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Initializes patient stream
        /// </summary>
        /// <param name="startTime">Start time of the simulation model</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            EntityPatient patient = InputData.GetNextPatient();

            EventEmergencyPatientArrival firstPatient = new EventEmergencyPatientArrival(this, patient, InputData);

            simEngine.AddScheduledEvent(firstPatient, startTime + InputData.PatientArrivalTime(startTime));
        } // end of Initialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Custom rules of emergency control, just calls dispatching and control method
        /// as long as no further event is triggered
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            bool eventLaunched = false;

            bool newEventLauchned = true;

            while (newEventLauchned)
            {
                if (PerformDisptatching(time, simEngine))
                {
                    eventLaunched = true;
                    continue;
                } // end if

                if (PerformControlled(time, simEngine))
                {
                    eventLaunched = true;
                    continue;
                } // end if

                newEventLauchned = false;
            } // end while

            return eventLaunched;
        } // end of PerformAssessment

        #endregion PerformCustomRules

        #region PerformDisptatching

        /// <summary>
        /// Handles requests of staff members to leave that are out of shift, routes action requests
        /// to corresponding organizational control units and handles a shared treatment facility by
        /// assigning it to the busier organizational unit
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected bool PerformDisptatching(DateTime time, ISimulationEngine simEngine)
        {
            #region StaffOutsideShift

            List<RequestBeAbsent> staffEndRequests = RAEL.Where(p => p is RequestBeAbsent).Cast<RequestBeAbsent>().ToList();

            foreach (RequestBeAbsent req in staffEndRequests)
            {
                if (req.StaffMember.IsWaiting() && req.StaffMember.CurrentPatients.Count == 0)
                {
                    req.StaffMember.StopCurrentActivities(time, simEngine);
                    EventStaffLeave staffLeave = new EventStaffLeave(req.StaffMember.ParentControlUnit, req.StaffMember);

                    staffLeave.Trigger(time, simEngine);

                    RemoveRequest(req);
                } // end if
            } // end foreach

            #endregion StaffOutsideShift

            #region RequestRouting

            List<RequestEmergencyAction> actionRequests = new List<RequestEmergencyAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction").Cast<RequestEmergencyAction>());

            foreach (RequestEmergencyAction request in actionRequests)
            {
                if (request.ActionType.Type == "Triage" || request.ActionType.Type == "Register")
                {
                    OrganizationalUnitPerName["OrgUnitTriageRegister"].AssignRequest(request);
                }
                else
                {
                    if (request.Patient.PatientClass.Category == "Surgical")
                        OrganizationalUnitPerName["OrgUnitSurgical"].AssignRequest(request);
                    else
                        OrganizationalUnitPerName["OrgUnitInternal"].AssignRequest(request);
                } // end if

                RemoveRequest(request);
            } // end foreach

            #endregion RequestRouting

            #region ResourceSharing

            foreach (EntityTreatmentFacility sharedTreatFac in AssignedTreatmentFacilities.Where(p => p.AssignmentType == AssignmentType.Shared))
            {
                if (sharedTreatFac.PatientBlocking != null)
                    continue;

                if (sharedTreatFac.CurrentlyAssignedOrganizationalUnit != null)
                    sharedTreatFac.CurrentlyAssignedOrganizationalUnit.RemoveAssignedTreatmentFacility(sharedTreatFac);

                if (OrganizationalUnitPerName["OrgUnitInternal"].RAEL.Count > OrganizationalUnitPerName["OrgUnitSurgical"].RAEL.Count)
                {
                    OrganizationalUnitPerName["OrgUnitSurgical"].AddAssignedTreatmentFacility(sharedTreatFac);
                }
                else
                {
                    OrganizationalUnitPerName["OrgUnitInternal"].AddAssignedTreatmentFacility(sharedTreatFac);
                } // end if
            } // end foreach

            #endregion ResourceSharing

            return false;
        } // end of PerformDisptatching

        #endregion PerformDisptatching

        #region PerformControlled

        /// <summary>
        /// Sends assisting staff members that are not further required back to their origin
        /// control unit
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected bool PerformControlled(DateTime time, ISimulationEngine simEngine)
        {
            IEnumerable<EntityDoctor> assistingDocs = HandledDoctors.Where(p => p.BaseControlUnit != this);
            IEnumerable<RequestEmergencyAction> assistedRequests = RAEL.Where(p => p.GetType() == typeof(RequestEmergencyAction) && ((RequestEmergencyAction)p).IsAssistedDoctor).Cast<RequestEmergencyAction>().ToList();

            bool moveTriggered = false;

            foreach (EntityDoctor doc in assistingDocs)
            {
                bool skillRequired = false;

                if (doc.IsWaiting())
                {
                    foreach (RequestEmergencyAction req in assistedRequests)
                    {
                        foreach (SkillSet skillSet in req.ActionType.AssistingDoctorRequirements)
                        {
                            if (doc.SatisfiesSkillSet(skillSet))
                                skillRequired = true;
                        } // end foreach
                    } // end foreach

                    if (!skillRequired)
                    {
                        ControlUnitManagement jointControl = (ControlUnitManagement)FindSmallestJointControl(doc.BaseControlUnit);
                        ActivityMove moveBack = new ActivityMove(jointControl, doc, this, doc.BaseControlUnit, null, jointControl.InputData.DurationMove(doc, this, doc.BaseControlUnit));
                        doc.StopCurrentActivities(time, simEngine);
                        moveBack.StartEvent.Trigger(time, simEngine);
                        moveTriggered = true;
                    } // end if
                } // end if
            } // end foreach

            return moveTriggered;
        } // end of PerformControlled

        #endregion PerformControlled

        //--------------------------------------------------------------------------------------------------
        // Custom Mehtods
        //--------------------------------------------------------------------------------------------------

        #region ChooseResourcesForAction

        /// <summary>
        /// Helping method to choose resource for action requests, first available resources
        /// that satisfy requirements are chosen or it is checked if preassigned staff members
        /// (e.g. corresponding doctors) have capacitiy to undertake action
        /// </summary>
        /// <param name="doctorsToChoose">Doctors that can be selected for action</param>
        /// <param name="nursesToChoose">Nurses that can be selected for action</param>
        /// <param name="treatmentFacilitiesToChoose">Treatment facilities that can be selected for action</param>
        /// <param name="emergencyRequest">The request that is the soruce for the action</param>
        /// <param name="resources">Out parameter of chosen resources for action</param>
        /// <returns>True if all resources have been assigned, false else</returns>
        public bool ChooseResourcesForAction(
            IEnumerable<EntityDoctor> doctorsToChoose,
            IEnumerable<EntityNurse> nursesToChoose,
            IEnumerable<EntityTreatmentFacility> treatmentFacilitiesToChoose,
            RequestEmergencyAction emergencyRequest,
            out ResourceSet resources)
        {
            resources = new ResourceSet();

            #region Doctors

            List<EntityDoctor> chosenDoctors = new List<EntityDoctor>();

            #region MainDoc

            if (emergencyRequest.ActionType.MainDoctorRequirements != null)
            {
                if (emergencyRequest.ResourceSet.MainDoctor != null)
                {
                    if (emergencyRequest.ResourceSet.MainDoctor.IsWaiting())
                    {
                        chosenDoctors.Add(resources.MainDoctor);
                        resources.MainDoctor = emergencyRequest.ResourceSet.MainDoctor;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {
                    List<EntityDoctor> possibleDocs = doctorsToChoose.Where(p => p.IsWaiting()
                        && p.SatisfiesSkillSet(emergencyRequest.ActionType.MainDoctorRequirements)
                        && !p.StaffOutsideShift).ToList();

                    if (possibleDocs.Count == 0)
                        return false;

                    resources.MainDoctor = possibleDocs.First();
                    chosenDoctors.Add(resources.MainDoctor);
                } // end if
            } // end if

            #endregion MainDoc

            #region AssistingDoctors

            if (emergencyRequest.ActionType.AssistingDoctorRequirements != null)
            {
                if (emergencyRequest.ResourceSet.AssistingDoctors != null)
                {
                    foreach (EntityDoctor doctor in emergencyRequest.ResourceSet.AssistingDoctors)
                    {
                        if (!doctor.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityDoctor> foundDoctors = new List<EntityDoctor>();

                    List<EntityDoctor> allDoctors = new List<EntityDoctor>(doctorsToChoose);

                    foreach (SkillSet skillSet in emergencyRequest.ActionType.AssistingDoctorRequirements)
                    {
                        EntityDoctor foundDoc = null;

                        foreach (EntityDoctor doc in allDoctors)
                        {
                            if (doc.IsWaiting() &&
                                doc.SatisfiesSkillSet(skillSet)
                                && !chosenDoctors.Contains(doc)
                                && !doc.StaffOutsideShift)
                            {
                                foundDoc = doc;
                                break;
                            } // end if
                        } // end foreach

                        if (foundDoc == null)
                        {
                            return false;
                        }
                        else
                        {
                            allDoctors.Remove(foundDoc);
                            foundDoctors.Add(foundDoc);
                            chosenDoctors.Add(foundDoc);
                        } // end if
                    } // end foreach

                    resources.AssistingDoctors = foundDoctors.ToArray();
                } // end if
            }// end if

            #endregion AssistingDoctors

            #endregion Doctors

            #region Nurses

            List<EntityNurse> chosenNurses = new List<EntityNurse>();

            #region MainNurse

            if (emergencyRequest.ActionType.MainNurseRequirements != null)
            {
                if (emergencyRequest.ResourceSet.MainNurse != null)
                {
                    if (emergencyRequest.ResourceSet.MainNurse.IsWaiting())
                    {
                        chosenNurses.Add(resources.MainNurse);
                        resources.MainNurse = emergencyRequest.ResourceSet.MainNurse;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {
                    List<EntityNurse> possibleDocs = nursesToChoose.Where(p => p.IsWaiting()
                        && p.SatisfiesSkillSet(emergencyRequest.ActionType.MainNurseRequirements)
                        && !p.StaffOutsideShift).ToList();

                    if (possibleDocs.Count == 0)
                        return false;

                    resources.MainNurse = possibleDocs.First();
                    chosenNurses.Add(resources.MainNurse);
                } // end if
            } // end if

            #endregion MainNurse

            #region AssistingNurses

            if (emergencyRequest.ActionType.AssistingNurseRequirements != null)
            {
                if (emergencyRequest.ResourceSet.AssistingNurses != null)
                {
                    foreach (EntityNurse nurse in emergencyRequest.ResourceSet.AssistingNurses)
                    {
                        if (!nurse.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityNurse> foundNurses = new List<EntityNurse>();

                    List<EntityNurse> allNurses = new List<EntityNurse>(nursesToChoose);

                    foreach (SkillSet skillSet in emergencyRequest.ActionType.AssistingNurseRequirements)
                    {
                        EntityNurse foundDoc = null;

                        foreach (EntityNurse nurse in allNurses)
                        {
                            if (nurse.IsWaiting() &&
                                nurse.SatisfiesSkillSet(skillSet)
                                && !chosenNurses.Contains(nurse)
                                && !nurse.StaffOutsideShift)
                            {
                                foundDoc = nurse;
                                break;
                            } // end if
                        } // end foreach

                        if (foundDoc == null)
                        {
                            return false;
                        }
                        else
                        {
                            allNurses.Remove(foundDoc);
                            foundNurses.Add(foundDoc);
                            chosenNurses.Add(foundDoc);
                        } // end if
                    } // end foreach

                    resources.AssistingNurses = foundNurses.ToArray();
                } // end if
            }// end if

            #endregion AssistingNurses

            #endregion Nurses

            #region TreatmentFacilities

            if (emergencyRequest.Patient.OccupiedFacility != null
                && emergencyRequest.Patient.OccupiedFacility.SatisfiesSkillSet(emergencyRequest.ActionType.FacilityRequirements))
            {
                resources.TreatmentFacility = emergencyRequest.Patient.OccupiedFacility;
            }
            else
            {
                bool foundFacility = false;

                foreach (EntityTreatmentFacility fac in treatmentFacilitiesToChoose.Where(p => !p.BlockedForPatient))
                {
                    if (!fac.Occupied && fac.SatisfiesSkillSet(emergencyRequest.ActionType.FacilityRequirements))
                    {
                        resources.TreatmentFacility = fac;
                        foundFacility = true;
                        break;
                    } // end if
                } // end foreach

                if (!foundFacility)
                    return false;
            } // end if

            #endregion TreatmentFacilities

            return true;
        } // end of ChooseResourcesForAction

        #endregion ChooseResourcesForAction

        #region CheckAvailabilityOfDoctors

        /// <summary>
        /// Checks if all skills of doctors are currently controlled by the control unit
        /// </summary>
        /// <param name="mainDocSkill">Skill required for main doctor</param>
        /// <param name="reqAssSkills">Skills required for assisting doctors</param>
        /// <returns>True if all skills are controlled</returns>
        public List<SkillSet> CheckAvailabilityOfDoctors(SkillSet mainDocSkill, SkillSet[] reqAssSkills)
        {
            //--------------------------------------------------------------------------------------------------
            // At the moment it is assumed that the main doctoral skkill set is always available
            //--------------------------------------------------------------------------------------------------
            List<SkillSet> nonAvailableSkillSets = new List<SkillSet>();

            List<EntityDoctor> nonChosenDoctors = new List<EntityDoctor>(ControlledDoctors);

            if (reqAssSkills == null)
                return nonAvailableSkillSets;

            foreach (SkillSet skillSet in reqAssSkills)
            {
                EntityDoctor foundDoc = null;

                foreach (EntityDoctor doc in nonChosenDoctors)
                {
                    if (doc.SatisfiesSkillSet(skillSet))
                    {
                        foundDoc = doc;
                        break;
                    } // end if
                } // end foreach

                if (foundDoc == null)
                    nonAvailableSkillSets.Add(skillSet);
                else
                    nonChosenDoctors.Remove(foundDoc);
            } // end foreach

            return nonAvailableSkillSets;
        } // end of CheckAvailabilityOfDoctors

        #endregion CheckAvailabilityOfDoctors
    }
}
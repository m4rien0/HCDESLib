using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Management;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.StaffHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Linq;
using System.Collections.Generic;
namespace SampleHospitalModel.Outpatient
{
    /// <summary>
    /// Sample control unit implementing a basic outpatient department
    /// </summary>
    public class ControlUnitOutpatientMedium : ControlUnitOutpatient
    {
        #region Constructor

        /// <summary>
        /// Basic constructor just calls base constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent management control unit</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Outpatient input data</param>
        /// <param name="handledOutpatientAdmissionTypes">Outpatient admission types that are handled by the control unit</param>
        /// <param name="waitinListControlUnit">Corresponding waiting list control unit</param>
        public ControlUnitOutpatientMedium(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            IInputOutpatient inputData,
                            ControlUnit waitingListControlUnit)
            : base(name, 
                   parentControlUnit, 
                   inputData.GetAdmissionTypes(), 
                   parentSimulationModel, 
                   waitingListControlUnit, inputData)
        {

        } // end of OutpatientControlUnit

        #endregion

        #region Initialize

        /// <summary>
        /// Initializes walk in patient stream
        /// </summary>
        /// <param name="startTime">Start time of the simulation model</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
           
            DateTime arrivalWalkIn;

            EntityPatient patient = InputData.GetNextWalkInPatient(out arrivalWalkIn, ParentControlUnit, startTime);

            if (patient != null)
            {
                EventOutpatientWalkInPatientArrival firstPatient = new EventOutpatientWalkInPatientArrival(this, InputData, patient);

                simEngine.AddScheduledEvent(firstPatient, arrivalWalkIn);
            } // end if

        } // end of Initialize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Custom rules of outpatient control, just calls dispatching and control method 
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

        #endregion

        #region PerformDisptatching

        // <summary>
        /// Handles requests of staff members to leave that are out of shift, register, assessment and treatment
        /// using patient slot times and priority
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

            #endregion

            #region Register

            List<RequestOutpatientAction> registerRequests = new List<RequestOutpatientAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestOutpatientAction)p).ActionType.Type == "Register").Cast<RequestOutpatientAction>());

            while (registerRequests.Count > 0)
            {

                // Get Register request Triage-FIFO
                RequestOutpatientAction requestRegister = registerRequests.OrderBy(p => p.TimeRequested).First();

                ResourceSet chosenResources;

                if (!ChooseResourcesForAction(requestRegister,
                    out chosenResources))
                {
                    break;
                } // enf if

                RemoveRequest(requestRegister);
                registerRequests.Remove(requestRegister);
                EntityPatient patient = (EntityPatient)requestRegister.Origin.First();

                ActivityHealthCareAction<OutpatientActionTypeClass> register = new ActivityHealthCareAction<OutpatientActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestRegister.ActionType,
                    patient.OutpatientTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                register.StartEvent.Trigger(time, simEngine);

            } // end while

            #endregion

            #region Assessment

            List<RequestOutpatientAction> assessmentRequests = new List<RequestOutpatientAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestOutpatientAction)p).ActionType.Type == "Assessment").Cast<RequestOutpatientAction>());

            while (assessmentRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Slottime-FIFO
                RequestOutpatientAction requestAssessment = PatientSlotTimePlusPriority(assessmentRequests);

                if (!ChooseResourcesForAction(requestAssessment,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list
                RemoveRequest(requestAssessment);
                assessmentRequests.Remove(requestAssessment);
                EntityPatient patient = (EntityPatient)requestAssessment.Origin.First();

                ActivityHealthCareAction<OutpatientActionTypeClass> assessment = new ActivityHealthCareAction<OutpatientActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestAssessment.ActionType,
                    patient.OutpatientTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                assessment.StartEvent.Trigger(time, simEngine);

            } // end while

            #endregion

            #region Treatment

            List<RequestOutpatientAction> treatmentRequests = new List<RequestOutpatientAction>(RAEL.Where(p => p.Activity == "ActivityHealthCareAction"
                && ((RequestOutpatientAction)p).ActionType.Type == "Treatment").Cast<RequestOutpatientAction>());

            while (treatmentRequests.Count > 0)
            {
                ResourceSet chosenResources;

                // Get Register request Triage-FIFO
                RequestOutpatientAction requestTreatment = PatientPriorityPlusFIFO<RequestOutpatientAction, OutpatientActionTypeClass>(treatmentRequests);

                if (!ChooseResourcesForAction(requestTreatment,
                    out chosenResources))
                {
                    break;
                } // enf if

                // Remove Request from RAEL list and treatmentlist
                RemoveRequest(requestTreatment);
                treatmentRequests.Remove(requestTreatment);
                EntityPatient patient = (EntityPatient)requestTreatment.Origin.First();

                ActivityHealthCareAction<OutpatientActionTypeClass> assessment = new ActivityHealthCareAction<OutpatientActionTypeClass>(
                    this,
                    InputData,
                    patient,
                    chosenResources,
                    requestTreatment.ActionType,
                    patient.OutpatientTreatmentPath);

                chosenResources.StopCurrentActivities(time, simEngine);
                patient.StopCurrentActivities(time, simEngine);
                assessment.StartEvent.Trigger(time, simEngine);

            } // end while

            #endregion

            return false;

        } // end of PerformDisptatching

        #endregion

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
            IEnumerable<RequestOutpatientAction> assistedRequests = RAEL.Where(p => p.GetType() == typeof(RequestOutpatientAction) && ((RequestOutpatientAction)p).IsAssistedDoctor).Cast<RequestOutpatientAction>().ToList();

            bool moveTriggered = false;

            foreach (EntityDoctor doc in assistingDocs)
            {
                bool skillRequired = false;

                if (doc.IsWaiting())
                {
                    foreach (RequestOutpatientAction req in assistedRequests)
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

            return false;

        } // end of PerformControlled

        #endregion

        #region ChooseResourcesForAction

        /// <summary>
        /// Helping method to choose resource for action requests, first available resources
        /// that satisfy requirements are chosen or it is checked if preassigned staff members
        /// (e.g. corresponding doctors) have capacitiy to undertake action
        /// </summary>
        /// <param name="outPatientRequestRequest">The request that is the soruce for the action</param>
        /// <param name="resources">Out parameter of chosen resources for action</param>
        /// <returns>True if all resources have been assigned, false else</returns>
        public bool ChooseResourcesForAction(
            RequestOutpatientAction outPatientRequestRequest,
            out ResourceSet resources)
        {
            resources = new ResourceSet();

            #region Doctors

            List<EntityDoctor> chosenDoctors = new List<EntityDoctor>();

            #region MainDoc

            if (outPatientRequestRequest.ActionType.MainDoctorRequirements != null)
            {
                if (outPatientRequestRequest.ResourceSet.MainDoctor != null)
                {
                    if (outPatientRequestRequest.ResourceSet.MainDoctor.IsWaiting())
                    {
                        chosenDoctors.Add(resources.MainDoctor);
                        resources.MainDoctor = outPatientRequestRequest.ResourceSet.MainDoctor;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {
                    List<EntityDoctor> possibleDocs = ControlledDoctors.Where(p => p.IsWaiting()
                        && p.SatisfiesSkillSet(outPatientRequestRequest.ActionType.MainDoctorRequirements)
                        && !p.StaffOutsideShift).ToList();

                    if (possibleDocs.Count == 0)
                        return false;

                    resources.MainDoctor = possibleDocs.First();
                    chosenDoctors.Add(resources.MainDoctor);
                } // end if

            } // end if

            #endregion

            #region AssistingDoctors

            if (outPatientRequestRequest.ActionType.AssistingDoctorRequirements != null)
            {

                if (outPatientRequestRequest.ResourceSet.AssistingDoctors != null)
                {
                    foreach (EntityDoctor doctor in outPatientRequestRequest.ResourceSet.AssistingDoctors)
                    {
                        if (!doctor.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityDoctor> foundDoctors = new List<EntityDoctor>();

                    List<EntityDoctor> allDoctors = new List<EntityDoctor>(ControlledDoctors);

                    foreach (SkillSet skillSet in outPatientRequestRequest.ActionType.AssistingDoctorRequirements)
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

            #endregion

            #endregion

            #region Nurses

            List<EntityNurse> chosenNurses = new List<EntityNurse>();

            #region MainNurse

            if (outPatientRequestRequest.ActionType.MainNurseRequirements != null)
            {
                if (outPatientRequestRequest.ResourceSet.MainNurse != null)
                {
                    if (outPatientRequestRequest.ResourceSet.MainNurse.IsWaiting())
                    {
                        chosenNurses.Add(resources.MainNurse);
                        resources.MainNurse = outPatientRequestRequest.ResourceSet.MainNurse;
                    }
                    else
                    {
                        return false;
                    } // end if
                }
                else
                {
                    List<EntityNurse> possibleDocs = ControlledNurses.Where(p => p.IsWaiting()
                        && p.SatisfiesSkillSet(outPatientRequestRequest.ActionType.MainNurseRequirements)
                        && !p.StaffOutsideShift).ToList();

                    if (possibleDocs.Count == 0)
                        return false;

                    resources.MainNurse = possibleDocs.First();
                    chosenNurses.Add(resources.MainNurse);
                } // end if

            } // end if

            #endregion

            #region AssistingNurses

            if (outPatientRequestRequest.ActionType.AssistingNurseRequirements != null)
            {

                if (outPatientRequestRequest.ResourceSet.AssistingNurses != null)
                {
                    foreach (EntityNurse nurse in outPatientRequestRequest.ResourceSet.AssistingNurses)
                    {
                        if (!nurse.IsWaiting())
                            return false;
                    } // end foreach
                }
                else
                {
                    List<EntityNurse> foundNurses = new List<EntityNurse>();

                    List<EntityNurse> allNurses = new List<EntityNurse>(ControlledNurses);

                    foreach (SkillSet skillSet in outPatientRequestRequest.ActionType.AssistingNurseRequirements)
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

            #endregion

            #endregion

            #region TreatmentFacilities

            if (outPatientRequestRequest.Patient.OccupiedFacility != null
                && outPatientRequestRequest.Patient.OccupiedFacility.SatisfiesSkillSet(outPatientRequestRequest.ActionType.FacilityRequirements))
            {
                resources.TreatmentFacility = outPatientRequestRequest.Patient.OccupiedFacility;
            }
            else
            {
                bool foundFacility = false;

                foreach (EntityTreatmentFacility fac in AssignedTreatmentFacilities.Where(p => !p.BlockedForPatient))
                {
                    if (!fac.Occupied && fac.SatisfiesSkillSet(outPatientRequestRequest.ActionType.FacilityRequirements))
                    {
                        resources.TreatmentFacility = fac;
                        foundFacility = true;
                        break;
                    } // end if                     
                } // end foreach

                if (!foundFacility)
                    return false;
            } // end if

            #endregion

            return true;
        } // end of ChooseResourcesForAction

        #endregion

        #region CheckAvailabilityOfDoctors

        /// <summary>
        /// Checks if all skills of doctors are currently controlled by the control unit
        /// </summary>
        /// <param name="mainDocSkill">Skill required for main doctor</param>
        /// <param name="reqAssSkills">Skills required for assisting doctors</param>
        /// <returns>True if all skills are controlled</returns>
        protected List<SkillSet> CheckAvailabilityOfDoctors(SkillSet mainDocSkill, SkillSet[] reqAssSkills)
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

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Control Mechansisms
        //--------------------------------------------------------------------------------------------------

        #region PatientSlotTimePlusPriority

        /// <summary>
        /// Helping method to choose the next patient based on slot time and priority
        /// </summary>
        /// <param name="requests">All current requests</param>
        /// <returns>Next request to handle</returns>
        public RequestOutpatientAction PatientSlotTimePlusPriority(List<RequestOutpatientAction> requests)
        {
            if (requests.Count == 0)
                return null;

            // determine the earliest slot time
            RequestOutpatientAction minSlotTime = requests.Aggregate((curmin, x) => (curmin == null || (x.TimeRequested) < curmin.TimeRequested ? x : curmin));

            // filter all requests of that time
            List<RequestOutpatientAction> requestWithEarliestSlotTime = requests.Where(p => p.SlotTime == minSlotTime.SlotTime).ToList();

            // return request with highest priority
            return requests.Aggregate((curmin, x) => (curmin == null || (x.Patient.PatientClass.Priority) < curmin.Patient.PatientClass.Priority ? x : curmin));

        } // end of 

        #endregion
    }
}

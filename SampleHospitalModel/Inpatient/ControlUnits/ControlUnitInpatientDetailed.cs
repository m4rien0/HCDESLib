using Enums;
using SimulationCore.MathAndHelpers;
using SimulationCore.HCCMElements.ActivityHandling;
using SimulationCore.HCCMElements.ActivityHandling.Activities;
using SimulationCore.HCCMElements.ActivityHandling.Activities.InpatientActivities;
using SimulationCore.HCCMElements.ActivityHandling.Requests;
using SimulationCore.HCCMElements.ControlUnit;
using SimulationCore.HCCMElements.Delegates;
using SimulationCore.HCCMElements.Diagnostics;
using SimulationCore.HCCMElements.Diagnostics.Activities;
using SimulationCore.HCCMElements.Drawing;
using SimulationCore.HCCMElements.Entities;
using SimulationCore.HCCMElements.Entities.DefaultEntities;
using SimulationCore.HCCMElements.Entities.InpatientEntities;
using SimulationCore.HCCMElements.Entities.OverallEntities;
using SimulationCore.HCCMElements.Events;
using SimulationCore.HCCMElements.EventsHandling.Events.InpatientEvents;
using SimulationCore.HCCMElements.Hospital.Activities;
using SimulationCore.HCCMElements.Inpatient.Actitvities;
using SimulationCore.HCCMElements.Inpatient.Events;
using SimulationCore.HCCMElements.InputOutput;
using SimulationCore.HCCMElements.InputOutput.Interfaces;
using SimulationCore.HCCMElements.SpecialTreatmentModels.Delegates;
using SimulationCore.Simulation_Core.HCCMElements.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthElements.Inpatient.ControlUnits
{
    public class ControlUnitInpatientDetailed : ControlUnitInpatient
    {
        #region Constructor

        public ControlUnitInpatientDetailed(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            InpatientWaitingListControlUnit waitingListControlUnits,
                            Dictionary<WardTypes,List<EntityWard>> wards,
                            IInputInpatient inputData)
            : base(name, parentControlUnit, inputData.GetHandledInpatientTreatments().ToArray(), inputData.GetHandledInpatientAdmissionTypes(), parentSimulationModel, waitingListControlUnits, inputData)
        {
            
            AddEntities(InputData.GetDoctors(this));

            _wards = new List<EntityWard>();

            foreach (KeyValuePair<WardTypes, List<EntityWard>> wardType in wards)
            {
                foreach (EntityWard ward in wardType.Value)
                {
                    _wards.Add(ward);
                    ward.ParentControlUnit = this;

                    foreach (EntityBed bed in ward.AllBeds)
                    {
                        bed.ParentControlUnit = this;
                    } // end foreach

                } // end foreach
            } // end foreach

            _wardsPerType = wards;
            _roundsDue = false;
            _duringWeekday = false;

            AddEntity(WaitingRoomPatients);
            AddEntity(WaitingRoomStaff);

            _possibleChangeWard = new List<RequestInpatientChangeWard>();
            _possibleEmergencyStayInBed = new List<RequestInpatientStayInBed>();
            _possibleScheduledStayInBed = new List<RequestInpatientStayInBed>();
        }

        #endregion

        #region Initialize

        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            if (startTime.Date + InputData.InPatientStartTime() >= startTime)
            {
                simEngine.AddScheduledEvent(new EventInpatientDayStart(this), startTime + InputData.InPatientStartTime());
            }
            else
            {
                simEngine.AddScheduledEvent(new EventInpatientDayStart(this), startTime.Date + TimeSpan.FromDays(1) + InputData.InPatientStartTime());
            } // end if

        } // end of Initialize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            bool eventLaunched = false;

            bool newEventLauchned = true;

            while (newEventLauchned)
            {
                if (PerformAssessment(time, simEngine))
                {
                    eventLaunched = true;
                    continue;
                } // end if

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

        #region PerformAssessment

        protected bool PerformAssessment(DateTime time, ISimulationEngine simEngine)
        {
           
            PossibleRAEL.Clear();
            PossibleEmergencyStayInBed.Clear();
            PossibleScheduledStayInBed.Clear();
            PossibleChangeWard.Clear();

            bool bedsAvailable = AllAvailableBeds > 0;

            foreach (ActivityRequest request in RAEL)
            {
                if (request.Activity == "ActivityInpatientStayInBed" && bedsAvailable)
                {
                    if (((RequestInpatientStayInBed)request).InpatientOrigin == InpatientOrigin.Scheduled)
                        PossibleScheduledStayInBed.Add((RequestInpatientStayInBed)request);
                    else if (((RequestInpatientStayInBed)request).InpatientOrigin == InpatientOrigin.Emergency)
                        PossibleEmergencyStayInBed.Add((RequestInpatientStayInBed)request);
                }
                else if (request is RequestInpatientActivity && ((RequestInpatientActivity)request).InpatientTreatment.IsEmergencyTreatment)
                    PossibleRAEL.Add(request);
                else if (request is RequestInpatientActivity && ((RequestInpatientActivity)request).Patient.StaysInBed
                        && !RoundsDue
                        && DuringWeekday)
                    PossibleRAEL.Add(request);
                else if (request is RequestInpatientResumeActivity && ((RequestInpatientResumeActivity)request).Patient.StaysInBed
                         && !RoundsDue
                         && DuringWeekday)
                    PossibleRAEL.Add(request);
                else if (request is RequestInpatientDoctorOrganziationalWork 
                         && ((RequestInpatientDoctorOrganziationalWork)request).Doctor.IsWaiting()
                         && !RoundsDue
                         && (((RequestInpatientDoctorOrganziationalWork)request).Doctor).ParentControlUnit == this)
                    PossibleRAEL.Add(request);
                else if (bedsAvailable && request is RequestInpatientChangeWard && ((RequestInpatientChangeWard)request).Patient.StaysInBed)
                    PossibleChangeWard.Add((RequestInpatientChangeWard)request);

            } // end foreach

            return false;

        } // end of PerformAssessment

        #endregion

        #region PerformDisptatching

        protected bool PerformDisptatching(DateTime time, ISimulationEngine simEngine)
        {
            #region StayInBed

            bool triggeredStayInBed = false;

            
            foreach (RequestInpatientStayInBed request in PossibleEmergencyStayInBed)
            {
                EntityPatient patient = request.Patient;
                List<EntityWard> possibleWards = WardsPerType[patient.InpatientPath.CurrentTreatmentBlock.WardType];

                EntityBed bed = null;

                foreach (EntityWard ward in possibleWards)
                {
                    bed = ward.GetBed(patient.InpatientPath.CurrentTreatmentBlock.BedType);
                    if (bed != null)
                        break;
                } // end foreach

                if (bed != null)
                {
                    patient.InpatientPath.CurrentTreatmentBlock.InititateTreatmentBlock(bed.ParentWard, bed);
                    patient.StopCurrentActivities(time, simEngine);
                    ActivityInpatientStayInBed stayInBed = new ActivityInpatientStayInBed(this, patient, bed.ParentWard, bed, false);
                    stayInBed.StartEvent.Trigger(time, simEngine);
                    triggeredStayInBed = true;

                    RemoveRequest(request);
                } // end if 
            } // end foreach

            foreach (RequestInpatientChangeWard request in PossibleChangeWard)
            {
                EntityPatient patient = request.Patient;
                List<EntityWard> possibleWards = WardsPerType[patient.InpatientPath.CurrentTreatmentBlock.WardType];

                EntityBed bed = null;

                foreach (EntityWard ward in possibleWards)
                {
                    bed = ward.GetBed(patient.InpatientPath.CurrentTreatmentBlock.BedType);
                    if (bed != null)
                        break;
                } // end foreach

                if (bed != null)
                {
                    patient.InpatientPath.CurrentTreatmentBlock.InititateTreatmentBlock(bed.ParentWard, bed);
                    request.OldBed.RemovePatient();
                    patient.StopCurrentActivities(time, simEngine);
                    ActivityInpatientStayInBed stayInBed = new ActivityInpatientStayInBed(this, patient, bed.ParentWard, bed, false);
                    stayInBed.StartEvent.Trigger(time, simEngine);

                    RemoveRequest(request);
                } // end if 
            } // end foreach

            foreach (RequestInpatientStayInBed request in PossibleScheduledStayInBed)
            {
                EntityPatient patient = request.Patient;
                List<EntityWard> possibleWards = WardsPerType[patient.InpatientPath.CurrentTreatmentBlock.WardType];

                EntityBed bed = null;

                foreach (EntityWard ward in possibleWards)
                {
                    bed = ward.GetBed(patient.InpatientPath.CurrentTreatmentBlock.BedType);
                    if (bed != null)
                        break;
                } // end foreach

                if (bed != null)
                {
                    patient.InpatientPath.CurrentTreatmentBlock.InititateTreatmentBlock(bed.ParentWard, bed);
                    patient.StopCurrentActivities(time, simEngine);
                    ActivityInpatientStayInBed stayInBed = new ActivityInpatientStayInBed(this, patient, bed.ParentWard, bed, false);
                    stayInBed.StartEvent.Trigger(time, simEngine);

                    RemoveRequest(request);
                } // end if 
            } // end foreach

            #endregion

            #region InpatientActivities

            List<RequestInpatientActivity> originalRequests = PossibleRAEL.Where(p => p is RequestInpatientActivity).Cast<RequestInpatientActivity>().ToList();
            List<RequestInpatientResumeActivity> resumeRequests = PossibleRAEL.Where(p => p is RequestInpatientResumeActivity).Cast<RequestInpatientResumeActivity>().ToList();

            List<RequestInpatientActivity> requestsEmergencyTreatment = originalRequests.Where(p => p.InpatientTreatment.IsEmergencyTreatment && ((RequestInpatientActivity)p).EarliestTime <= time).Cast<RequestInpatientActivity>().ToList();
            List<RequestInpatientActivity> requestsTreatment = originalRequests.Where(p => !p.InpatientTreatment.IsEmergencyTreatment && ((RequestInpatientActivity)p).EarliestTime <= time).Cast<RequestInpatientActivity>().ToList();
            List<RequestInpatientResumeActivity> requestsResumeTreatment = resumeRequests.Where(p => !p.InpatientTreatment.IsEmergencyTreatment && ((RequestInpatientResumeActivity)p).EarliestTime <= time).Cast<RequestInpatientResumeActivity>().ToList();
            

            #region EmergencyTreatments

            if (requestsEmergencyTreatment.Count > 0)
            {
                EntityPatient patient = requestsEmergencyTreatment.First().Patient;

                EntityDoctor doctor = ChooseDoctor(requestsEmergencyTreatment.First().Patient, requestsEmergencyTreatment.First());

                if (doctor != null)
                {
                    ActivityInpatientEmergencyTreatment newInpatientEmergencyTreatment = new ActivityInpatientEmergencyTreatment(this,
                                                                                                        patient,
                                                                                                        doctor,
                                                                                                        patient.Ward,
                                                                                                        patient.Bed);
                    patient.StopCurrentActivities(time, simEngine);
                    doctor.StopCurrentActivities(time, simEngine);

                    newInpatientEmergencyTreatment.StartEvent.Trigger(time, simEngine);

                    PossibleRAEL.Remove(requestsEmergencyTreatment[0]);
                    RemoveRequest(requestsEmergencyTreatment[0]);
                    requestsEmergencyTreatment.RemoveAt(0);

                    return true;
                } // end if

            } // end if

            #endregion

            #region Treatments

            if (requestsTreatment.Count > 0)
            {
                EntityPatient patient = requestsTreatment.First().Patient;

                EntityDoctor doctor = ChooseDoctor(requestsTreatment.First().Patient, requestsTreatment.First());

                if (doctor != null)
                {
                    ActivityInpatientTreatment newInpatientTreatment = new ActivityInpatientTreatment(
                                                                                                        this,
                                                                                                        patient,
                                                                                                        doctor,
                                                                                                        patient.Ward,
                                                                                                        patient.Bed,
                                                                                                        requestsTreatment.First().InpatientTreatment);
                    patient.StopCurrentActivities(time, simEngine);
                    doctor.StopCurrentActivities(time, simEngine);

                    newInpatientTreatment.StartEvent.Trigger(time, simEngine);

                    PossibleRAEL.Remove(requestsTreatment[0]);
                    RemoveRequest(requestsTreatment[0]);
                    requestsTreatment.RemoveAt(0);

                    return true;
                } // end if

            } // end if

            #endregion

            #region ResumeTreatments

            if (requestsResumeTreatment.Count > 0)
            {
                EntityPatient patient = requestsResumeTreatment.First().Patient;

                EntityDoctor doctor = ChooseDoctor(requestsResumeTreatment.First().Patient, requestsResumeTreatment.First());

                if (doctor != null)
                {
                    ActivityInpatientTreatment newInpatientTreatment = new ActivityInpatientTreatment(this,
                                                                                                        patient,
                                                                                                        doctor,
                                                                                                        patient.Ward,
                                                                                                        patient.Bed,
                                                                                                        requestsResumeTreatment.First().DegreeOfCompletion,
                                                                                                        requestsResumeTreatment.First().Duration,
                                                                                                        requestsResumeTreatment.First().InpatientTreatment);
                    patient.StopCurrentActivities(time, simEngine);
                    doctor.StopCurrentActivities(time, simEngine);

                    newInpatientTreatment.StartEvent.Trigger(time, simEngine);

                    PossibleRAEL.Remove(requestsResumeTreatment[0]);
                    RemoveRequest(requestsResumeTreatment[0]);
                    requestsResumeTreatment.RemoveAt(0);

                    return true;
                } // end if

            } // end if

            #endregion

            #endregion

            #region OrganizationalWork

            bool triggeredOrganizationalWork = false;

            List<RequestInpatientDoctorOrganziationalWork> allOrgReqs = PossibleRAEL.Where(p => p is RequestInpatientDoctorOrganziationalWork).Cast<RequestInpatientDoctorOrganziationalWork>().ToList();

            foreach (RequestInpatientDoctorOrganziationalWork reqOrg in allOrgReqs)
            {
                if (reqOrg.Doctor.IsWaiting())
                {
                    reqOrg.Doctor.StopCurrentActivities(time, simEngine);

                    ActivityInpatientDoctorOrganizationalWork orgWork = new ActivityInpatientDoctorOrganizationalWork(this, reqOrg.Doctor, WaitingRoomStaff, reqOrg.DegreeOfCompletion, reqOrg.Duration);

                    orgWork.StartEvent.Trigger(time, simEngine);

                    triggeredOrganizationalWork = true;

                    PossibleRAEL.Remove(reqOrg);
                    RemoveRequest(reqOrg);
                } // end if

                
            } // end foreach

            #endregion

            return triggeredStayInBed || triggeredOrganizationalWork;
        } // end of PerformDisptatching

        #endregion

        #region PerformControlled

        protected bool PerformControlled(DateTime time, ISimulationEngine simEngine)
        {
            IReadOnlyList<EntityDoctor> doctors = ControlledDoctors;
            if (RoundsDue)
            {
                List<EntityDoctor> availableDoctors = doctors.Where(p => p.IsWaiting()).ToList();

                if (availableDoctors.Count > 0)
                {
                    
                    ActivityInpatientRound newRound = new ActivityInpatientRound(this, availableDoctors, Wards);

                    foreach (EntityDoctor doc in availableDoctors)
                    {
                        doc.StopWaitingActivity(time, simEngine);
                    } // end foreach

                    newRound.StartEvent.Trigger(time, simEngine);

                    foreach (EntityDoctor doc in doctors)
                    {
                        doc.BlockedForDispatching = false;
                    } // end foreach

                    RoundsDue = false;

                } // end if
            } // end foreach

            return false;
        } // end of PerformControlled

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region RoundsDue

        private bool _roundsDue;

        public bool RoundsDue
        {
            get
            {
                return _roundsDue;
            }
            set
            {
                _roundsDue = value;
            }
        } // end of RoundsDue

        #endregion

        #region DuringWeekday

        private bool _duringWeekday;

        public bool DuringWeekday
        {
            get
            {
                return _duringWeekday;
            }
            set
            {
                _duringWeekday = value;
            }
        } // end of DuringWeekday

        #endregion

        #region WardsPerType

        protected Dictionary<WardTypes, List<EntityWard>> _wardsPerType;

        public Dictionary<WardTypes, List<EntityWard>> WardsPerType
        {
            get
            {
                return _wardsPerType;
            }
        } // end of Wards

        #endregion

        #region Wards

        private List<EntityWard> _wards;

        public List<EntityWard> Wards
        {
            get
            {
                return _wards;
            }
        } // end of Wards

        #endregion

        #region AllAvailableBeds

        public int AllAvailableBeds
        {
            get
            {
                return Wards.Select(p=>p.AllAvailableBeds.Count).Sum();
            }
        } // end of AllAvailableBeds

        #endregion

        #region BedTypes

        private List<BedType> _bedTypes;

        public List<BedType> BedTypes
        {
            get
            {
                return _bedTypes;
            }
        } // end of BedTypes

        #endregion

        #region PossibleEmergencyStayInBed

        private List<RequestInpatientStayInBed> _possibleEmergencyStayInBed;

        public List<RequestInpatientStayInBed> PossibleEmergencyStayInBed
        {
            get
            {
                return _possibleEmergencyStayInBed;
            }
            set
            {
                _possibleEmergencyStayInBed = value;
            }
        } // end of PossibleEmergencyStayInBed

        #endregion

        #region PossibleScheduledStayInBed

        private List<RequestInpatientStayInBed> _possibleScheduledStayInBed;

        public List<RequestInpatientStayInBed> PossibleScheduledStayInBed
        {
            get
            {
                return _possibleScheduledStayInBed;
            }
            set
            {
                _possibleScheduledStayInBed = value;
            }
        } // end of PossibleScheduledStayInBed

        #endregion

        #region PossibleChangeWard

        private List<RequestInpatientChangeWard> _possibleChangeWard;

        public List<RequestInpatientChangeWard> PossibleChangeWard
        {
            get
            {
                return _possibleChangeWard;
            }
            set
            {
                _possibleChangeWard = value;
            }
        } // end of PossibleChangeWard

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Custom Methods
        //--------------------------------------------------------------------------------------------------

        #region ChooseDoctor

        private EntityDoctor ChooseDoctor(EntityPatient patient, ActivityRequest request)
        {
            IReadOnlyList<EntityDoctor> doctors = ControlledDoctors;

            foreach (EntityDoctor doctor in doctors)
            {
                if ((doctor.IsWaiting() || doctor.IsInOnlyActivity("ActivityInpatientDoctorOrganizationalWork"))
                    && !doctor.BlockedForDispatching)
                    return doctor;
            } // end foreach

            if (request is RequestInpatientActivity && ((RequestInpatientActivity)request).Activity == "ActivityInpatientEmergencyTreatment")
            {
                foreach (EntityDoctor doctor in doctors)
                {
                    if (doctor.IsWaitingOrPreEmptable())
                        return doctor;
                } // end foreach
            } // end if

            return null;

        } // end of ChooseDoctor

        #endregion


    } // end of InpatientControlUnit
}

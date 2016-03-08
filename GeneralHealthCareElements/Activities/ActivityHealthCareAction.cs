using Enums;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Activities
{
    /// <summary>
    /// Basic version of a health care activity.
    /// </summary>
    /// <typeparam name="T">Defines type of action (e.g. emergency or outpatient) </typeparam>
    public class ActivityHealthCareAction<T> : Activity where T : ActionTypeClass
    {
        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="parentControlUnit">Parent control unit, either a health care department control unit or a organizational unit</param>
        /// <param name="input">Input data of department model</param>
        /// <param name="patient">Patient receiving service</param>
        /// <param name="resources">Resources included in the activity</param>
        /// <param name="type">Type of activity (e.g. emergency action type)</param>
        /// <param name="patientPath">The current path of the patient (can vary for outpatient or emergency paths)</param>
        public ActivityHealthCareAction(ControlUnit parentControlUnit,
            IInputHealthCareDepartment input,
            EntityPatient patient,
            ResourceSet resources,
            T type,
            PatientPath<T> patientPath)
            : base(parentControlUnit,
                "ActivityHealthCareAction", 
                type.IsPreemptable)
        {
            _inputData = input;
            _patient = patient;
            _actionType = type;
            _resourceSet = resources;
            _patientPath = patientPath;

            // in case that the parent control unit is a organizational control
            // the parent department control is set
            if (parentControlUnit is ControlUnitHealthCareDepartment)
                _parentDepartmentControl = parentControlUnit as ControlUnitHealthCareDepartment;
            else if (parentControlUnit is ControlUnitOrganizationalUnit)
                _parentDepartmentControl = ((ControlUnitOrganizationalUnit)parentControlUnit).ParentDepartmentControl as ControlUnitHealthCareDepartment;
        } // end of ActivityHealthCareAction

        #endregion

        #region Name

        public static string Name = "ActivityHealthCareAction";

        #endregion

        #region ParentDepartmentControl

        private ControlUnitHealthCareDepartment _parentDepartmentControl;

        /// <summary>
        /// Parent department control, may differ from parent control if the latter is an organizational unit
        /// </summary>
        public ControlUnitHealthCareDepartment ParentDepartmentControl
        {
            get
            {
                return _parentDepartmentControl;
            }
        } // end of ParentDepartmentControl

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent

        /// <summary>
        /// State changes of the activities start event. Most state changes are standardized and configurable via input.
        /// </summary>
        /// <param name="time"> Time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            //--------------------------------------------------------------------------------------------------
            // Some activities define the start of corresponding doctor, nurses for future reference
            //--------------------------------------------------------------------------------------------------

            #region CorrespondingStaff

            if (ActionType.DefinesCorrespondingDocStart)
            {
                Patient.CorrespondingDoctor = ResourceSet.MainDoctor;
                ResourceSet.MainDoctor.AddPatient(Patient);
            } // end if 

            if (ActionType.DefinesCorrespondingNurseStart)
            {
                Patient.CorrespondingNurse = ResourceSet.MainNurse;
                ResourceSet.MainNurse.AddPatient(Patient);
            } // end if 

            #endregion

            //--------------------------------------------------------------------------------------------------
            // If treatment has doctor(s), busyFactors are assigned 
            //--------------------------------------------------------------------------------------------------

            #region AssignBusyFactorsDoctors

            if (ResourceSet.MainDoctor != null)
                ResourceSet.MainDoctor.BusyFactor += ActionType.BusyFactorDoctor;

            if (ResourceSet.AssistingDoctors != null)
            {
                if (ResourceSet.AssistingDoctors.Length != ActionType.BusyFactorAssistingDoctors.Length)
                    throw new InvalidOperationException();

                for (int i = 0; i < ResourceSet.AssistingDoctors.Length; i++)
                {
                    ResourceSet.AssistingDoctors[i].BusyFactor += ActionType.BusyFactorAssistingDoctors[i];
                } // end for
            } // end if 

            #endregion

            //--------------------------------------------------------------------------------------------------
            // If treatment has nurse(s), busyFactors are assigned 
            //--------------------------------------------------------------------------------------------------

            #region AssignBusyFactorsNurses

            if (ResourceSet.MainNurse != null)
                ResourceSet.MainNurse.BusyFactor += ActionType.BusyFactorNurse;

            if (ResourceSet.AssistingNurses != null)
            {
                if (ResourceSet.AssistingNurses.Length != ActionType.BusyFactorAssistingNurses.Length)
                    throw new InvalidOperationException();

                for (int i = 0; i < ResourceSet.AssistingNurses.Length; i++)
                {
                    ResourceSet.AssistingNurses[i].BusyFactor += ActionType.BusyFactorAssistingNurses[i];
                } // end for
            } // end if 

            #endregion

            //--------------------------------------------------------------------------------------------------
            // Preemption 
            //--------------------------------------------------------------------------------------------------

            #region Preemption

            // in case the activity was preempted only the remaining duration is considered for scheduling
            // the end event
            if (DegreeOfCompletion > 0)
            {
                DateTime endTime = time + Helpers<double>.MultiplyTimeSpan(Duration, (1 - DegreeOfCompletion));
                simEngine.AddScheduledEvent(this.EndEvent, endTime);
                return;
            } // end if

            #endregion

            //--------------------------------------------------------------------------------------------------
            // Occupation 
            //--------------------------------------------------------------------------------------------------

            #region Occupation

            // in case of a multiple patient treatment facility the patient
            // is added to the holdeld entities
            if (ResourceSet.TreatmentFacility is EntityMultiplePatientTreatmentFacility)
            {
                ((EntityMultiplePatientTreatmentFacility)ResourceSet.TreatmentFacility).HoldedEntities.Add(Patient);
            }
            else
            {

                //--------------------------------------------------------------------------------------------------
                // Set treatmentBooth to occupied 
                //--------------------------------------------------------------------------------------------------
                ResourceSet.TreatmentFacility.Occupied = true;

                //--------------------------------------------------------------------------------------------------
                // in case patient is blocking the facility required actions are taken
                //--------------------------------------------------------------------------------------------------
                if (ActionType.DefinesFacilitiyOccupationStart)
                {
                    // facility is blocked for patient
                    ResourceSet.TreatmentFacility.PatientBlocking = Patient;

                    // facility is assigned to patient
                    Patient.OccupiedFacility = ResourceSet.TreatmentFacility;
                } // end if 
            } // end if
            #endregion

            //--------------------------------------------------------------------------------------------------
            // Updating of next Acion and possible skipping of latter 
            //--------------------------------------------------------------------------------------------------

            #region UpdateNextAction

            PatientPath.UpdateNextAction();

            T nextActionType = PatientPath.GetCurrentActionType();

            #endregion

            #region PossibleSkipOfNextAction

            //--------------------------------------------------------------------------------------------------
            // A treatment may be skipped, e.g. in case that the current doctor is qualified enough
            // This is defined via the input
            //--------------------------------------------------------------------------------------------------
            while (ParentDepartmentControl.SkipNextAction(Patient, ResourceSet.MainDoctor, ActionType, PatientPath.GetCurrentActionType()))
            {
                PatientPath.UpdateNextAction();
                nextActionType = PatientPath.GetCurrentActionType();
            } // end if

            #endregion

            //--------------------------------------------------------------------------------------------------
            // In case a holding Activity follows no end event is scheduled
            //--------------------------------------------------------------------------------------------------

            #region HoldingOfPatient

            if (ActionType.IsHold)
            {
                // Staff on hold flag is set to true
                foreach (EntityStaff staff in AffectedEntities.Where(p => p is EntityStaff))
                {
                    staff.OnHold = true;
                } // end foreach

                HoldingRequired = true;
                // in case of holding the next action on the path is taken
                if (PatientPath.TakeNextAction(simEngine, StartEvent, time, ParentControlUnit))
                {
                    // either waiting or waiting in the treatment facility is launched
                    if (Patient.OccupiedFacility == null || Patient.OccupiedFacility.ParentDepartmentControl != ParentDepartmentControl)
                    {
                        EndEvent.SequentialEvents.Add(Patient.StartWaitingActivity(ParentDepartmentControl.WaitingAreaPatientForNextActionType(nextActionType)));
                    }
                    else
                    {
                        ActivityWaitInFacility waitInFacility = new ActivityWaitInFacility(ParentControlUnit, Patient, Patient.OccupiedFacility);
                        EndEvent.SequentialEvents.Add(waitInFacility.StartEvent);
                    } // end if
                } // end if
            }
            else
            {
                // if the activity is not hold the end event is scheduled
                Duration = InputData.PatientActionTime(Patient,
                                            ResourceSet,
                                            ActionType);

                DateTime endTime = time + Duration;
                simEngine.AddScheduledEvent(this.EndEvent, endTime);
            } // end if

            #endregion

        } // end of TriggerStartEvent

        #endregion

        #region TriggerEndEvent

        /// <summary>
        /// State changes of the activities end event. Most state changes are standardized and configurable via input.
        /// </summary>
        /// <param name="time"> time of activity start</param>
        /// <param name="simEngine"> SimEngine the handles the activity triggering</param>
        override public void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {

            //--------------------------------------------------------------------------------------------------
            // Some activities define the end of corresponding doctor, nurses for future reference
            //--------------------------------------------------------------------------------------------------

            #region CorrespondingStaff

            if (ActionType.DefinesCorrespondingDocEnd)
            {
                Patient.CorrespondingDoctor = null;
                ResourceSet.MainDoctor.RemovePatient(Patient);
            } // end if 

            if (ActionType.DefinesCorrespondingNurseEnd)
            {
                Patient.CorrespondingNurse = null;
                ResourceSet.MainNurse.RemovePatient(Patient);
            } // end if 

            #endregion

            //--------------------------------------------------------------------------------------------------
            // Occupation
            //--------------------------------------------------------------------------------------------------

            #region Occupation

            if (ResourceSet.TreatmentFacility is EntityMultiplePatientTreatmentFacility)
            {
                ((EntityMultiplePatientTreatmentFacility)ResourceSet.TreatmentFacility).HoldedEntities.Remove(Patient);
            }
            else
            {

                ResourceSet.TreatmentFacility.Occupied = false;

                //--------------------------------------------------------------------------------------------------
                // in case patient blocking is released required actions are taken
                //--------------------------------------------------------------------------------------------------
                if (ActionType.DefinesFacilitiyOccupationEnd)
                {
                    // facility is released
                    ResourceSet.TreatmentFacility.PatientBlocking = null;

                    // facility is removed from patient
                    Patient.OccupiedFacility = null;
                } // end if
            } // end if
            #endregion

            //--------------------------------------------------------------------------------------------------
            // If treatment has doctor(s), busyFactors are assigned 
            //--------------------------------------------------------------------------------------------------

            #region AssignBusyFactorsDoctors

            if (ResourceSet.MainDoctor != null)
            {
                ResourceSet.MainDoctor.BusyFactor -= ActionType.BusyFactorDoctor;
                ResourceSet.MainDoctor.BlockedForDispatching = false;
            } // end if

            if (ResourceSet.AssistingDoctors != null)
            {
                if (ResourceSet.AssistingDoctors.Length != ActionType.BusyFactorAssistingDoctors.Length)
                    throw new InvalidOperationException();

                for (int i = 0; i < ResourceSet.AssistingDoctors.Length; i++)
                {
                    ResourceSet.AssistingDoctors[i].BusyFactor -= ActionType.BusyFactorAssistingDoctors[i];
                    ResourceSet.AssistingDoctors[i].BlockedForDispatching = false;
                } // end for
            } // end if 

            #endregion

            //--------------------------------------------------------------------------------------------------
            // If treatment has nurse(s), busyFactors are assigned 
            //--------------------------------------------------------------------------------------------------

            #region AssignBusyFactorsNurses

            if (ResourceSet.MainNurse != null)
                ResourceSet.MainNurse.BusyFactor -= ActionType.BusyFactorNurse;

            if (ResourceSet.AssistingNurses != null)
            {
                if (ResourceSet.AssistingNurses.Length != ActionType.BusyFactorAssistingNurses.Length)
                    throw new InvalidOperationException();

                for (int i = 0; i < ResourceSet.AssistingNurses.Length; i++)
                {
                    ResourceSet.AssistingNurses[i].BusyFactor -= ActionType.BusyFactorAssistingNurses[i];
                } // end for
            } // end if 

            #endregion

            T nextActionType = PatientPath.GetCurrentActionType();

            //--------------------------------------------------------------------------------------------------
            // Preemption 
            //--------------------------------------------------------------------------------------------------

            #region Preempted

            if (!HoldingRequired)
            {
                if (Duration.Ticks > 0)
                    DegreeOfCompletion += (double)(time - StartTime).Ticks / Duration.Ticks;
                else
                    DegreeOfCompletion = 1;

                if (Math.Abs(DegreeOfCompletion - 1) > Helpers<double>.GetNumbericalPrecission())
                {
                    RequestHealthCareAction<T> req =
                            new RequestHealthCareAction<T>(Patient,
                                                       DegreeOfCompletion,
                                                       ActionType,
                                                       time,
                                                       ResourceSet);

                    ParentControlUnit.AddRequest(req);
                    simEngine.RemoveScheduledEvent(EndEvent);
                    EndEvent.SequentialEvents.Add(Patient.StartWaitingActivity(ParentDepartmentControl.WaitingAreaPatientForNextActionType(nextActionType)));
                    return;
                } // end if
            } // end if

            #endregion

            #region NextActions

            //--------------------------------------------------------------------------------------------------
            // In case of an holding treatment the next action was already taken 
            //--------------------------------------------------------------------------------------------------
            
            if (!HoldingRequired)
            {
                if (PatientPath.TakeNextAction(simEngine, 
                        EndEvent,
                        time,
                        ParentControlUnit))
                {
                    // either waiting or waiting in the treatment facility is launched
                    if (Patient.OccupiedFacility == null || Patient.OccupiedFacility.ParentDepartmentControl != ParentDepartmentControl)
                    {
                        EndEvent.SequentialEvents.Add(Patient.StartWaitingActivity(ParentDepartmentControl.WaitingAreaPatientForNextActionType(nextActionType)));
                    }
                    else
                    {
                        ActivityWaitInFacility waitInFacility = new ActivityWaitInFacility(ParentControlUnit, Patient, Patient.OccupiedFacility);
                        EndEvent.SequentialEvents.Add(waitInFacility.StartEvent);
                    } // end if
                } // end if
                //--------------------------------------------------------------------------------------------------
                // Possible waiting activities are started 
                //--------------------------------------------------------------------------------------------------

                #region StartWaitingActivities

                if (ResourceSet.MainDoctor != null && ResourceSet.MainDoctor.GetCurrentActivities().Count == 1)
                    EndEvent.SequentialEvents.Add(ResourceSet.MainDoctor.StartWaitingActivity(ParentDepartmentControl.WaitingRoomForStaff(ResourceSet.MainDoctor)));

                if (ResourceSet.AssistingDoctors != null)
                {
                    foreach (EntityDoctor doc in ResourceSet.AssistingDoctors)
                    {
                        if (doc.GetCurrentActivities().Count == 1)
                            EndEvent.SequentialEvents.Add(doc.StartWaitingActivity(ParentDepartmentControl.WaitingRoomForStaff(doc)));
                    } // end foreach
                } // end if

                if (ResourceSet.MainNurse != null && ResourceSet.MainNurse.GetCurrentActivities().Count == 1)
                    EndEvent.SequentialEvents.Add(ResourceSet.MainNurse.StartWaitingActivity(ParentDepartmentControl.WaitingRoomForStaff(ResourceSet.MainDoctor)));

                if (ResourceSet.AssistingNurses != null)
                {
                    foreach (EntityNurse nurse in ResourceSet.AssistingNurses)
                    {
                        if (nurse.GetCurrentActivities().Count == 1)
                            EndEvent.SequentialEvents.Add(nurse.StartWaitingActivity(ParentDepartmentControl.WaitingRoomForStaff(nurse)));
                    } // end foreach
                } // end if

                #endregion

            }

            #endregion

            #region ReleaseHolding

            if (ActionType.IsHold)
            {
                foreach (EntityStaff staff in AffectedEntities.Where(p => p is EntityStaff))
                {
                    staff.OnHold = false;
                } // end foreach
                return;
            } // end if

            #endregion

        } // end of TriggerEndEvent

        #endregion

        //--------------------------------------------------------------------------------------------------
        // AffectedEntities
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient receiving service
        /// </summary>
        public EntityPatient Patient
        {
            get
            {
                return _patient;
            }
        } // end of Patient

        #endregion

        #region ResourceSet

        private ResourceSet _resourceSet;

        /// <summary>
        /// Resource set performing activity
        /// </summary>
        public ResourceSet ResourceSet
        {
            get
            {
                return _resourceSet;
            }
            set
            {
                _resourceSet = value;
            }
        } // end of ResourceSet

        #endregion

        #region AffectedEntites

        List<Entity> _affectedEntities = null;

        /// <summary>
        /// Overridden affected entities including patient, facility, doctors and nurses
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                if (_affectedEntities == null)
                {
                    _affectedEntities = new List<Entity>();

                    _affectedEntities.Add(Patient);
                    if (ResourceSet.MainDoctor != null)
                        _affectedEntities.Add(ResourceSet.MainDoctor);
                    if (ResourceSet.AssistingDoctors != null)
                        _affectedEntities.AddRange(ResourceSet.AssistingDoctors);
                    if (ResourceSet.MainNurse != null)
                        _affectedEntities.Add(ResourceSet.MainNurse);
                    if (ResourceSet.AssistingNurses != null)
                        _affectedEntities.AddRange(ResourceSet.AssistingNurses);
                    _affectedEntities.Add(ResourceSet.TreatmentFacility);
                }

                return _affectedEntities.ToArray();
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Parameter
        //--------------------------------------------------------------------------------------------------

        #region PatientPath

        private PatientPath<T> _patientPath;

        /// <summary>
        /// The corresponding patient path (can be of different type) for taking next actions
        /// </summary>
        public PatientPath<T> PatientPath
        {
            get
            {
                return _patientPath;
            }
            set
            {
                _patientPath = value;
            }
        } // end of PatientPath

        #endregion

        #region ActionType

        private T _actionType;

        /// <summary>
        /// Action type describing the action of activity
        /// </summary>
        public T ActionType
        {
            get
            {
                return _actionType;
            }
        } // end of TreatmentType

        #endregion

        #region InputData

        private IInputHealthCareDepartment _inputData;

        /// <summary>
        /// The input of the parent health care department, per design organizational units
        /// do not have their own input data
        /// </summary>
        public IInputHealthCareDepartment InputData
        {
            get
            {
                return _inputData;
            }
            set
            {
                _inputData = value;
            }
        } // end of InputData

        #endregion

        #region HoldingRequired

        private bool _holdingRequired;

        /// <summary>
        /// Flag indicating if holding is required for the activity
        /// </summary>
        public bool HoldingRequired
        {
            get
            {
                return _holdingRequired;
            }
            set
            {
                _holdingRequired = value;
            }
        } // end of AssistedTreatmentRequired

        #endregion

        #region Duration

        private TimeSpan _duration;

        /// <summary>
        /// Sampled duration of activity
        /// </summary>
        public TimeSpan Duration
        {
            get
            {
                return _duration;
            }
            set
            {
                _duration = value;
            }
        } // end of Duration

        #endregion

        #region DegreeOfCompletion

        private double _degreeOfCompletion;

        /// <summary>
        /// Degree of completion for pre-empted (interrupted activities)
        /// </summary>
        public double DegreeOfCompletion
        {
            get
            {
                return _degreeOfCompletion;
            }
            set
            {
                if (value < 0 || value > 1)
                    throw new InvalidOperationException("Degree of completion must be between 0 and 1!");
                else
                    _degreeOfCompletion = value;
            }
        } // end of DegreeOfCompletion

        #endregion

        #region PreEmptable

        /// <summary>
        /// Flag for the possibility to pre-empt the activity, only possilbe if holding is not
        /// required
        /// </summary>
        /// <returns></returns>
        public override bool PreEmptable()
        {
            return base.PreEmptable() && !HoldingRequired;
        } // end of PreEmptable

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        override public string ToString()
        {
            return Name;
        } // end of GetName

        #endregion

        #region Clone

        public override Activity Clone()
        {
            return new ActivityHealthCareAction<T>(ParentControlUnit,
                InputData,
                (EntityPatient)Patient.Clone(),
                ResourceSet,
                ActionType,
                PatientPath);
        } // end of Clone

        #endregion

        
    } // end of ActivityHealthCareAction
}

using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input.XMLInputClasses;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.StaffHandling;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneralHealthCareElements.Input
{
    /// <summary>
    /// Abstract class to transform a xml department input in an instance implementing IInputHealthCareDepartment
    /// </summary>
    abstract public class GenericXMLDepartmentInput: IInputHealthCareDepartment
    {
        //--------------------------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="xmlInput">xml input for department</param>
        public GenericXMLDepartmentInput(XMLInputHealthCareDepartment xmlInput)
        {
            _xmlInput = xmlInput;
            _considerAdmissionForPath = xmlInput.ConsiderAdmissionForPath;
            _considerPatientClassForPath = xmlInput.ConsiderPatientClassForPath;

            List<string> structAreas = new List<string>(xmlInput.StructuralAreas);

            if (!structAreas.Contains("RootDepartment"))
                structAreas.Add("RootDepartment");

            _structuralAreaIdentifiers = structAreas.ToArray();

            #region SkillSets

            _skillsPerID = new Dictionary<int, SkillSet>();

            // store skills

            foreach (XMLSkillSet xmlSkillSet in xmlInput.SkillSets)
            {
                _skillsPerID.Add(xmlSkillSet.ID, new SkillSet(xmlSkillSet.Skills.ToArray()));
            } // end foreach

            #endregion

            #region Doctors

            _doctorsPerID = new Dictionary<int, EntityDoctor>();

            // create doctors from xml staff definitions
            foreach (XMLStaff staff in xmlInput.StaffHandling.Doctors)
            {
                if (!SkillsPerID.ContainsKey(staff.SkillID))
                    throw new InvalidOperationException(String.Format("Skill ID for Doctor {0} not specified", staff.ID));

                _doctorsPerID.Add(staff.ID, new EntityDoctor(staff.ID, SkillsPerID[staff.SkillID]));
            } // end foreach

            #endregion

            #region Nurses

            _nursesPerID = new Dictionary<int, EntityNurse>();

            // create nurses from xml nurse definitions
            foreach (XMLStaff staff in xmlInput.StaffHandling.Nurses)
            {
                if (!SkillsPerID.ContainsKey(staff.SkillID))
                    throw new InvalidOperationException(String.Format("Skill ID for Nurse {0} not specified", staff.ID));

                _nursesPerID.Add(staff.ID, new EntityNurse(staff.ID, SkillsPerID[staff.SkillID]));
            } // end foreach

            #endregion

            #region StaffHandler

            _staffAvailabilityPerID = new Dictionary<int, StaffAvailabilityPeriod>();

            // creates a StaffPerPeriodHandler

            // all staff availability periods defined in the xml-input are considered
            // separately
            foreach (XMLStaffAvailabilityPeriod xmlPeriod in xmlInput.StaffHandling.StaffAvailabilityPeriods)
            {
                // checks if staff defintions from xml are skills or actual staff entites
                // those must not be mixed
                if (xmlPeriod.DefinedBySkillSets)
                {
                    List<ResourceAssignment<SkillSet>> doctorSkillsAvailable = new List<ResourceAssignment<SkillSet>>();
                    List<ResourceAssignment<SkillSet>> nurseSkillAvailable = new List<ResourceAssignment<SkillSet>>();

                    foreach (XMLStaffAssignment staffAssingment in xmlPeriod.DoctorAssignments)
                    {
                        if (!staffAssingment.DefinedBySkillSets)
                            throw new InvalidOperationException("Can't mix skill and staff definitions for availability periods!");

                        if (!SkillsPerID.ContainsKey(staffAssingment.SkillID))
                            throw new InvalidOperationException(String.Format("Skill ID {0} not specified for period {1}", staffAssingment.SkillID, xmlPeriod.ID));

                        // create a resource assingment for doctors
                        doctorSkillsAvailable.Add(new ResourceAssignment<SkillSet>(SkillsPerID[staffAssingment.SkillID], staffAssingment.OrganizationalUnit, ParseEnum<AssignmentType>(staffAssingment.AssignmentType)));
                    } // end foreach

                    foreach (XMLStaffAssignment staffAssingment in xmlPeriod.NurseAssignments)
                    {
                        if (!staffAssingment.DefinedBySkillSets)
                            throw new InvalidOperationException("Can't mix skill and staff definitions for availability periods!");

                        if (!SkillsPerID.ContainsKey(staffAssingment.SkillID))
                            throw new InvalidOperationException(String.Format("Skill ID {0} not specified for period {1}", staffAssingment.SkillID, xmlPeriod.ID));

                        // create a resource assingment for nurses
                        nurseSkillAvailable.Add(new ResourceAssignment<SkillSet>(SkillsPerID[staffAssingment.SkillID], staffAssingment.OrganizationalUnit, ParseEnum<AssignmentType>(staffAssingment.AssignmentType)));
                    } // end foreach

                    // here only skills are passed and the StaffHandler generates then entities
                    _staffAvailabilityPerID.Add(xmlPeriod.ID, new StaffAvailabilityPeriod(xmlPeriod.StartHour,
                                                                                          xmlPeriod.EndHour,
                                                                                          doctorSkillsAvailable.ToArray(),
                                                                                          nurseSkillAvailable.ToArray()));
                }
                else
                {
                    List<ResourceAssignmentStaff> doctorAvailable = new List<ResourceAssignmentStaff>();
                    List<ResourceAssignmentStaff> nurseAvailable = new List<ResourceAssignmentStaff>();

                    foreach (XMLStaffAssignment staffAssingment in xmlPeriod.DoctorAssignments)
                    {
                        if (staffAssingment.DefinedBySkillSets)
                            throw new InvalidOperationException("Can't mix skill and staff definitions for availability periods!");

                        if (!DoctorsPerID.ContainsKey(staffAssingment.StaffID))
                            throw new InvalidOperationException(String.Format("Staff ID {0} not specified for period {1}", staffAssingment.SkillID, xmlPeriod.ID));

                        // translating xml assingments into actual resource assignement
                        doctorAvailable.Add(new ResourceAssignmentStaff(DoctorsPerID[staffAssingment.StaffID],
                                                                        staffAssingment.OrganizationalUnit,
                                                                        ParseEnum<AssignmentType>(staffAssingment.AssignmentType)));
                    } // end foreach

                    foreach (XMLStaffAssignment staffAssingment in xmlPeriod.NurseAssignments)
                    {
                        if (staffAssingment.DefinedBySkillSets)
                            throw new InvalidOperationException("Can't mix skill and staff definitions for availability periods!");

                        if (!SkillsPerID.ContainsKey(staffAssingment.SkillID))
                            throw new InvalidOperationException(String.Format("Skill ID {0} not specified for period {1}", staffAssingment.SkillID, xmlPeriod.ID));

                        // translating xml assingments into actual resource assignement
                        nurseAvailable.Add(new ResourceAssignmentStaff(NursesPerID[staffAssingment.StaffID],
                                                                        staffAssingment.OrganizationalUnit,
                                                                        ParseEnum<AssignmentType>(staffAssingment.AssignmentType)));

                    } // end foreach

                    // in this case acutal resource assignments are passed
                    _staffAvailabilityPerID.Add(xmlPeriod.ID, new StaffAvailabilityPeriod(xmlPeriod.StartHour,
                                                                                             xmlPeriod.EndHour,
                                                                                             doctorAvailable.ToArray(),
                                                                                             nurseAvailable.ToArray()));

                } // end if

            } // end foreach

            Dictionary<DayOfWeek, DayTimeLineConfig> daysPerWeekConfigs = new Dictionary<DayOfWeek, DayTimeLineConfig>();

            // for each specified weekday the availability periods are used to create a DayTimeLineConfig that
            // handles available staff
            foreach (XMLStaffAvailabilitiesPerWeekDay xmlAvailPerDay in xmlInput.StaffHandling.StaffPerWeekdays)
            {
                daysPerWeekConfigs.Add(ParseEnum<DayOfWeek>(xmlAvailPerDay.Weekday),
                    new DayTimeLineConfig(xmlAvailPerDay.StaffAvailabilityIDs.Select(p => StaffAvailabilityPerID[p]).ToArray()));
            } // end foreach

            _staffHandler = new StaffPerPeriodHandler(daysPerWeekConfigs);

            #endregion

            #region Facilities

            _treatmentFacilitiesAssignments = new List<ResourceAssignmentPhysical<EntityTreatmentFacility>>();

            // xml treatment faclilities are translated to resource assignments
            foreach (XMLTreatmentFacility treatFac in xmlInput.TreatmentFacilities)
            {
                if (treatFac.IsMultiplePatient)
                {
                    _treatmentFacilitiesAssignments.Add(new ResourceAssignmentPhysical<EntityTreatmentFacility>(
                                                            new EntityMultiplePatientTreatmentFacility(treatFac.ID, SkillsPerID[treatFac.SkillSetID], treatFac.Type, new Point(treatFac.X, treatFac.Y), new Size(treatFac.Width, treatFac.Height)),
                                                            treatFac.OrganizationalUnit,
                                                            treatFac.StructuralArea,
                                                            ParseEnum<AssignmentType>(treatFac.AssignmentType)));
                }
                else
                {
                    _treatmentFacilitiesAssignments.Add(new ResourceAssignmentPhysical<EntityTreatmentFacility>(
                                                            new EntityTreatmentFacility(treatFac.ID, SkillsPerID[treatFac.SkillSetID], treatFac.Type, new Point(treatFac.X, treatFac.Y), new Size(treatFac.Width, treatFac.Height)),
                                                            treatFac.OrganizationalUnit,
                                                            treatFac.StructuralArea,
                                                            ParseEnum<AssignmentType>(treatFac.AssignmentType)));
                } // end if
            } // end foreach

            _waitingRoomPatientResourceAssignment = new List<ResourceAssignmentPhysical<EntityWaitingArea>>();
            _waitingRoomStaffResourceAssignment = new List<ResourceAssignmentPhysical<EntityWaitingArea>>();

            // xml waiting rooms are translated to resource assignments
            foreach (XMLWaitingRoom waitRoom in xmlInput.WaitingRooms)
            {
                if (waitRoom.Type == "WaitingRoomPatient")
                    _waitingRoomPatientResourceAssignment.Add(new ResourceAssignmentPhysical<EntityWaitingArea>(
                                                                    new EntityWaitingArea(waitRoom.ID, waitRoom.Identifier, new Point(waitRoom.X, waitRoom.Y), new Size(waitRoom.Width, waitRoom.Height)),
                                                                    waitRoom.OrganizationalUnit,
                                                                    waitRoom.StructuralArea));
                else if (waitRoom.Type == "WaitingRoomStaff")
                    _waitingRoomStaffResourceAssignment.Add(new ResourceAssignmentPhysical<EntityWaitingArea>(
                                                                     new EntityWaitingArea(waitRoom.ID, waitRoom.Identifier, new Point(waitRoom.X, waitRoom.Y), new Size(waitRoom.Width, waitRoom.Height)),
                                                                     waitRoom.OrganizationalUnit,
                                                                     waitRoom.StructuralArea));
            } // end foreach

            #endregion

            #region ActionTypes

            _actionTypes = new Dictionary<string, ActionTypeClass>();

            // reads all action types specified in the xml input structure
            foreach (XMLActionType xmlAction in xmlInput.ActionTypes)
            {
                _actionTypes.Add(xmlAction.Identifier,
                    new ActionTypeClass(xmlAction.Type,
                                                 xmlAction.Identifier,
                                                 xmlAction.FacilitySkillSetID >= 0 ? SkillsPerID[xmlAction.FacilitySkillSetID] : null,
                                                 xmlAction.MainDoctorSkillSetID >= 0 ? SkillsPerID[xmlAction.MainDoctorSkillSetID] : null,
                                                 xmlAction.MainNurseSkillSetID >= 0 ? SkillsPerID[xmlAction.MainNurseSkillSetID] : null,
                                                 xmlAction.AssistingDoctorSkillSetIDs != null ? xmlAction.AssistingDoctorSkillSetIDs.Select(p => SkillsPerID[p]).ToArray() : null,
                                                 xmlAction.AssistingNurseSkillSetIDs != null ? xmlAction.AssistingNurseSkillSetIDs.Select(p => SkillsPerID[p]).ToArray() : null,
                                                 xmlAction.DefinesCorrespondingDoctorStart,
                                                 xmlAction.DefinesCorrespondingDoctorEnd,
                                                 xmlAction.DefinesFacilityOccupationStart,
                                                 xmlAction.DefinesFacilityOccupationEnd,
                                                 xmlAction.BusyFactorMainDoctor,
                                                 xmlAction.BusyFactorAssistingDoctors,
                                                 xmlAction.BusyFactorMainNurse,
                                                 xmlAction.BusyFactorAssistingNurses,
                                                 xmlAction.IsPreemptable,
                                                 xmlAction.IsHold,
                                                 xmlAction.DefinesCorrespondingNurseStart,
                                                 xmlAction.DefinesCorrespondingNurseEnd));

            } // end foreach

            #endregion

            #region PatientClasses

            _patientClassPerXmlPatientClass = new Dictionary<XMLPatientClass, PatientClass>();

            // translates xml patient classes to actual patient classes and creates empirical distribution for the
            // paths associated with the patient class
            foreach (XMLPatientClass xmlPatientClass in xmlInput.PatientClasses)
            {
                PatientClass patientClass = new PatientClass(xmlPatientClass.Category, xmlPatientClass.Priority, xmlPatientClass.ArrivalMode);
                _patientClassPerXmlPatientClass.Add(xmlPatientClass, patientClass);
                if (xmlPatientClass.SinglePath < 0)
                    xmlPatientClass.PathDistribution = new EmpiricalDiscreteDistribution<int>(xmlPatientClass.PathIDProbabilities.ToArray(), xmlPatientClass.PathIDs.ToArray());
            } // end foreach

            // create a empricial distribution to handle multiple paths with different probabilities for a single patient class
            _patientClassDistribution = new EmpiricalDiscreteDistribution<XMLPatientClass>(xmlInput.PatientClasses.Select(p => p.Probability).ToArray(), xmlInput.PatientClasses.ToArray());

            #endregion

            #region Paths

            _xmlPathsPerID = new Dictionary<int, XMLPatientPath>();

            // emprirical distributions are linked to the paths that determine probabilities of certain steps
            // and possible admissions at the end of the path
            foreach (XMLPatientPath path in xmlInput.Paths)
            {
                _xmlPathsPerID.Add(path.ID, path);

                foreach (XMLPatientPathStep step in path.PathSteps)
                {
                    if (step.SingleAction == null)
                        step.StepDistribution = new EmpiricalDiscreteDistribution<string>(step.ActionProbabilities.ToArray(), step.Actions.ToArray());
                } // end foreach

                if(path.OutpatientAdmissions.Count > 0)
                {
                    path.OutpatientAdmissionDistribution = new EmpiricalDiscreteDistribution<XMLAdmission>(path.OutpatientAdmissions.Select(p => p.Probability).ToArray(), path.OutpatientAdmissions.ToArray());
                } // end if

                if (path.InpatientAdmissions.Count > 0)
                {
                    path.InpatientAdmissionDistribution = new EmpiricalDiscreteDistribution<XMLAdmission>(path.InpatientAdmissions.Select(p => p.Probability).ToArray(), path.InpatientAdmissions.ToArray());
                } // end if

            } // end foreach

            #endregion

        } // end of GenericXMLDepartmentInput

	    #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region PatientActionTime

        /// <summary>
        /// Returns the time consumed for an action type, patient and consuming resources
        /// </summary>
        /// <param name="patient">Patient of action</param>
        /// <param name="resources">Resource set of action</param>
        /// <param name="actionType">Type of action</param>
        /// <returns></returns>
        public abstract TimeSpan PatientActionTime(EntityPatient patient, ResourceSet resources, ActionTypeClass actionType);
    
        #endregion

        #region PatientClassDistribution

        private EmpiricalDiscreteDistribution<XMLPatientClass> _patientClassDistribution;

        /// <summary>
        /// Distribution for the different patient classes that might be assigned to new patients
        /// </summary>
        public EmpiricalDiscreteDistribution<XMLPatientClass> PatientClassDistribution
        {
            get
            {
                return _patientClassDistribution;
            }
        } // end of PatientClassDistribution

        #endregion

        #region PatientClassPerXmlPatientClass

        private Dictionary<XMLPatientClass, PatientClass> _patientClassPerXmlPatientClass;

        /// <summary>
        /// Stores actual patient classes per corresponding xml patient class
        /// </summary>
        public Dictionary<XMLPatientClass, PatientClass> PatientClassPerXmlPatientClass
        {
            get
            {
                return _patientClassPerXmlPatientClass;
            }
            set
            {
                _patientClassPerXmlPatientClass = value;
            }
        } // end of PatientClassPerXmlPatientClass

        #endregion

        #region XMLPathsPerID

        private Dictionary<int, XMLPatientPath> _xmlPathsPerID;

        /// <summary>
        /// Stores xml paths per id
        /// </summary>
        public Dictionary<int, XMLPatientPath> XMLPathsPerID
        {
            get
            {
                return _xmlPathsPerID;
            }
            set
            {
                _xmlPathsPerID = value;
            }
        } // end of XMLPathsPerID

        #endregion

        #region ConsiderAdmissionForPath

        private bool _considerAdmissionForPath;

        /// <summary>
        /// Flag if Admission types are considered for path selelction of new patients
        /// </summary>
        public bool ConsiderAdmissionForPath
        {
            get
            {
                return _considerAdmissionForPath;
            }
            set
            {
                _considerAdmissionForPath = value;
            }
        } // end of ConsiderAdmissionForPath

        #endregion

        #region ConsiderPatietnClassForPath

        private bool _considerPatientClassForPath;

        /// <summary>
        /// Flag if patient classes aare considered while path generation
        /// </summary>
        public bool ConsiderPatietnClassForPath
        {
            get
            {
                return _considerPatientClassForPath;
            }
            set
            {
                _considerPatientClassForPath = value;
            }
        } // end of ConsiderPatietnClassForPath

        #endregion

        #region StructuralAreaIdentifiers

        private string[] _structuralAreaIdentifiers;

        /// <summary>
        /// Stores all structual areas in department
        /// </summary>
        public string[] StructuralAreaIdentifiers
        {
            get
            {
                return _structuralAreaIdentifiers;
            }
        } // end of StructuralAreaIdentifiers

        #endregion

        #region XMLInput

        private XMLInputHealthCareDepartment _xmlInput;

        /// <summary>
        /// Underlying xml input
        /// </summary>
        public XMLInputHealthCareDepartment XMLInput
        {
            get
            {
                return _xmlInput;
            }
        } // end of XMLInput

        #endregion

        #region WaitingRoomPatientResourceAssignment

        private List<ResourceAssignmentPhysical<EntityWaitingArea>> _waitingRoomPatientResourceAssignment;

        /// <summary>
        /// Resource assingments of waiting areas for patiens
        /// </summary>
        public List<ResourceAssignmentPhysical<EntityWaitingArea>> WaitingRoomPatientResourceAssignment
        {
            get
            {
                return _waitingRoomPatientResourceAssignment;
            }
        } // end of WaitingRoomPatientResourceAssignment

        #endregion

        #region WaitingRoomStaffResourceAssignment

        private List<ResourceAssignmentPhysical<EntityWaitingArea>> _waitingRoomStaffResourceAssignment;

        /// <summary>
        /// Resource assingments of waiting areas for staff members
        /// </summary>
        public List<ResourceAssignmentPhysical<EntityWaitingArea>> WaitingRoomStaffResourceAssignment
        {
            get
            {
                return _waitingRoomStaffResourceAssignment;
            }
        } // end of WaitingRoomStaffResourceAssignment

        #endregion

        #region TreatmentFacilitiesAssingments

        private List<ResourceAssignmentPhysical<EntityTreatmentFacility>> _treatmentFacilitiesAssignments;

        /// <summary>
        /// Resource assignments for treatment facilities
        /// </summary>
        public List<ResourceAssignmentPhysical<EntityTreatmentFacility>> TreatmentFacilitiesAssingments
        {
            get
            {
                return _treatmentFacilitiesAssignments;
            }
            set
            {
                _treatmentFacilitiesAssignments = value;
            }
        } // end of TreatmentFacilities

        #endregion

        #region ActionTypes

        private Dictionary<string, ActionTypeClass> _actionTypes;

        /// <summary>
        /// Holds all action types that can be performed at department
        /// </summary>
        public Dictionary<string, ActionTypeClass> ActionTypes
        {
            get
            {
                return _actionTypes;
            }
            set
            {
                _actionTypes = value;
            }
        } // end of ActionTypes

        #endregion

        #region SkillsPerID

        private Dictionary<int, SkillSet> _skillsPerID;

        /// <summary>
        /// Holds all skills that are used for entities in the department per ID
        /// </summary>
        public Dictionary<int, SkillSet> SkillsPerID
        {
            get
            {
                return _skillsPerID;
            }
            set
            {
                _skillsPerID = value;
            }
        } // end of SkillsPerID

        #endregion

        #region DoctorsPerID

        private Dictionary<int, EntityDoctor> _doctorsPerID;

        /// <summary>
        /// In case xml doctor definitions were not done via skills, actual doctor entities are stored here
        /// </summary>
        public Dictionary<int, EntityDoctor> DoctorsPerID
        {
            get
            {
                return _doctorsPerID;
            }
        } // end of DoctorsPerID

        #endregion

        #region NursesPerID

        private Dictionary<int, EntityNurse> _nursesPerID;

        /// <summary>
        /// In case xml nurse definitions were not done via skills, actual nurse entities are stored here
        /// </summary>
        public Dictionary<int, EntityNurse> NursesPerID
        {
            get
            {
                return _nursesPerID;
            }
        } // end of NursesPerID

        #endregion

        #region StaffAvailabilityPerID

        private Dictionary<int, StaffAvailabilityPeriod> _staffAvailabilityPerID;

        /// <summary>
        /// Definitions of staff availability periods are held here
        /// </summary>
        public Dictionary<int, StaffAvailabilityPeriod> StaffAvailabilityPerID
        {
            get
            {
                return _staffAvailabilityPerID;
            }
        } // end of StaffAvailabilityPerID

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Interface Methods 
        //--------------------------------------------------------------------------------------------------

        #region StaffHandler

        private IStaffHandling _staffHandler;

        /// <summary>
        /// Defines the staff availability for the operational health care control unit
        /// </summary>
        public IStaffHandling StaffHandler
        {
            get
            {
                return _staffHandler;
            }
            set
            {
                _staffHandler = value;
            }
        } // end of StaffHandler

        #endregion

        #region GetStructuralAreaIdentifiers

        /// <summary>
        /// StructuralArea Identifiers 
        /// </summary>
        /// <returns></returns>
        public string[] GetStructuralAreaIdentifiers()
        {
            return StructuralAreaIdentifiers;
        } // end of GetStructuralAreaIdentifiers

        #endregion

        #region GetTreatmentFacilities

        /// <summary>
        /// Return TreatmentFacilities of control Unit 
        /// </summary>
        /// <returns></returns>
        public ResourceAssignmentPhysical<EntityTreatmentFacility>[] GetTreatmentFacilities()
        {
            return TreatmentFacilitiesAssingments.ToArray();
        } // end of GetTreatmentFacilities

        #endregion

        #region GetWaitingRoomPatients

        /// <summary>
        /// Return WaitingRooms for Patients of control Unit 
        /// </summary>
        /// <returns></returns>
        public ResourceAssignmentPhysical<EntityWaitingArea>[] GetWaitingRoomPatients()
        {
            return WaitingRoomPatientResourceAssignment.ToArray();
        } // end of GetWaitingRoomPatients

        #endregion

        #region GetWaitingRoomsStaff

        /// <summary>
        /// Return WaitingRooms for Staff of control Unit 
        /// </summary>
        /// <returns></returns>
        public ResourceAssignmentPhysical<EntityWaitingArea>[] GetWaitingRoomsStaff()
        {
            return WaitingRoomStaffResourceAssignment.ToArray();
        } // end of GetWaitingRoomsStaff

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region ParseEnum

        /// <summary>
        /// Helping function to parse enums
        /// </summary>
        /// <typeparam name="T">Enum type to be parsed</typeparam>
        /// <param name="value">String to parse</param>
        /// <returns></returns>
        protected T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        } // end of ParseEnumg

        #endregion

        #region GetCorePath

        /// <summary>
        /// Method to create the core of a patients path that does not differ over departments, Creates
        /// list of actions and inpatient or outpatient admissions
        /// </summary>
        /// <param name="patient">Patient the path is created for</param>
        /// <param name="admissionType">Admission type that might be the basis of path</param>
        /// <param name="actions">Out parameter for the list of actions of the final path</param>
        /// <param name="outpatientAdmission">Out parameter for the outpatient admission of the path</param>
        /// <param name="inpatientAdmission">Out parameter for the inpatient admission of the path</param>
        public void GetCorePath(EntityPatient patient, 
                                AdmissionType admissionType,
                                out List<ActionTypeClass> actions,
                                out Admission outpatientAdmission,
                                out Admission inpatientAdmission)
        {
            XMLPatientClass selectedPatientClass = null;

            foreach (XMLPatientClass xmlPatientClass in PatientClassPerXmlPatientClass.Keys)
            {
                if ((admissionType == null 
                        || xmlPatientClass.AdmissionType == admissionType.Identifier 
                        || !ConsiderAdmissionForPath)
                    && (patient.PatientClass.Equals(PatientClassPerXmlPatientClass[xmlPatientClass]) 
                        || !ConsiderPatietnClassForPath))
                {
                    selectedPatientClass = xmlPatientClass;
                    break;
                } // end if
            } // end foreach

            if (selectedPatientClass == null)
                throw new InvalidOperationException("Patient class and admission not found in XML-Patient Classes");

            actions = new List<ActionTypeClass>();

            XMLPatientPath selectedPath = null;

            if (selectedPatientClass.SinglePath >= 0)
            {
                selectedPath = XMLPathsPerID[selectedPatientClass.SinglePath];
            }
            else
            {
                selectedPath = XMLPathsPerID[selectedPatientClass.PathDistribution.GetRandomValue()];
            } // end if

            foreach (XMLPatientPathStep pathStep in selectedPath.PathSteps)
            {
                if (pathStep.SingleAction != null)
                {
                    actions.Add(ActionTypes[pathStep.SingleAction]);
                }
                else
                {
                    bool sampleFound;
                    string actionType = pathStep.StepDistribution.GetNullableRandomValue(out sampleFound);

                    if (!sampleFound)
                        continue;

                    actions.Add(ActionTypes[(string)actionType]);
                } // end if
            } // end foreach

            outpatientAdmission = null;
            inpatientAdmission = null;

            if (selectedPath.OutpatientAdmissions.Count > 0)
            {
                bool sampleFound;
                XMLAdmission selectedAdmission = selectedPath.OutpatientAdmissionDistribution.GetNullableRandomValue(out sampleFound);

                if (sampleFound)
                {
                    outpatientAdmission = new Admission(patient,
                        new OutpatientAdmissionTypes(selectedAdmission.AdmissionType),
                        selectedAdmission.MinDays,
                        selectedAdmission.MaxDays,
                        false,
                        patient.CorrespondingDoctor);
                } // end if
            } // end if

            if (selectedPath.InpatientAdmissions.Count > 0)
            {
                bool sampledFound;
                XMLAdmission selectedAdmission = selectedPath.InpatientAdmissionDistribution.GetNullableRandomValue(out sampledFound);

                if (sampledFound)
                {
                    inpatientAdmission = new Admission(patient,
                        new InpatientAdmissionTypes(selectedAdmission.AdmissionType,
                        false),
                        selectedAdmission.MinDays,
                        selectedAdmission.MaxDays,
                        false,
                        patient.CorrespondingDoctor);
                } // end if
            } // end if

        } // end of GetCorePath

        #endregion

    } // end of GenericXMLDepartmentInput
}

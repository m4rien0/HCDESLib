using Enums;
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.ControlUnits
{
    /// <summary>
    /// Base class for health care control untis, including controlled and handled staff members,
    /// admission types that are performed and other general functionality
    /// </summary>
    public abstract class ControlUnitHealthCare : ControlUnit
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="type">Control unit type, e.g. emergency or outpatient</param>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control unit if not root control of model</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        public ControlUnitHealthCare(ControlUnitType type,
                           string name,
                           ControlUnit parentControlUnit,
                           SimulationModel parentSimulationModel)
            : base(name,
                  parentControlUnit,
                  parentSimulationModel)
        {
            _controlUnitType = type;

            _controlledDoctors = new List<EntityDoctor>();
            _controlledNurses = new List<EntityNurse>();
        } // end of ControlUnitHealthCare

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // General Members
        //--------------------------------------------------------------------------------------------------

        #region ControlUnitType

        private ControlUnitType _controlUnitType;

        /// <summary>
        /// Type of control unit
        /// </summary>
        public ControlUnitType ControlUnitType
        {
            get
            {
                return _controlUnitType;
            }
        } // end of ControlUnitType

        #endregion ControlUnitType

        #region ChildHealthCareControlUnits

        private ControlUnitHealthCare[] _childHealthCareControlUnits;

        /// <summary>
        /// Casts child control units to health care control units for those who are of that type
        /// </summary>
        public ControlUnitHealthCare[] ChildHealthCareControlUnits
        {
            get
            {
                if (_childHealthCareControlUnits == null)
                    _childHealthCareControlUnits = ChildControlUnits.Where(p => p is ControlUnitHealthCare).Cast<ControlUnitHealthCare>().ToArray();

                return _childHealthCareControlUnits;
            }
        } // end of ChildHealthCareControlUnits

        #endregion ChildHealthCareControlUnits

        //--------------------------------------------------------------------------------------------------
        // Handled Treatments
        //--------------------------------------------------------------------------------------------------

        #region HandledOutpatientAdmissionTypes

        /// <summary>
        /// Each health care control unit must specify which outpatient admission types are handled (can be empty)
        /// </summary>
        public abstract OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes { get; }

        #endregion HandledOutpatientAdmissionTypes

        #region HandledInpatientAdmissionTypes

        /// <summary>
        /// Each health care control unit must specify which inpatient admission types are handled (can be empty)
        /// </summary>
        public abstract InpatientAdmissionTypes[] HandledInpatientAdmissionTypes { get; }

        #endregion HandledInpatientAdmissionTypes

        #region HandledSpecialFacilityAdmissionTypes

        /// <summary>
        /// Each health care control unit must specify which special facility services admission types are handled (can be empty)
        /// </summary>
        public abstract SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes { get; }

        #endregion HandledSpecialFacilityAdmissionTypes

        //--------------------------------------------------------------------------------------------------
        // HandledDoctors
        //--------------------------------------------------------------------------------------------------

        #region HandledDoctors

        /// <summary>
        /// Handled doctors include the own controlled doctors as well as doctors
        /// controlled by all control units of the sub-tree
        /// </summary>
        public List<EntityDoctor> HandledDoctors
        {
            get
            {
                List<EntityDoctor> handledDocs = new List<EntityDoctor>();

                if (ControlledEntities.ContainsKey(typeof(EntityDoctor)))
                    handledDocs.AddRange(ControlledDoctors);

                foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                {
                    handledDocs.AddRange(childControl.HandledDoctors);
                } // end foreach

                return handledDocs;
            }
        } // end of HandledDoctors

        #endregion HandledDoctors

        #region ControlledDoctors

        private List<EntityDoctor> _controlledDoctors;

        /// <summary>
        /// Own controlled doctors
        /// </summary>
        public List<EntityDoctor> ControlledDoctors
        {
            get
            {
                return _controlledDoctors;
            }
        } // end of ControlledDoctors

        #endregion ControlledDoctors

        //--------------------------------------------------------------------------------------------------
        // HandledNurses
        //--------------------------------------------------------------------------------------------------

        #region HandledNurses

        /// <summary>
        /// Handled  nurses include the own controlled nurses as well as nurses
        /// controlled by all control units of the sub-tree
        /// </summary>
        public IReadOnlyList<EntityNurse> HandledNurses
        {
            get
            {
                List<EntityNurse> handledDocs = new List<EntityNurse>();

                if (ControlledEntities.ContainsKey(typeof(EntityNurse)))
                    handledDocs.AddRange(ControlledNurses);

                foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                {
                    handledDocs.AddRange(childControl.HandledNurses);
                } // end foreach

                return handledDocs;
            }
        } // end of HandledNurses

        #endregion HandledNurses

        #region ControlledNurses

        private List<EntityNurse> _controlledNurses;

        /// <summary>
        /// Own controlled nurses
        /// </summary>
        public List<EntityNurse> ControlledNurses
        {
            get
            {
                return _controlledNurses;
            }
        } // end of ControlledNurses

        #endregion ControlledNurses

        //--------------------------------------------------------------------------------------------------
        // Change State
        //--------------------------------------------------------------------------------------------------

        #region AddEntity

        /// <summary>
        /// Overrides standard AddEntity method, as doctors and nurses are added
        /// to corresponding lists
        /// </summary>
        /// <param name="entity">Entity to add</param>
        public override void AddEntity(Entity entity)
        {
            base.AddEntity(entity);

            if (entity is EntityDoctor)
                _controlledDoctors.Add((EntityDoctor)entity);
            else if (entity is EntityNurse)
                _controlledNurses.Add((EntityNurse)entity);
        } // end of AddEntity

        #endregion AddEntity

        #region RemoveEntity

        /// <summary>
        /// Overrides standard RemoveEntity method, as doctors and nurses are removed
        /// from corresponding lists
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public override void RemoveEntity(Entity entity)
        {
            base.RemoveEntity(entity);

            if (entity is EntityDoctor)
                _controlledDoctors.Remove((EntityDoctor)entity);
            else if (entity is EntityNurse)
                _controlledNurses.Remove((EntityNurse)entity);
        } // end of RemoveEntity

        #endregion RemoveEntity

        //--------------------------------------------------------------------------------------------------
        // Find ControlUnits
        //--------------------------------------------------------------------------------------------------

        #region FindControlUnitForSpecialFacitlityService

        /// <summary>
        /// Looks for a control unit in the tree that handles a specific special facility service (e.g. diagnostics)
        /// request
        /// </summary>
        /// <param name="request">The special facility service request</param>
        /// <returns>Either a control unit that handles the request or null if no such control unit exists</returns>
        public ControlUnit FindControlUnitForSpecialFacitlityService(RequestSpecialFacilitiyService request)
        {
            if (request is RequestSpecialFacilitiyService)
            {
                if (!HandledSpecialFacilityAdmissionTypes.Contains(((RequestSpecialFacilitiyService)request).SpecialFacilityAdmissionTypes))
                {
                    if (ParentControlUnit != null)
                        return ((ControlUnitHealthCare)ParentControlUnit).FindControlUnitForSpecialFacitlityService((RequestSpecialFacilitiyService)request);
                    else
                        return null;
                } // end if

                if (this.ControlUnitType == Enums.ControlUnitType.SpecialFacilityModel
                    && this.HandledSpecialFacilityAdmissionTypes.Contains(((RequestSpecialFacilitiyService)request).SpecialFacilityAdmissionTypes))
                {
                    return this;
                }
                else
                {
                    foreach (ControlUnitHealthCare child in ChildHealthCareControlUnits.Where(p => p.HandledSpecialFacilityAdmissionTypes.Contains(((RequestSpecialFacilitiyService)request).SpecialFacilityAdmissionTypes)))
                    {
                        ControlUnit foundControl = child.FindControlUnitForSpecialFacitlityService(request);
                        if (foundControl != null)
                            return foundControl;
                    } // end foreach
                } // end if
            } // end if

            return null;
        } // end of FindControlUnitForSpecialFacitlityService

        #endregion FindControlUnitForSpecialFacitlityService

        #region FindControlForOutpatientAdmission

        /// <summary>
        /// Finds a control unit that handles a specific outpatient admission type
        /// </summary>
        /// <param name="admission">The admission type that is searched for</param>
        /// <returns>Either a control unit that handles the admission or null if no such control unit exists</returns>
        public ControlUnit FindControlForOutpatientAdmission(OutpatientAdmissionTypes admission)
        {
            if (!HandledOutpatientAdmissionTypes.Contains(admission))
                return null;

            if (!HandledOutpatientAdmissionTypes.Contains(admission))
            {
                if (ParentControlUnit != null)
                    return ((ControlUnitHealthCare)ParentControlUnit).FindControlForOutpatientAdmission(admission);
                else
                    return null;
            } // end if

            if (this.ControlUnitType == Enums.ControlUnitType.Outpatient
                && this.HandledOutpatientAdmissionTypes.Contains(admission))
            {
                return this;
            }
            else
            {
                foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits.Where(p => p.HandledOutpatientAdmissionTypes.Contains(admission)))
                {
                    ControlUnit foundControl = childControl.FindControlForOutpatientAdmission(admission);
                    if (foundControl != null)
                        return foundControl;
                } // end foreach
            } // end if

            return null;
        } // end of FindControlForOutpatientAdmission

        #endregion FindControlForOutpatientAdmission

        #region FindControlForInpatientAdmission

        /// <summary>
        /// Finds a control unit that handles a specific inpatient admission type
        /// </summary>
        /// <param name="admission">The admission type that is searched for</param>
        /// <returns>Either a control unit that handles the admission or null if no such control unit exists</returns>
        public ControlUnit FindControlForInpatientAdmission(InpatientAdmissionTypes admission)
        {
            if (!HandledInpatientAdmissionTypes.Contains(admission))
                return null;

            if (this.ControlUnitType == Enums.ControlUnitType.Inpatient
                && this.HandledInpatientAdmissionTypes.Contains(admission))
            {
                return this;
            }
            else
            {
                foreach (ControlUnitHealthCare childControl in ChildHealthCareControlUnits)
                {
                    ControlUnit foundControl = childControl.FindControlForInpatientAdmission(admission);
                    if (foundControl != null)
                        return foundControl;
                } // end foreach
            } // end if

            return null;
        } // end of FindControlForInpatientAdmission

        #endregion FindControlForInpatientAdmission

        //--------------------------------------------------------------------------------------------------
        // Find SkillSets
        //--------------------------------------------------------------------------------------------------

        #region FindDoctorWithSkillSet

        /// <summary>
        /// Looks for doctors in the handled doctors (entire sub-tree) that satisfy a skill set
        /// </summary>
        /// <param name="skillSet">Skill set that is required for the searched doctors</param>
        /// <returns>A list of doctors that satisfy the specified skill set</returns>
        public List<EntityDoctor> FindDoctorWithSkillSet(SkillSet skillSet)
        {
            List<EntityDoctor> fittingDocs = new List<EntityDoctor>();

            foreach (EntityDoctor doc in HandledDoctors)
            {
                if (doc.SatisfiesSkillSet(skillSet))
                    fittingDocs.Add(doc);
            } // end foreach

            return fittingDocs;
        } // end of FindDoctorWithSkillSet

        #endregion FindDoctorWithSkillSet

        //--------------------------------------------------------------------------------------------------
        // Control Mechansisms
        //--------------------------------------------------------------------------------------------------

        #region PatientPriorityPlusFIFO

        /// <summary>
        /// A standardized control mechanism to pick requests from a list that
        /// first picks in order of patient priorities and breaks ties according
        /// to FIFO principle
        /// </summary>
        /// <typeparam name="T">Type of requests</typeparam>
        /// <typeparam name="U">Type of action for request</typeparam>
        /// <param name="requests">List of requests to choose from</param>
        /// <returns>The highest priority request that is longest waiting</returns>
        public T PatientPriorityPlusFIFO<T, U>(List<T> requests)
            where U : ActionTypeClass
            where T : RequestHealthCareAction<U>
        {
            if (requests.Count == 0)
                return null;

            // determine the lowest grade (equal to highest priority
            int minTriageGrade = requests.Select(p => p.Patient.PatientClass.Priority).Aggregate((curmin, x) => (curmin == null || (x) < curmin ? x : curmin));

            // filter all requests of that grade
            List<T> requestsWithHighestPriority = requests.Where(p => p.Patient.PatientClass.Priority == minTriageGrade).ToList();

            // return request that was filed first
            return requestsWithHighestPriority.Aggregate((curmin, x) => (curmin == null || (x.TimeRequested) < curmin.TimeRequested ? x : curmin));
        } // end of

        #endregion PatientPriorityPlusFIFO
    } // end of ControlUnitHealthCare
}
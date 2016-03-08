using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths
{
    #region ActionTypeClass

    /// <summary>
    /// This class describes an action type for a health care activity.
    /// A set of attributes typical to health care actions is provided
    /// </summary>
    public class ActionTypeClass
    {
         //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that sets almost all attributes to default values
        /// </summary>
        /// <param name="type">Type of actions, e.g. treatment, assessment or diagnostics</param>
        /// <param name="identifier">Identifier that is a sub-categorization of the type, e.g. XRay for diagnostics</param>
        public ActionTypeClass(
            string type,
            string identifier)
        {
            _type = type;
            _name = identifier;
        } // end of EmergencyActionType

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="actionType">Action type class to copy</param>
        public ActionTypeClass(ActionTypeClass actionType)
        {
            _type = actionType.Type;
            _name = actionType.Identifier;
            _isHold = actionType.IsHold;
            _isPreemptable = actionType.IsPreemptable;

            _facilityRequirements = actionType.FacilityRequirements;
            _mainDoctorRequirements = actionType.MainDoctorRequirements;
            _mainNurseRequirements = actionType.MainNurseRequirements;
            _assistingDoctorRequirements = actionType.AssistingDoctorRequirements;
            _assistingNurseRequirements = actionType.AssistingNurseRequirements;
            _definesCorrespondingDocStart = actionType.DefinesCorrespondingDocStart;
            _definesCorrespondingDocEnd = actionType.DefinesCorrespondingDocEnd;
            _definesCorrespondingNurseStart = actionType.DefinesCorrespondingNurseStart;
            _definesCorrespondingNurseEnd = actionType.DefinesCorrespondingNurseEnd;
            _busFactorDoctor = actionType.BusyFactorDoctor;
            _busFactorNurse = actionType.BusyFactorNurse;
            _busyFactorAssistingDoctors = actionType.BusyFactorAssistingDoctors;
            _busyFactorAssistingNurses = actionType.BusyFactorAssistingNurses;
            _definesFacilitiyOccupationStart = actionType.DefinesFacilitiyOccupationStart;
            _definesFacilitiyOccupationEnd = actionType.DefinesFacilitiyOccupationEnd;

        } // end of ActionTypeClass

        /// <summary>
        /// Exhaustive constructor where all attributes are passed
        /// </summary>
        /// <param name="type">Type of actions, e.g. treatment, assessment or diagnostics</param>
        /// <param name="identifier">Identifier that is a sub-categorization of the type, e.g. XRay for diagnostics</param>
        /// <param name="facilitySkill">Defines the required facility skills for the action</param>
        /// <param name="mainDoctorSkill">Defines the required skill of the main doctor for the action</param>
        /// <param name="mainNurseSkill">Defines the required skill of the main nurse for the action</param>
        /// <param name="assistingDoctorSkills">Defines the required skills of assisting doctors for the action</param>
        /// <param name="assistingNurseSkills">Defines the required skills of assisting nurses for the action</param>
        /// <param name="definesCorrespondingDocStart">True if from this action on the main doctor is the corresponding doctor of the patient</param>
        /// <param name="definesCorrespondingDocEnd">True if this action clears the corresponding doctor for the patient</param>
        /// <param name="definesFacilityOccupationStart">True if from this action on the facility is blocked for the patient</param>
        /// <param name="definesFacilityOccupationEnd">True if from this action on the facility blocking is released</param>
        /// <param name="busyFactorMainDoc">Defines the capacity of the main doctor that is consumed by the action</param>
        /// <param name="busyFacorAssistingDocs">Defines the capacities of the assisting doctors that is consumed by the action</param>
        /// <param name="busyFactorMainNurse">Defines the capacity of the main nurse that is consumed by the action</param>
        /// <param name="busyFactorAssisitingNurses">Defines the capacities of the assisting nurses that is consumed by the action</param>
        /// <param name="definesCorrespondingNurseStart">True if from this action on the main nurse is the corresponding doctor of the patient</param>
        /// <param name="definesCorrespondingNurseEnd">True if this action clears the corresponding nurse for the patient</param>
        /// <param name="isHolding">True if the action requires holding, i.e. is only ended when the next action starts and no waiting is allowed inbetween</param>
        /// <param name="isPreemptable">True if the action can be interrupted</param>
        public ActionTypeClass(
            string type,
            string identifier,
            SkillSet facilitySkill,
            SkillSet mainDoctorSkill,
            SkillSet mainNurseSkill,
            SkillSet[] assistingDoctorSkills,
            SkillSet[] assistingNurseSkills,
            bool definesCorrespondingDocStart,
            bool definesCorrespondingDocEnd,
            bool definesFacilityOccupationStart,
            bool definesFacilityOccupationEnd,
            double busyFactorMainDoc,
            double[] busyFacorAssistingDocs,
            double busyFactorMainNurse,
            double[] busyFactorAssisitingNurses,
            bool definesCorrespondingNurseStart,
            bool definesCorrespondingNurseEnd,
            bool isHolding,
            bool isPreemptable)
        {
            _type = type;
            _name = identifier;
            _isHold = isHolding;
            _isPreemptable = isPreemptable;

            _facilityRequirements = facilitySkill;
            _mainDoctorRequirements = mainDoctorSkill;
            _mainNurseRequirements = mainNurseSkill;
            _assistingDoctorRequirements = assistingDoctorSkills;
            _assistingNurseRequirements = assistingNurseSkills;
            _definesCorrespondingDocStart = definesCorrespondingDocStart;
            _definesCorrespondingDocEnd = definesCorrespondingDocEnd;
            _definesCorrespondingNurseStart = definesCorrespondingNurseStart;
            _definesCorrespondingNurseEnd = definesCorrespondingNurseEnd;
            _busFactorDoctor = busyFactorMainDoc;
            _busFactorNurse = busyFactorMainNurse;
            _busyFactorAssistingDoctors = busyFacorAssistingDocs;
            _busyFactorAssistingNurses = busyFactorAssisitingNurses;
            _definesFacilitiyOccupationStart = definesFacilityOccupationStart;
            _definesFacilitiyOccupationEnd = definesFacilityOccupationEnd;

            if (BusyFactorAssistingDoctors == null && assistingDoctorSkills != null)
            {
                _busyFactorAssistingDoctors = assistingDoctorSkills.Select(p => 1d).ToArray();
            } // end

            if (BusyFactorAssistingNurses == null && assistingNurseSkills != null)
            {
                _busyFactorAssistingNurses = assistingNurseSkills.Select(p => 1d).ToArray();
            } // end

        } // end of Emergency

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Properties 
        //--------------------------------------------------------------------------------------------------

        #region Type

        private string _type;

        /// <summary>
        /// Type of actions, e.g. treatment, assessment or diagnostics
        /// </summary>
        public string Type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        } // end of Type

        #endregion

        #region Identifier

        private string _name;

        /// <summary>
        /// Identifier that is a sub-categorization of the type, e.g. XRay for diagnostics
        /// </summary>
        public string Identifier
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        } // end of Name

        #endregion

        #region FacilityRequirements

        private SkillSet _facilityRequirements;

        /// <summary>
        /// Defines the required facility skills for the action
        /// </summary>
        public SkillSet FacilityRequirements
        {
            get
            {
                return _facilityRequirements;
            }
            set
            {
                _facilityRequirements = value;
            }
        } // end of FacilityRequirements

        #endregion

        #region MainDoctorRequirements

        private SkillSet _mainDoctorRequirements;

        /// <summary>
        /// Defines the required skill of the main doctor for the action
        /// </summary>
        public SkillSet MainDoctorRequirements
        {
            get
            {
                return _mainDoctorRequirements;
            }
            set
            {
                _mainDoctorRequirements = value;
            }
        } // end of MainDoctorRequirements

        #endregion

        #region AssistingDoctorRequirements

        private SkillSet[] _assistingDoctorRequirements;

        /// <summary>
        /// Defines the required skills of assisting doctors for the action
        /// </summary>
        public SkillSet[] AssistingDoctorRequirements
        {
            get
            {
                return _assistingDoctorRequirements;
            }
            set
            {
                _assistingDoctorRequirements = value;
            }
        } // end of AssistingDoctorRequirements

        #endregion

        #region MainNurseRequirements

        private SkillSet _mainNurseRequirements;

        /// <summary>
        /// Defines the required skill of the main nurse for the action
        /// </summary>
        public SkillSet MainNurseRequirements
        {
            get
            {
                return _mainNurseRequirements;
            }
            set
            {
                _mainNurseRequirements = value;
            }
        } // end of MainNurseRequirements

        #endregion

        #region AssistingNurseRequirements

        private SkillSet[] _assistingNurseRequirements;

        /// <summary>
        /// Defines the required skills of assisting nurses for the action
        /// </summary>
        public SkillSet[] AssistingNurseRequirements
        {
            get
            {
                return _assistingNurseRequirements;
            }
            set
            {
                _assistingNurseRequirements = value;
            }
        } // end of AssistingNurseRequirements

        #endregion

        #region BusyFactorNurse

        private double _busFactorNurse;

        /// <summary>
        /// Defines the capacity of the main nurse that is consumed by the action
        /// </summary>
        public double BusyFactorNurse
        {
            get
            {
                return _busFactorNurse;
            }
            set
            {
                _busFactorNurse = value;
            }
        } // end of BusyFactorNurse

        #endregion

        #region BusyFactorAssistingDoctors

        private double[] _busyFactorAssistingDoctors;

        /// <summary>
        /// Defines the capacities of the assisting doctors that is consumed by the action
        /// </summary>
        public double[] BusyFactorAssistingDoctors
        {
            get
            {
                return _busyFactorAssistingDoctors;
            }
        } // end of BusyFactorAssistingDoctors

        #endregion

        #region BusyFactorDoctor

        private double _busFactorDoctor;

        /// <summary>
        /// Defines the capacity of the main doctor that is consumed by the action
        /// </summary>
        public double BusyFactorDoctor
        {
            get
            {
                return _busFactorDoctor;
            }
            set
            {
                _busFactorDoctor = value;
            }
        } // end of BusyFactorDoctor

        #endregion

        #region BusyFactorAssistingNurses

        private double[] _busyFactorAssistingNurses;

        /// <summary>
        /// Defines the capacities of the assisting nurses that is consumed by the action
        /// </summary>
        public double[] BusyFactorAssistingNurses
        {
            get
            {
                return _busyFactorAssistingNurses;
            }
        } // end of BusyFactorAssistingNurses

        #endregion

        #region DefinesCorrespondingDocStart

        private bool _definesCorrespondingDocStart;

        /// <summary>
        /// True if from this action on the main doctor is the corresponding doctor of the patient
        /// </summary>
        public bool DefinesCorrespondingDocStart
        {
            get
            {
                return _definesCorrespondingDocStart;
            }
            set
            {
                _definesCorrespondingDocStart = value;
            }
        } // end of DefinesCorrespondingDoc

        #endregion

        #region DefinesCorrespondingDocEnd

        private bool _definesCorrespondingDocEnd;

        /// <summary>
        /// True if this action clears the corresponding doctor for the patient
        /// </summary>
        public bool DefinesCorrespondingDocEnd
        {
            get
            {
                return _definesCorrespondingDocEnd;
            }
            set
            {
                _definesCorrespondingDocEnd = value;
            }
        } // end of DefinesCorrespondingDoc

        #endregion

        #region DefinesCorrespondingNurseStart

        private bool _definesCorrespondingNurseStart;

        /// <summary>
        /// True if from this action on the main doctor is the corresponding nurse of the patient
        /// </summary>
        public bool DefinesCorrespondingNurseStart
        {
            get
            {
                return _definesCorrespondingNurseStart;
            }
            set
            {
                _definesCorrespondingNurseStart = value;
            }
        } // end of DefinesCorrespondingNurse

        #endregion

        #region DefinesCorrespondingNurseEnd

        private bool _definesCorrespondingNurseEnd;

        /// <summary>
        /// True if this action clears the corresponding nurse for the patient
        /// </summary>
        public bool DefinesCorrespondingNurseEnd
        {
            get
            {
                return _definesCorrespondingNurseEnd;
            }
            set
            {
                _definesCorrespondingNurseEnd = value;
            }
        } // end of DefinesCorrespondingNurse

        #endregion

        #region DefinesFacilitiyOccupationStart

        private bool _definesFacilitiyOccupationStart;

        /// <summary>
        /// True if from this action on the facility is blocked for the patient
        /// </summary>
        public bool DefinesFacilitiyOccupationStart
        {
            get
            {
                return _definesFacilitiyOccupationStart;
            }
        } // end of DefinesFacilitiyOccupationStart

        #endregion

        #region DefinesFacilitiyOccupationEnd

        private bool _definesFacilitiyOccupationEnd;

        /// <summary>
        /// True if from this action on the facility blocking is released
        /// </summary>
        public bool DefinesFacilitiyOccupationEnd
        {
            get
            {
                return _definesFacilitiyOccupationEnd;
            }
        } // end of DefinesFacilitiyOccupationEnd

        #endregion

        #region IsPreemptable

        private bool _isPreemptable;

        /// <summary>
        /// True if the action can be interrupted
        /// </summary>
        public bool IsPreemptable
        {
            get
            {
                return _isPreemptable;
            }
            set
            {
                _isPreemptable = value;
            }
        } // end of IsPreemptable

        #endregion

        #region IsHold

        private bool _isHold;

        /// <summary>
        /// True if the action requires holding, i.e. is only ended when the next action starts and no waiting is allowed inbetween
        /// </summary>
        public bool IsHold
        {
            get
            {
                return _isHold;
            }
            set
            {
                _isHold = value;
            }
        } // end of IsHold

        #endregion

    } // end of BaseActionTypeClass

    #endregion

    #region EmergencyActionTypeClass

    /// <summary>
    /// Special class for emergency actions
    /// </summary>
    public class EmergencyActionTypeClass : ActionTypeClass
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that sets almost all attributes to default values
        /// </summary>
        /// <param name="type">Type of actions, e.g. treatment, assessment or diagnostics</param>
        /// <param name="identifier">Identifier that is a sub-categorization of the type, e.g. FirstAssessment for assessment</param>
        public EmergencyActionTypeClass(string type,
            string identifier) :base(type, identifier)
        {
        } // end of EmergencyActionType

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="actionType">Action type class to copy</param>
        public EmergencyActionTypeClass(ActionTypeClass actionType) : base(actionType)
        {
        } // end of Emergency

        #endregion
       
        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return Identifier;
        } // end of ToString

        #endregion

    } // end of EmergencyActionType

    #endregion

    #region SpecialServiceActionTypeClass

    /// <summary>
    /// Special class for special service actions
    /// </summary>
    public class SpecialServiceActionTypeClass : ActionTypeClass
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that sets almost all attributes to default values
        /// </summary>
        /// <param name="type">Type of actions, e.g. treatment, assessment or diagnostics</param>
        /// <param name="identifier">Identifier that is a sub-categorization of the type, e.g. XRay for diagnostics</param>
        public SpecialServiceActionTypeClass(string type,
            string identifier)
            : base(type, identifier)
        {
        } // end of EmergencyActionType

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="actionType">Action type class to copy</param>
        public SpecialServiceActionTypeClass(ActionTypeClass actionType)
            : base(actionType)
        {
        } // end of Emergency

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return Identifier;
        } // end of ToString

        #endregion

    } // end of EmergencyActionType

    #endregion

    #region OutpatientActionTypeClass

    /// <summary>
    /// Special class for outpatient actions
    /// </summary>
    public class OutpatientActionTypeClass : ActionTypeClass
    { 
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor that sets almost all attributes to default values
        /// </summary>
        /// <param name="type">Type of actions, e.g. treatment, assessment or diagnostics</param>
        /// <param name="identifier">Identifier that is a sub-categorization of the type, e.g. FirstAssessment for assessment</param>
        public OutpatientActionTypeClass(string type,
            string identifier)
            : base(type, identifier)
        {
        } // end of OutpatientActionTypeClass

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="actionType">Action type class to copy</param>
        public OutpatientActionTypeClass(ActionTypeClass actionType)
            : base(actionType)
        {
        } // end of OutpatientActionTypeClass

        #endregion

    } // end of OutpatientActionTypeClass

    #endregion
}

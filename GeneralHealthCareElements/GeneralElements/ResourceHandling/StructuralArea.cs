using GeneralHealthCareElements.Entities;
using System.Collections.Generic;

namespace GeneralHealthCareElements.ResourceHandling
{
    /// <summary>
    /// Class to summarize physical resources that define a structural area within the model
    /// </summary>
    public class StructuralArea
    {
        #region Constructor

        /// <summary>
        /// Empty constructor that initializes no resources
        /// </summary>
        /// <param name="identifier">Identifier of structural area</param>
        public StructuralArea(string identifier)
        {
            _identifier = identifier;
            _waitingAreaPatients = new List<EntityWaitingArea>();
            _treatmentFacilities = new List<EntityTreatmentFacility>();
            _multiplePatientsTreatmentFacilities = new List<EntityMultiplePatientTreatmentFacility>();
        } // end of StructuralArea

        /// <summary>
        /// Basic constructor intializing all types of resources
        /// </summary>
        /// <param name="identifier">Identifier of structural area</param>
        /// <param name="waitingAreaPatients">List of waiting areas in the structural area to be used by patients</param>
        /// <param name="multiplePatientsTreatmentFacilities">Treatment facilities of type EntityMultiplePatientTreatmentFacility</param>
        /// <param name="treatmentFacilities">Treatment facilities that are not for multiple patients</param>
        /// <param name="staffWaitingRoom">Staff waiting area, currently only one allowed per structural area</param>
        public StructuralArea(string identifier,
                              List<EntityWaitingArea> waitingAreaPatients,
                              List<EntityMultiplePatientTreatmentFacility> multiplePatientsTreatmentFacilities,
                              List<EntityTreatmentFacility> treatmentFacilities,
                              EntityWaitingArea staffWaitingRoom)
        {
            _identifier = identifier;
            _waitingAreaPatients = waitingAreaPatients;
            _multiplePatientsTreatmentFacilities = multiplePatientsTreatmentFacilities;
            _treatmentFacilities = treatmentFacilities;
            _staffWaitingArea = staffWaitingRoom;
        } // end of StructuralArea

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Identifier

        private string _identifier;

        /// <summary>
        /// Identifier of structural area
        /// </summary>
        public string Identifier
        {
            get
            {
                return _identifier;
            }
            set
            {
                _identifier = value;
            }
        } // end of Identifier

        #endregion Identifier

        #region WaitingAreasPatients

        private List<EntityWaitingArea> _waitingAreaPatients;

        /// <summary>
        /// List of waiting areas in the structural area to be used by patients
        /// </summary>
        public List<EntityWaitingArea> WaitingAreasPatients
        {
            get
            {
                return _waitingAreaPatients;
            }
            set
            {
                _waitingAreaPatients = value;
            }
        } // end of WaitingAreasPatients

        #endregion WaitingAreasPatients

        #region MultiplePatientTreatmentFacilities

        private List<EntityMultiplePatientTreatmentFacility> _multiplePatientsTreatmentFacilities;

        /// <summary>
        /// Treatment facilities of type EntityMultiplePatientTreatmentFacility
        /// </summary>
        public List<EntityMultiplePatientTreatmentFacility> MultiplePatientTreatmentFacilities
        {
            get
            {
                return _multiplePatientsTreatmentFacilities;
            }
            set
            {
                _multiplePatientsTreatmentFacilities = value;
            }
        } // end of MultiplePatientTreatmentFacilities

        #endregion MultiplePatientTreatmentFacilities

        #region TreatmentFacilities

        private List<EntityTreatmentFacility> _treatmentFacilities;

        /// <summary>
        /// Treatment facilities that are not for multiple patients
        /// </summary>
        public List<EntityTreatmentFacility> TreatmentFacilities
        {
            get
            {
                return _treatmentFacilities;
            }
            set
            {
                _treatmentFacilities = value;
            }
        } // end of TreatmentFacilities

        #endregion TreatmentFacilities

        #region StaffWaitingRoom

        private EntityWaitingArea _staffWaitingArea;

        /// <summary>
        /// Staff waiting area, currently only one allowed per structural area
        /// </summary>
        public EntityWaitingArea StaffWaitingRoom
        {
            get
            {
                return _staffWaitingArea;
            }
            set
            {
                _staffWaitingArea = value;
            }
        } // end of StaffWaitingRoom

        #endregion StaffWaitingRoom
    } // end of StructuralArea
}
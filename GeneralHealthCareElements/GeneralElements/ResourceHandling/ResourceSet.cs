using GeneralHealthCareElements.Entities;
using SimulationCore.SimulationClasses;
using System;

namespace GeneralHealthCareElements.ResourceHandling
{
    /// <summary>
    /// Class to summarize the resources participating in a health care action
    /// </summary>
    public class ResourceSet
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Empty constructor for a null resource set
        /// </summary>
        public ResourceSet()
        {
        } // end of ResourceSet

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="mainDoc">Main doctor of health care action</param>
        /// <param name="mainNurse">Main nurse of health care action</param>
        /// <param name="facility">Main facility of health care action</param>
        /// <param name="assistingDoctors">Assisting doctors of health care action</param>
        /// <param name="assistingNurses">Assisting nurses of health care action</param>
        public ResourceSet(EntityDoctor mainDoc,
            EntityNurse mainNurse,
            EntityTreatmentFacility facility,
            EntityDoctor[] assistingDoctors = null,
            EntityNurse[] assistingNurses = null)
        {
            _mainDoc = mainDoc;
            _mainNurse = mainNurse;
            _treatmentFacility = facility;
            _assistingDoctors = assistingDoctors;
            _assistingNurses = assistingNurses;
        } // end of ResourceSet

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region MainDoctor

        private EntityDoctor _mainDoc;

        /// <summary>
        /// Main doctor of health care action
        /// </summary>
        public EntityDoctor MainDoctor
        {
            get
            {
                return _mainDoc;
            }
            set
            {
                _mainDoc = value;
            }
        } // end of MainDoc

        #endregion MainDoctor

        #region MainNurse

        private EntityNurse _mainNurse;

        /// <summary>
        /// Main nurse of health care action
        /// </summary>
        public EntityNurse MainNurse
        {
            get
            {
                return _mainNurse;
            }
            set
            {
                _mainNurse = value;
            }
        } // end of MainNurse

        #endregion MainNurse

        #region AssistingDoctors

        private EntityDoctor[] _assistingDoctors;

        /// <summary>
        /// Assisting doctors of health care action
        /// </summary>
        public EntityDoctor[] AssistingDoctors
        {
            get
            {
                return _assistingDoctors;
            }
            set
            {
                _assistingDoctors = value;
            }
        } // end of AssistingDoctors

        #endregion AssistingDoctors

        #region AssistingNurses

        private EntityNurse[] _assistingNurses;

        /// <summary>
        /// Assisting nurses of health care action
        /// </summary>
        public EntityNurse[] AssistingNurses
        {
            get
            {
                return _assistingNurses;
            }
            set
            {
                _assistingNurses = value;
            }
        } // end of AssistingNurses

        #endregion AssistingNurses

        #region TreatmentFacilities

        private EntityTreatmentFacility _treatmentFacility;

        /// <summary>
        /// Main facility of health care action
        /// </summary>
        public EntityTreatmentFacility TreatmentFacility
        {
            get
            {
                return _treatmentFacility;
            }
            set
            {
                _treatmentFacility = value;
            }
        } // end of TreatmentFacilities

        #endregion TreatmentFacilities

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region StopCurrentActivities

        /// <summary>
        /// Method to stop all activities the staff resources are involved in.
        /// To be used before the start of a new action.
        /// </summary>
        /// <param name="time">The time activities are stopped</param>
        /// <param name="simEngine">The simulation engine responsible for stopping</param>
        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            if (MainDoctor != null)
                MainDoctor.StopCurrentActivities(time, simEngine);

            if (AssistingDoctors != null)
            {
                foreach (EntityDoctor doc in AssistingDoctors)
                {
                    doc.StopCurrentActivities(time, simEngine);
                } // end foreach
            } // end if

            if (MainNurse != null)
                MainNurse.StopCurrentActivities(time, simEngine);

            if (AssistingNurses != null)
            {
                foreach (EntityNurse nurse in AssistingNurses)
                {
                    nurse.StopCurrentActivities(time, simEngine);
                } // end foreach
            } // end if
        } // end of StopCurrentActivities

        #endregion StopCurrentActivities
    } // end of
}
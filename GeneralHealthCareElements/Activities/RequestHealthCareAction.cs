using Enums;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Activities
{
    /// <summary>
    /// Basic request for health care actions
    /// </summary>
    /// <typeparam name="T">Action type that is requested, e.g. emergency action types</typeparam>
    public class RequestHealthCareAction<T> : ActivityRequest where T : ActionTypeClass
    {
        #region Constructor

        /// <summary>
        /// Basic constructor for request
        /// </summary>
        /// <param name="patient">Patient requesting the service</param>
        /// <param name="degreeOfCompletion">In case a activity was pre-empted</param>
        /// <param name="actionType">The requested action type</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="resources">There maybe a pre-defined resource set for the activity, e.g. corresponding doctors</param>
        public RequestHealthCareAction(EntityPatient patient,
            double degreeOfCompletion,
            T actionType,
            DateTime time,
            ResourceSet resources)
            : base("ActivityHealthCareAction", patient.ToArray(), time)
        {
            _patient = patient;
            _actionType = actionType;
            _degreeOfCompletion = degreeOfCompletion;
            _resourceSet = resources;
            _isAssistedDoctor = (actionType.AssistingDoctorRequirements != null);
            _isAssistedNurse = (actionType.AssistingNurseRequirements != null);
            _staffRequested = false;
        } // end of RequestEmergencyAssistedTreatment

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region Patient

        private EntityPatient _patient;

        /// <summary>
        /// Patient requesting service
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
        /// Maybe pre-defined resource set
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

        #region ActionType

        private T _actionType;

        /// <summary>
        /// Requested action type
        /// </summary>
        public T ActionType
        {
            get
            {
                return _actionType;
            }
        } // end of TreatmentType

        #endregion

        #region ReadyForDispatch

        private bool _readyForDispatch;

        /// <summary>
        /// Flag if the request is ready for dispatching, e.g. not the case if the patient waits for
        /// an extern consultant
        /// </summary>
        public bool ReadyForDispatch
        {
            get
            {
                return _readyForDispatch;
            }
            set
            {
                _readyForDispatch = value;
            }
        } // end of ReadyForDispatch

        #endregion

        #region DegreeOfCompletion

        private double _degreeOfCompletion;

        /// <summary>
        /// Degree of completion for interrupted activities
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

        #region StaffRequested

        private bool _staffRequested;

        /// <summary>
        /// Flag if extern staff has already been requested
        /// </summary>
        public bool StaffRequested
        {
            get
            {
                return _staffRequested;
            }
            set
            {
                _staffRequested = value;
            }
        } // end of StaffRequested

        #endregion

        #region IsResume

        /// <summary>
        /// Flag if the request is a resuming activity for pre-empted activities
        /// </summary>
        public bool IsResume
        {
            get
            {
                return Math.Abs(DegreeOfCompletion - 1) > Helpers<double>.GetNumbericalPrecission();
            }
        } // end of IsResume

        #endregion

        #region IsAssistedDoctor

        private bool _isAssistedDoctor;

        /// <summary>
        /// Flag if there are assisting doctors for the main doctor in the required resource set
        /// </summary>
        public bool IsAssistedDoctor
        {
            get
            {
                return _isAssistedDoctor;
            }
        } // end of IsAssistedDoctor

        #endregion

        #region IsAssistedNurse

        private bool _isAssistedNurse;

        /// <summary>
        /// Flag if there are assisting nurses for the main nurse in the required resource set
        /// </summary>
        public bool IsAssistedNurse
        {
            get
            {
                return _isAssistedNurse;
            }
        } // end of IsAssistedNurse

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return ActionType.Identifier + ": Patint: " + Patient.Identifier;
        } // end of ToString

        #endregion

    } // end of RequestEmergencyAssistedTreatment
}

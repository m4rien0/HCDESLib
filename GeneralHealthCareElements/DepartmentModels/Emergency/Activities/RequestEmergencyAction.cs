using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Request of an emergency action, has no own functionality but prevents too much
    /// casting in rule definitions of control units
    /// </summary>
    public class RequestEmergencyAction : RequestHealthCareAction<EmergencyActionTypeClass>
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="patient">Patient requesting the service</param>
        /// <param name="degreeOfCompletion">In case a activity was pre-empted</param>
        /// <param name="actionType">The requested action type</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="resources">There maybe a pre-defined resource set for the activity, e.g. corresponding doctors</param>
        public RequestEmergencyAction(EntityPatient patient,
            double degreeOfCompletion,
            EmergencyActionTypeClass actionType,
            DateTime time,
            ResourceSet resources)
            : base(patient, degreeOfCompletion, actionType, time, resources)
        {
        } // end of RequestEmergencyAssistedTreatment

        #endregion

    } // end of RequestEmergencyAction
}

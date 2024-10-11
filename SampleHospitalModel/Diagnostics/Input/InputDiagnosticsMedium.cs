using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleHospitalModel.Diagnostics
{
    /// <summary>
    /// Sample input for a diagnostic control extending the general xml input for models with
    /// a booking model
    /// </summary>
    public class InputDiagnostics : GenericXMLHCDepInputWithAdmissionAndBookingModel, IInputSpecialFacility
    {
        //--------------------------------------------------------------------------------------------------
        //  Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic contructor
        /// </summary>
        /// <param name="xmlInput">Xml input that defines most of input</param>
        public InputDiagnostics(XMLInputHealthCareWithWaitingList xmlInput)
            : base(xmlInput)
        {
        } // end of InputEmergencyMedium

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Interface Methods
        //--------------------------------------------------------------------------------------------------

        #region PatientSpecialActionTime

        /// <summary>
        /// Activity duriations, here modeled with exponential distributions independent from the patient conditions,
        /// just dependent on the action performed
        /// </summary>
        /// <param name="patient">Patient participating action</param>
        /// <param name="resources">Resources involved in action</param>
        /// <param name="actionType">Type of action</param>
        /// <returns>Time span that represent the duration of the action</returns>
        public override TimeSpan PatientActionTime(EntityPatient patient, ResourceSet resources, ActionTypeClass actionType)
        {
            if (actionType.Type == "Diagnostics")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(25));

            if (actionType.Type == "Lab")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(25));

            return TimeSpan.FromMinutes(Distributions.Instance.Exponential(15));
        } // end of PatientSpecialActionTime

        #endregion PatientSpecialActionTime

        #region CreatePatientPath

        /// <summary>
        /// Method for patient path creation, uses core path method from base class
        /// and casts action list for special service
        /// </summary>
        /// <param name="admission">Admission type for special service</param>
        /// <param name="patient">Patient the path is created for</param>
        /// <param name="orignalRequest">Request that is the basis for the patient being admitted to the special service control</param>
        /// <returns>Patient path</returns>
        public SpecialServicePatientPath CreatePatientPath(SpecialServiceAdmissionTypes admission,
            EntityPatient patient,
            RequestSpecialFacilitiyService orignalRequest)
        {
            List<ActionTypeClass> actions;
            Admission outpatientAdmission;
            Admission inpatientAdmission;

            GetCorePath(patient, admission, out actions, out outpatientAdmission, out inpatientAdmission);

            return new SpecialServicePatientPath(actions.Select(p => new SpecialServiceActionTypeClass(p)).ToList(), orignalRequest, patient);
        } // end of CreatePatientPath

        #endregion CreatePatientPath

        #region GetAdmissionTypes

        /// <summary>
        /// Special service admission types that are handled by the control
        /// </summary>
        /// <returns></returns>
        public SpecialServiceAdmissionTypes[] GetAdmissionTypes()
        {
            return AdmissionTypes.Select(p => new SpecialServiceAdmissionTypes(p)).ToArray();
        } // end of GetSpecialFacilityAdmissionTypes

        #endregion GetAdmissionTypes
    } // end of InputDiagnosticsMedium
}
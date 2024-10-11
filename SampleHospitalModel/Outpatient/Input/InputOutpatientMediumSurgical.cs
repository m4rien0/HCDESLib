using GeneralHealthCareElements;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SampleHospitalModel.Outpatient
{
    /// <summary>
    /// Sample input for a outpatient control extending the general xml input for models with
    /// a booking model
    /// </summary>
    public class InputOutpatientMediumSurgical : GenericXMLHCDepInputWithAdmissionAndBookingModel, IInputOutpatient
    {
        #region Constructor

        /// <summary>
        /// Basic contructor
        /// </summary>
        /// <param name="xmlInput">Xml input that defines most of input</param>
        public InputOutpatientMediumSurgical(XMLInputHealthCareWithWaitingList xmlInput)
            : base(xmlInput)
        {
        } // end of InputEmergencyMedium

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Interface Methods
        //--------------------------------------------------------------------------------------------------

        #region GetNextWalkInPatient

        /// <summary>
        /// No walk in patients for this example
        /// </summary>
        /// <param name="arrivalTime">Next walk in arrival time</param>
        /// <param name="parentControlUnit">Control unit of outpatient department</param>
        /// <param name="currentTime">Current time</param>
        /// <returns>Null</returns>
        public EntityPatient GetNextWalkInPatient(out DateTime arrivalTime, ControlUnit parentControlUnit, DateTime currentTime)
        {
            arrivalTime = currentTime;

            return null;
        } // end of GetNextWalkInPatient

        #endregion GetNextWalkInPatient

        #region CreateOutpatientTreatmentPath

        /// <summary>
        /// Creates the path outpatients take, uses create core path method plus extra information on slot time
        /// </summary>
        /// <param name="patient">Patient</param>
        /// <param name="admission">Admission type of patient</param>
        /// <param name="slotTime">Slot time of patient arrival</param>
        /// <param name="walkIn">Walk-in flag of patient</param>
        /// <returns>Path for outpatient</returns>
        public OutpatientPath CreateOutpatientTreatmentPath(EntityPatient patient,
            Admission admissionType,
            DateTime slotTime,
            bool walkIn)
        {
            List<ActionTypeClass> actions;
            Admission outpatientAdmission;
            Admission inpatientAdmission;

            GetCorePath(patient, admissionType.AdmissionType, out actions, out outpatientAdmission, out inpatientAdmission);

            return new OutpatientPath(actions.Select(p => new OutpatientActionTypeClass(p)).ToList(), outpatientAdmission, inpatientAdmission, patient, slotTime, walkIn);
        } // end of CreateOutpatientTreatmentPath

        #endregion CreateOutpatientTreatmentPath

        #region GetNextWaitingListPatient

        /// <summary>
        /// Stream of patients arriving at the waiting list , for this example exponential interarrival time
        /// and patients arrive 24/7
        /// </summary>
        /// <param name="arrivalTime">Arrival time of next patient</param>
        /// <param name="admission">Admission type of next patient</param>
        /// <param name="parentControlUnit">Control unit of outpatient department</param>
        /// <param name="currentTime">Current time</param>
        /// <returns>Next patient with associated patient class</returns>
        public EntityPatient GetNextWaitingListPatient(out DateTime arrivalTime, out Admission admission, ControlUnit parentControlUnit, DateTime currentTime)
        {
            PatientClass newPatientClass = (PatientClass)PatientClassPerXmlPatientClass[PatientClassDistribution.GetRandomValue()].Clone();

            EntityPatient patient = new EntityPatient(EntityPatient.RunningPatientID++, newPatientClass);
            admission = new Admission(patient, new OutpatientAdmissionTypes("SurgicalOutpatient"), 0, 100, true);
            arrivalTime = currentTime + TimeSpan.FromMinutes(Distributions.Instance.Exponential(400));

            return patient;
        } // end of GetNextWaitingListPatient

        #endregion GetNextWaitingListPatient

        #region PatientActionTime

        /// <summary>
        /// Activity duriations, here modeled with exponential distributions independent from the patient conditions,
        /// just dependent on the action performed
        /// </summary>
        /// <param name="patient">Patient participating action</param>
        /// <param name="resources">Resources involved in action</param>
        /// <param name="actionType">Type of action</param>
        /// <returns>Time span that represent the duration of the action</returns>
        public override TimeSpan PatientActionTime(EntityPatient patient,
            ResourceSet resources,
            ActionTypeClass actionType)
        {
            if (actionType.Type == "Register")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(5));
            if (actionType.Type == "Assessment")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(10));
            if (actionType.Type == "Treatment")
                return TimeSpan.FromMinutes(Distributions.Instance.Exponential(15));

            return TimeSpan.FromMinutes(Distributions.Instance.Exponential(15));
        } // end of PatientEmergencyTreatmentTime

        #endregion PatientActionTime

        #region GetAdmissionTypes

        /// <summary>
        /// Specifies handled admission types
        /// </summary>
        /// <returns>Handled admission types</returns>
        public OutpatientAdmissionTypes[] GetAdmissionTypes()
        {
            return AdmissionTypes.Select(p => new OutpatientAdmissionTypes(p)).ToArray();
        } // end of GetSpecialFacilityAdmissionTypes

        #endregion GetAdmissionTypes
    } // end of InputOutpatientMedium
}
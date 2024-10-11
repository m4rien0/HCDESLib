using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Input;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Interface of Input Data required for emergency departments
    /// </summary>
    public interface IInputEmergency : IInputHealthCareDepartment
    {
        /// <summary>
        /// Inter-arrival times of arriving emergency patients
        /// </summary>
        /// <param name="time">Current time</param>
        /// <returns>Time span to next patient arrival</returns>
        TimeSpan PatientArrivalTime(DateTime time);

        /// <summary>
        /// Defines arriving patients, including patient class.
        /// </summary>
        /// <returns>New patient</returns>
        EntityPatient GetNextPatient();

        /// <summary>
        /// Creates emergency patient path, may use core path creating method of abstract
        /// patient path.
        /// </summary>
        /// <param name="patient">Patient for which path is created</param>
        /// <returns>New patient path</returns>
        EmergencyPatientPath CreateEmergencyPath(EntityPatient patient);
    } // end of IInputEmergencyMedium
}
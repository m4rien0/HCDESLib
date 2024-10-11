using GeneralHealthCareElements.TreatmentAdmissionTypes;
using System;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    /// <summary>
    /// Interface that defines required input for a booking model
    /// </summary>
    public interface IInputBookingModel
    {
        /// <summary>
        /// Defines the length of a slot for a specific admission
        /// </summary>
        /// <param name="admission">Admission</param>
        /// <returns>Length of slot required</returns>
        TimeSpan GetSlotLengthPerAdmission(Admission admission);

        /// <summary>
        /// Capacity that a specific admission requires
        /// </summary>
        /// <param name="admission">Admission</param>
        /// <returns>capacity, should be between 0 and 1</returns>
        double GetSlotCapacityPerAdmission(Admission admission);
    }
}
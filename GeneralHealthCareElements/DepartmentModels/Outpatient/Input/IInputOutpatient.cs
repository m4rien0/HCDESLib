using Enums;
using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Input;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.DepartmentModels.Outpatient
{
    public interface IInputOutpatient : IInputBookingModel, IInputHealthCareDepartment
    {

        /// <summary>
        /// Specifies handled admission types 
        /// </summary>
        /// <returns>Handled admission types</returns>
        OutpatientAdmissionTypes[] GetAdmissionTypes();

        /// <summary>
        /// Specifies the next time for dispatching requests for slot assignment 
        /// </summary>
        /// <param name="time">Current time</param>
        /// <returns>Time for next dispatching</returns>
        DateTime NextDispatching(DateTime time);

        /// <summary>
        /// Creates the path outpatients take
        /// </summary>
        /// <param name="patient">Patient</param>
        /// <param name="admission">Admission type of patient</param>
        /// <param name="slotTime">Slot time of patient arrival</param>
        /// <param name="walkIn">Walk-in flag of patient</param>
        /// <returns>Path for outpatient</returns>
        OutpatientPath CreateOutpatientTreatmentPath(EntityPatient patient, 
            Admission admission, 
            DateTime slotTime,
            bool walkIn);

        /// <summary>
        /// Stream of patients arriving at the waiting list 
        /// </summary>
        /// <param name="arrivalTime">Arrival time of next patient</param>
        /// <param name="admission">Admission type of next patient</param>
        /// <param name="parentControlUnit">Control unit of outpatient department</param>
        /// <param name="currentTime">Current time</param>
        /// <returns>Next patient with associated patient class</returns>
        EntityPatient GetNextWaitingListPatient(
            out DateTime arrivalTime,
            out Admission admission,
            ControlUnit parentControlUnit, 
            DateTime currentTime);

        /// <summary>
        /// Stream of arriving walk-in patients 
        /// </summary>
        /// <param name="arrivalTime">Next walk in arrival time</param>
        /// <param name="parentControlUnit">Control unit of outpatient department</param>
        /// <param name="currentTime">Current time</param>
        /// <returns>Next patient with associated patient class</returns>
        EntityPatient GetNextWalkInPatient(out DateTime arrivalTime, ControlUnit parentControlUnit, DateTime currentTime);

        /// <summary>
        /// Models no show of waiting list patients
        /// </summary>
        /// <param name="patient">Booked patient</param>
        /// <param name="admission">Admission type of patient</param>
        /// <param name="slot">Booked slot</param>
        /// <param name="currentTime">current time</param>
        /// <returns>Returns true if patient does not show for booked slot</returns>
        bool NoShowForAppointment(EntityPatient patient, Admission admission, Slot slot, DateTime currentTime);

        /// <summary>
        /// Time difference in the patient arrival to the time booked
        /// </summary>
        /// <param name="patient">Booked patient</param>
        /// <param name="admission">Admission type of patient</param>
        /// <returns>Time deviations (late or earliness) of booked time</returns>
        TimeSpan PatientArrivalDeviationFromSlotTime(EntityPatient patient, Admission admission);

        /// <summary>
        /// WaitingListSchedule used for the model 
        /// </summary>
        /// <returns></returns>
        EntityWaitingListSchedule GetWaitingListSchedule();
    } // end of IInputOutpatientMedium
}

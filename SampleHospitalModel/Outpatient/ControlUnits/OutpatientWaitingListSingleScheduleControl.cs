using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.DepartmentModels.Outpatient.WaitingList;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.Outpatient
{
    /// <summary>
    /// Sample control unit of a waiting list for a outpatient clinic
    /// </summary>
    public class OutpatientWaitingListSingleScheduleControl : OutpatientWaitingListControlUnit
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent outpatient control</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="input">Corresponding outpatient input data</param>
        /// <param name="assigningAtEvents">Flag if slots are assigned at events only, or immediately</param>
        public OutpatientWaitingListSingleScheduleControl(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            IInputOutpatient input,
                            bool assigningAtTimes)
            : base(name, parentControlUnit, parentSimulationModel, input, assigningAtTimes)
        {
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Initialization: Patient arrival stream for waiting list patients is initialized.
        /// If dispatching is done at discrete events this stream is initialized as well.
        /// </summary>
        /// <param name="startTime">Time the simulation starts</param>
        /// <param name="simEngine">SimEngine that handles the simulation run</param>
        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            WaitingListSchedule.Initialize(startTime);

            AddEntity((EntityWaitingListSchedule)WaitingListSchedule);

            DateTime nextArrivalTime;
            Admission admission;
            EntityPatient newPatient = InputData.GetNextWaitingListPatient(out nextArrivalTime, out admission, this, startTime);

            if (newPatient != null)
            {
                EventOutpatientWaitingListPatientArrival nextArrival = 
                    new EventOutpatientWaitingListPatientArrival(this, 
                        (ControlUnitOutpatient)ParentControlUnit, 
                        newPatient, 
                        admission, 
                        InputData);

                simEngine.AddScheduledEvent(nextArrival, nextArrivalTime);

            } // end if

            WaitingListSchedule.ReadyForDispatch = true;

            if (AssigningSlotsAtEvents)
            {
                EventOutpatientStartDispatching nextDispatch = new EventOutpatientStartDispatching(this, WaitingListSchedule, InputData);

                simEngine.AddScheduledEvent(nextDispatch, InputData.NextDispatching(startTime));
            
                WaitingListSchedule.ReadyForDispatch = false;
            }
        } // end of Initialize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Dispatches slot requests by booking in the booking model, further, now show probabilities
        /// and arrival deviations of patients are calculated. Corresponding events for arrival are scheduled.
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            
            if (RAEL.Count == 0)
                return false;

            if (!WaitingListSchedule.ReadyForDispatch)
                return false;

            while (RAEL.Count > 0)
            {
                RequestOutpatientWaitingListPatientToAssignSlot reqToDisptatch = (RequestOutpatientWaitingListPatientToAssignSlot)RAEL.First();

                DateTime earliestTime = time;

                earliestTime = reqToDisptatch.EarliestTime;

                Slot slot = WaitingListSchedule.GetEarliestSlotTime(time, 
                    earliestTime, 
                    reqToDisptatch.Patient, 
                    reqToDisptatch.AdmissionType);

                WaitingListSchedule.BookSlot(slot, reqToDisptatch.AdmissionType);

                reqToDisptatch.Patient.StopCurrentActivities(time, simEngine);
                ParentControlUnit.RemoveRequest(reqToDisptatch);
                RemoveRequest(reqToDisptatch);

                if (InputData.NoShowForAppointment(reqToDisptatch.Patient, reqToDisptatch.AdmissionType, slot, time))
                    continue;

                DateTime arrivalTime = slot.StartTime + InputData.PatientArrivalDeviationFromSlotTime(reqToDisptatch.Patient, reqToDisptatch.AdmissionType);

                arrivalTime = new DateTime(Math.Max(time.Ticks, arrivalTime.Ticks));

                EventOutpatientArrival arrival = new EventOutpatientArrival(ParentControlUnit, 
                    reqToDisptatch.Patient, 
                    slot.StartTime,
                    InputData,
                    reqToDisptatch.AdmissionType);

                simEngine.AddScheduledEvent(arrival, arrivalTime);

                Event patientWait = reqToDisptatch.Patient.StartWaitingActivity(null);

                patientWait.Trigger(time, simEngine);

            } // end while

            WaitingListSchedule.ReadyForDispatch = false;

            return false;

        } // end of PerformCustomRules

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Enter Leave
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        /// <summary>
        /// Not implemented for this example
        /// </summary>
        /// <param name="time">Time entity enters</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <param name="entity">Entity that enters</param>
        /// <param name="originDelegate">The delegate that corresponds with the entering, can be null</param>
        /// <returns></returns>
        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityEnterControlUnit

        #endregion

        #region EntityLeaveControlUnit

        /// <summary>
        /// Not implemented for this example
        /// </summary>
        /// <param name="time">Time entity leaves</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        /// <param name="entity">Entity that leaves</param>
        /// <param name="originDelegate">Delegate of sending entity</param>
        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityLeaveControlUnit

        #endregion

    } // end of
}

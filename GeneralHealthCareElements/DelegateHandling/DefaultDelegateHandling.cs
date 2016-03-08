using Enums;
using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Management;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements;
using SimulationCore.Helpers;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Delegates
{
    /// <summary>
    /// A list of pre-defined handling methods for delegates that might be used or
    /// be replaced by custom methods
    /// </summary>
    public static class DefaultDelegateHandling
    {
        #region HandleMoveOutpatient

        /// <summary>
        /// Handles the move or admission of a patient to an outpatient department in the model. The patient is
        /// directly referred to the outpatient control via the enter method. The handling of arriving
        /// patients is left to the target control.
        /// </summary>
        /// <param name="del">The RequestMoveOutpatient delegate</param>
        /// <param name="controlUnit">The filing control unit</param>
        /// <param name="time">Time the request was filed</param>
        /// <param name="simEngine">SimEngine responsible</param>
        /// <returns>True if the request could be handled</returns>
        static public bool HandleMoveOutpatient(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine)
        {
            #region MoveOutpatient

            if (del is RequestMoveOutpatient)
            {
                ControlUnit controlForAction;

                RequestMoveOutpatient outDel = (RequestMoveOutpatient)del;

                OutpatientAdmissionTypes admissionType = (OutpatientAdmissionTypes)outDel.OutpatientAdmission.AdmissionType;

                controlForAction = ((ControlUnitHealthCare)controlUnit).FindControlForOutpatientAdmission(admissionType);

                if (controlForAction == null)
                {
                    controlUnit.SendDelegateTo(controlUnit.ParentControlUnit, outDel);
                    return true;
                }
                else
                {
                    // patient is referred to the outpatient clinic
                    EntityPatient patient = ((outDel).Patient);
                    patient.StopCurrentActivities(time, simEngine);

                    Event enterEvent = controlForAction.EntityEnterControlUnit(time, simEngine, patient, outDel);

                    enterEvent.Trigger(time, simEngine);

                    return true;
                } // end if
            } // end if

            #endregion

            return true;
        } // end of HandleMoveOutpatient

        #endregion

        #region HandleMoveInpatient

        /// <summary>
        /// Handles the move or admission of a patient to an inpatient department in the model. The patient is
        /// directly referred to the inpatient control via the enter method. The handling of arriving
        /// patients is left to the target control.
        /// </summary>
        /// <param name="del">The RequestMoveInpatient delegate</param>
        /// <param name="controlUnit">The filing control unit</param>
        /// <param name="time">Time the request was filed</param>
        /// <param name="simEngine">SimEngine responsible</param>
        /// <returns>True if the request could be handled</returns>
        static public bool HandleMoveInpatient(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine)
        {
            #region MoveInpatient

            if (del is RequestMoveInpatient)
            {
                ControlUnit controlForAction;

                RequestMoveInpatient outDel = (RequestMoveInpatient)del;

                InpatientAdmissionTypes admissionType = (InpatientAdmissionTypes)outDel.InpatientAdmission.AdmissionType;

                controlForAction = ((ControlUnitHealthCare)controlUnit).FindControlForInpatientAdmission(admissionType);

                if (controlForAction == null)
                {
                    controlUnit.SendDelegateTo(controlUnit.ParentControlUnit, outDel);
                    return true;
                }
                else
                {
                    EntityPatient patient = ((outDel).Patient);
                    patient.StopCurrentActivities(time, simEngine);

                    Event enterEvent = controlForAction.EntityEnterControlUnit(time, simEngine, patient, outDel);

                    enterEvent.Trigger(time, simEngine);

                    return true;
                } // end if
            } // end if

            #endregion

            return true;
        } // end of HandleMoveInpatient

        #endregion

        #region ForwardServiceRequestSpecialTreatmentModel

        /// <summary>
        /// Looks for a control unit to handle a special service requests and forwards the request to the
        /// special service model, as a booking maybe required
        /// </summary>
        /// <param name="del">The original requests</param>
        /// <param name="controlUnit">The control unit that filed the request</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>True if request has been handled</returns>
        static public bool ForwardServiceRequestSpecialTreatmentModel(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine)
        {
            ControlUnit controlForAction = ((ControlUnitHealthCare)controlUnit).FindControlUnitForSpecialFacitlityService((RequestSpecialFacilitiyService)del);

            if (controlForAction == null)
            {
                controlUnit.SendDelegateTo(controlUnit.ParentControlUnit, del);
                return true;
            }
            else
            {
                controlUnit.SendDelegateTo(controlForAction, del);
                return true;
            }
        } // end of HandleServiceRequestSpecialTreatmentModel

        #endregion

        #region BookImmediateServiceRequestSpecialTreatmentModel

        /// <summary>
        /// Standard method, to be used when special service does nor require special booking
        /// and can be "booked" immidiately. An availability delegate is sent back to 
        /// the requesting control unit
        /// </summary>
        /// <param name="del">The original requests</param>
        /// <param name="controlUnit">The control unit that handles the service request</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>True if request has been handled</returns>
        public static bool BookImmediateServiceRequestSpecialTreatmentModel(IDelegate del, 
            ControlUnit controlUnit, 
            DateTime time, 
            ISimulationEngine simEngine)
        {
            RequestSpecialFacilitiyService serviceRequest = (RequestSpecialFacilitiyService)del;

            ControlUnitSpecialServiceModel specialFacilityControl = (ControlUnitSpecialServiceModel)controlUnit;

            Slot slot = specialFacilityControl.WaitingListSchedule.GetEarliestSlotTime(time,
                time,
                serviceRequest.Patient,
                new Admission(serviceRequest.Patient, serviceRequest.SpecialFacilityAdmissionTypes));

            DelegateAvailabilitiesForRequest availDel = new DelegateAvailabilitiesForRequest(specialFacilityControl, serviceRequest, SpecialServiceBookingTypes.Immediate, Helpers<Slot>.ToList(slot));

            controlUnit.SendDelegateTo(del.OriginControlUnit, availDel);

            return true;
        } // end of 

        #endregion

        #region HandleImmediateSpecialServiceRequest

        /// <summary>
        /// Standard method to handle a "immediate" booking of a special service control.
        /// Sends the patient on a move from the requesting control to the special service control.
        /// </summary>
        /// <param name="del">The availability delegate</param>
        /// <param name="controlUnit">The control unit that filed the original request</param>
        /// <param name="time">Time the request is filed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>True if request has been handled</returns>
        public static bool HandleImmediateSpecialServiceRequest(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine)
        {
            #region ServiceRequests

            DelegateAvailabilitiesForRequest availability = (DelegateAvailabilitiesForRequest)del;

            if (availability.BookingType != SpecialServiceBookingTypes.Immediate)
                throw new InvalidOperationException("Only immediate bookings handled");

            if (availability.OriginalServiceRequest is RequestSpecialFacilitiyService)
            {
                RequestSpecialFacilitiyService req = (RequestSpecialFacilitiyService)availability.OriginalServiceRequest;

                ControlUnitManagement jointControl = (ControlUnitManagement)controlUnit.ParentControlUnit.FindSmallestJointControl(availability.ServiceControl);

                ActivityMove movePatientToSpecialTreatment
                    = new ActivityMove(jointControl,
                                       req.Patient,
                                       controlUnit,
                                       availability.ServiceControl,
                                       req,
                                       jointControl.InputData.DurationMove(req.Patient,
                                        controlUnit,
                                        availability.ServiceControl));

                req.Patient.StopWaitingActivity(time, simEngine);
                movePatientToSpecialTreatment.StartEvent.Trigger(time, simEngine);

                return true;

            } // end if

            #endregion

            return false;
        } // end of HandleAvailabilitiesSpecialServiceRequest

        #endregion

        #region HandleRequireDocs

        /// <summary>
        /// Handles a request to send doctors for consultation or assisting between departments
        /// </summary>
        /// <param name="del">The original request for doctors to consult or assist</param>
        /// <param name="controlUnit">Control unit that filed request for assistance or consultation</param>
        /// <param name="time">Time request was filed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>True if request has been handled</returns>
        static public bool HandleRequireDocs(IDelegate del, ControlUnit controlUnit, DateTime time, ISimulationEngine simEngine)
        {
            foreach (SkillSet reqSkill in ((DelegateRequestDocsForAssisting)del).RequiredSkillSets)
            {
                List<EntityDoctor> possibleDoc = ((ControlUnitHealthCare)controlUnit).FindDoctorWithSkillSet(reqSkill);

                if (possibleDoc.Count == 0)
                    break;

                EntityDoctor chosenDoc = null;

                foreach (EntityDoctor doc in possibleDoc)
                {
                    if (doc.ParentControlUnit == controlUnit)
                    {
                        ActivityMove possibleMove = doc.GetPossibleMovingActivity();

                        if (possibleMove != null
                            && (possibleMove.Destination == del.OriginControlUnit
                            || possibleMove.Destination == doc.BaseControlUnit))
                        {
                            chosenDoc = doc;
                            break;
                        } // end if


                    } // end if

                    if (((ControlUnitHealthCare)doc.ParentControlUnit).ControlUnitType == Enums.ControlUnitType.Inpatient)
                    {
                        chosenDoc = doc;
                        break;
                    } // end if 
                } // end foreach

                if (chosenDoc == null)
                    chosenDoc = possibleDoc.First();

                if (chosenDoc.ParentControlUnit == controlUnit)
                {
                    ActivityMove possibleMove = chosenDoc.GetPossibleMovingActivity();

                    if (possibleMove != null && possibleMove.Destination == chosenDoc.BaseControlUnit)
                    {
                        simEngine.RemoveScheduledEvent(possibleMove.EndEvent);
                        chosenDoc.StopCurrentActivities(time, simEngine);
                        ActivityMove move = new ActivityMove(controlUnit, chosenDoc, controlUnit, del.OriginControlUnit, del, TimeSpan.FromMinutes(1));
                        move.StartEvent.Trigger(time, simEngine);
                    } // end if
                }
                else
                {
                    controlUnit.SendDelegateTo(chosenDoc.ParentControlUnit, new DelegateSentDocForAssistedTreatment((ControlUnitHealthCare)del.OriginControlUnit, reqSkill));
                } // end if

            } // end foreach

            return true;
        } // end of HandleRequireDocs

        #endregion

    } // end of Default DelegateHandling
}

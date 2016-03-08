using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeneralHealthCareElements.ControlUnits;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using GeneralHealthCareElements.SpecialFacility;
using GeneralHealthCareElements.Delegates;
using GeneralHealthCareElements.TreatmentAdmissionTypes;
using SimulationCore.Helpers;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.Events;
using Enums;

namespace GeneralHealthCareElements.DepartmentModels.Emergency
{
    /// <summary>
    /// Base class for emergency department control units
    /// </summary>
    abstract public class ControlUnitEmergency : ControlUnitHealthCareDepartment
    {
        #region Constructor

        /// <summary>
        /// Basic constructor of emergency department controls
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent management control unit</param>
        /// <param name="parentSimulationModel">Parent simulation model</param>
        /// <param name="inputData">Emergency input data</param>
        public ControlUnitEmergency(string name, 
                                    ControlUnit parentControlUnit, 
                                    SimulationModel parentSimulationModel, 
                                    IInputEmergency inputData)
            : base(ControlUnitType.Emergency, 
                   name, 
                   parentControlUnit,
                   parentSimulationModel,
                   inputData)
        {
            _inputData = inputData;

            _delegateHandlingMethods.Add(typeof(DelegateAvailabilitiesForRequest), DefaultDelegateHandling.HandleImmediateSpecialServiceRequest);

        } // end of ControlUnitEmergency
        #endregion

        //--------------------------------------------------------------------------------------------------
        // Enter Leave
        //--------------------------------------------------------------------------------------------------

        #region EntityEnterControlUnit

        /// <summary>
        /// Entity arriving hanlding. Patients are handled with an arrival event, staff with a different arrive/leave event
        /// </summary>
        /// <param name="time">Time entity arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <param name="entity">Arriving entity</param>
        /// <param name="originDelegate">A possible delegate that is associated with the arrival</param>
        /// <returns></returns>
        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            if (entity is EntityPatient)
                return new EventEmergencyPatientArrival(this, (EntityPatient)entity, InputData);

            if (entity is EntityStaff)
                return new EventControlUnitStaffEnterLeave(this, true, (EntityStaff)entity, originDelegate, WaitingRoomForStaff((EntityHealthCareStaff)entity));

            return null;

        } // end of EntityEnterControlUnit

        #endregion

        #region EntityLeaveControlUnit

        /// <summary>
        /// Leaving entities handling, no functionality provided per default
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        /// <param name="entity"></param>
        /// <param name="originDelegate"></param>
        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {

        } // end of EntityLeaveControlUnit

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Handled Treatments 
        //--------------------------------------------------------------------------------------------------

        #region HandledOutpatientAdmissionTypes

        /// <summary>
        /// Overriden handled outpatient admissions, returns empty array as this
        /// is an emergency control
        /// </summary>
        public override OutpatientAdmissionTypes[] HandledOutpatientAdmissionTypes
        {
            get
            {
                return Helpers<OutpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledOutpatientAdmissionTypes

        #endregion

        #region HandledInpatientAdmissionTypes

        /// <summary>
        /// Overriden handled inpatient admissions, returns empty array as this
        /// is an emergency control
        /// </summary>
        public override InpatientAdmissionTypes[] HandledInpatientAdmissionTypes
        {
            get
            {
                return Helpers<InpatientAdmissionTypes>.EmptyArray();
            }
        } // end of HandledInpatientAdmissionTypes

        #endregion

        #region HandledSpecialFacilityAdmissionTypes

        /// <summary>
        /// Overriden handled special service admissions, returns empty array as this
        /// is an emergency control
        /// </summary>
        public override SpecialServiceAdmissionTypes[] HandledSpecialFacilityAdmissionTypes
        {
            get
            {
                return Helpers<SpecialServiceAdmissionTypes>.EmptyArray();
            }
        } // end of HandledDiagnosticsTreatments

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputEmergency _inputData;

        /// <summary>
        /// Emergency Input Data
        /// </summary>
        public new IInputEmergency InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion
       
    } // end of ControlUnitEmergency
}

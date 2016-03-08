using SampleHospitalModel.Emergency;
using SampleHospitalModel.Hospital;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel
{
    public class HospitalSimulationModel : SimulationModel
    {
        #region Constructor

        public HospitalSimulationModel(DateTime startTime, DateTime endTime)
            :base(startTime, endTime)
        {
            //--------------------------------------------------------------------------------------------------
            // Create Tree
            //--------------------------------------------------------------------------------------------------

            // hospital
            InputHospital inputHosptial = new InputHospital();
            ControlUnitHospital hospital = new ControlUnitHospital("Hospital", null, this, inputHosptial);

            // emergency
            InputEmergency inputEmergency = new InputEmergency(null);
            ControlUnitEmergencyExample emergency = new ControlUnitEmergencyExample("Emergency", hospital, this, inputEmergency);

            // diagnostics
            //InputDiagnosticsMedium inputDiagnostics = new InputDiagnosticsMedium();
            //ControlUnitSpecialTreatmentModelDiagnostics diagnostics = new ControlUnitSpecialTreatmentModelDiagnostics("Diagnostics",
            //    hospital,
            //    this,
            //    inputDiagnostics.GetSpecialFacilityAdmissionTypes().ToArray(),
            //    inputDiagnostics,
            //    inputDiagnostics);

            ////surgery theatre
            //InputSurgeryTheatre inputSurgeryTheatre = new InputSurgeryTheatre();
            //ControlUnitSpecialTreatmentSurgeryTheatre surgery
            //    = new ControlUnitSpecialTreatmentSurgeryTheatre("SurgeryTheatres",
            //    hospital,
            //    this,
            //    Helpers<TreatmentType>.EmptyArray(),
            //    inputSurgeryTheatre,
            //    inputSurgeryTheatre);

            // outpatient

            #region OutpatientSurgical

            //InputOutpatientMediumSurgical inputOutpatientSurgical = new InputOutpatientMediumSurgical();
            //OutpatientWaitingListControlUnit waitingListOutpatientSurgical = new OutpatientWaitingListControlUnit("OutpatientSurgicalWaitingList",
            //    hospital,
            //    this,
            //    inputOutpatientSurgical,
            //    true,
            //    true);

            //ControlUnitOutpatientMedium outpatientSurgical =
            //    new ControlUnitOutpatientMedium("OutpatientSurgical",
            //                                    hospital,
            //                                    this,
            //                                    inputOutpatientSurgical,
            //                                    waitingListOutpatientSurgical);
            //waitingListOutpatientSurgical.SetParentControlUnit(outpatientSurgical);

            #endregion

            #region OutpatientIntern

            //InputOutpatientMediumIntern inputOutpatientIntern = new InputOutpatientMediumIntern();
            //OutpatientWaitingListControlUnit waitingListOutpatientIntern = new OutpatientWaitingListControlUnit("OutpatientInternWaitingList",
            //    hospital,
            //    this,
            //    inputOutpatientIntern,
            //    true,
            //    true);


            //ControlUnitOutpatientMedium outpatientIntern =
            //    new ControlUnitOutpatientMedium("OutpatientIntern",
            //        hospital,
            //        this,
            //        inputOutpatientIntern,
            //        waitingListOutpatientIntern);
            //waitingListOutpatientIntern.SetParentControlUnit(outpatientIntern);


            #endregion

            //outpatientSurgical.SetChildControlUnits(new ControlUnit[] { waitingListOutpatientSurgical });
            //outpatientIntern.SetChildControlUnits(new ControlUnit[] { waitingListOutpatientIntern });
            //hospital.SetChildControlUnits(new ControlUnit[] { diagnostics, emergency, inpatientSurgical, outpatientSurgical, outpatientIntern, inpatientIntern, surgery });
            //hospital.SetChildControlUnits(new ControlUnit[] {diagnostics, inpatientIntern, inpatientSurgical, emergency, outpatientIntern});
            //hospital.SetChildControlUnits(new ControlUnit[] { waitingListInpatient, inpatient, emergency, diagnostics, waitingListOutpatient, outpatient});
            //hospital.SetChildControlUnits(new ControlUnit[] { waitingListInpatient, inpatient});

            _rootControlUnit = hospital;
        } // end of HosptialSimulationModel

        #endregion

        #region InitializeModel

        public override void CustomInitializeModel()
        {

        } // end of InitializeModel

        #endregion

        #region GetModelString

        public override string GetModelString()
        {
            throw new NotImplementedException();
        }

        #endregion
        
    } // end of HospitalSimulationModel
}

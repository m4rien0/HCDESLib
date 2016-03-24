using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.Input;
using SampleHospitalModel.Diagnostics;
using SampleHospitalModel.Emergency;
using SampleHospitalModel.Hospital;
using SampleHospitalModel.Outpatient;
using SampleHospitalModel.Visualization;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.GeometricClasses;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;
using WPFVisualizationBase;

namespace SampleHospitalModel
{
    /// <summary>
    /// Sample simulation model for a hospital
    /// </summary>
    public class HospitalSimulationModelWithVisualization : SimulationModel
    {
        ControlUnit hospital;
        ControlUnitEmergencyExample emergency;
        ControlUnitSpecialTreatmentModelDiagnostics diagnostics;
        OutpatientWaitingListSingleScheduleControl waitingListOutpatientSurgical;
        ControlUnitOutpatientMedium outpatientSurgical;
        ControlUnitEmergencyRegisterTriage triageRegisterOrgUnit;
        ContorlUnitAssessmentTreatmentExample surgicalOrgUnit;
        ContorlUnitAssessmentTreatmentExample internalOrgUnit;


        #region Constructor

        /// <summary>
        /// All submodels for emergency, outpatient and diagnostic departments are set
        /// </summary>
        /// <param name="startTime">Start time of simulation</param>
        /// <param name="endTime">End time of simulation</param>
        public HospitalSimulationModelWithVisualization(DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            XmlSerializer deserializer = new XmlSerializer(typeof(XMLInputHealthCareWithWaitingList));

            //--------------------------------------------------------------------------------------------------
            // Create Tree
            //--------------------------------------------------------------------------------------------------

            #region Hospital

            // hospital
            InputHospital inputHosptial = new InputHospital();
            hospital = new ControlUnitHospital("Hospital", null, this, inputHosptial);

            #endregion

            #region Emergency

            // emergency
            string emergencyInputFile = "C:\\Users\\Nik\\Documents\\Auckland\\HCCM\\NiksWorkingDir\\Source\\SampleHospitalModel\\Emergency\\Input\\XMLEmergencySampleInput.xml";
            TextReader textReader = new StreamReader(emergencyInputFile);

            XmlSerializer emergencyDeserializer = new XmlSerializer(typeof(XMLInputHealthCareDepartment));
            XMLInputHealthCareDepartment emergencyXMLInput = (XMLInputHealthCareDepartment)emergencyDeserializer.Deserialize(textReader);

            InputEmergency inputEmergency = new InputEmergency(emergencyXMLInput);
            emergency = new ControlUnitEmergencyExample("Emergency", hospital, this, inputEmergency);

            triageRegisterOrgUnit = new ControlUnitEmergencyRegisterTriage("OrgUnitTriageRegister", emergency, emergency, this, inputEmergency);
            surgicalOrgUnit = new ContorlUnitAssessmentTreatmentExample("OrgUnitSurgical", emergency, emergency, this, inputEmergency);
            internalOrgUnit = new ContorlUnitAssessmentTreatmentExample("OrgUnitInternal", emergency, emergency, this, inputEmergency);

            emergency.SetChildOrganizationalControls(new ControlUnitOrganizationalUnit[] { triageRegisterOrgUnit, surgicalOrgUnit, internalOrgUnit });


            #endregion

            #region Diagnostics

            // diagnostics
            string diagnosticsInputFile = "C:\\Users\\Nik\\Documents\\Auckland\\HCCM\\NiksWorkingDir\\Source\\SampleHospitalModel\\Diagnostics\\Input\\XMLSampleInputDiagnostics.xml";
            textReader = new StreamReader(diagnosticsInputFile);

            XMLInputHealthCareWithWaitingList diagnosticsXMLInput = (XMLInputHealthCareWithWaitingList)deserializer.Deserialize(textReader);

            InputDiagnostics inputDiagnostics = new InputDiagnostics(diagnosticsXMLInput);
            diagnostics =
                new ControlUnitSpecialTreatmentModelDiagnostics("Diagnostics",
                hospital,
                this,
                inputDiagnostics.GetAdmissionTypes().ToArray(),
                inputDiagnostics.GetWaitingListSchedule(),
                inputDiagnostics);

            #endregion

            #region OutpatientSurgical

            string outpatientInputFile = "C:\\Users\\Nik\\Documents\\Auckland\\HCCM\\NiksWorkingDir\\Source\\SampleHospitalModel\\Outpatient\\Input\\XMLSampleInputOutpatientSurgical.xml";
            textReader = new StreamReader(outpatientInputFile);

            XMLInputHealthCareWithWaitingList outPatientXMLInput = (XMLInputHealthCareWithWaitingList)deserializer.Deserialize(textReader);
            InputOutpatientMediumSurgical inputOutpatientSurgical = new InputOutpatientMediumSurgical(outPatientXMLInput);

            waitingListOutpatientSurgical =
                new OutpatientWaitingListSingleScheduleControl(
                "OutpatientSurgicalWaitingList",
                hospital,
                this,
                inputOutpatientSurgical,
                true);


            outpatientSurgical =
                new ControlUnitOutpatientMedium("OutpatientSurgical",
                                                hospital,
                                                this,
                                                inputOutpatientSurgical,
                                                waitingListOutpatientSurgical);
            waitingListOutpatientSurgical.SetParentControlUnit(outpatientSurgical);
            outpatientSurgical.SetChildControlUnits(new ControlUnit[] { waitingListOutpatientSurgical });

            #endregion

            hospital.SetChildControlUnits(new ControlUnit[] { emergency, outpatientSurgical, diagnostics });

            _rootControlUnit = hospital;
        } // end of

        #endregion

        #region InitializeModel

        public override void CustomInitializeModel()
        {

            
        } // end of InitializeModel

        #endregion

        #region InitializeVisualization

        /// <summary>
        /// Visualization engines per control units are set for a drawing system passed as args
        /// </summary>
        /// <param name="args">Here a drawing system</param>
        public override void InitializeVisualization(object args = null)
        {
            BaseWPFModelVisualization visioEngine = new BaseWPFModelVisualization(this, (DrawingOnCoordinateSystem)args);

            WPFVisualizationEngineHealthCareDepartmentControlUnit<EmergencyActionTypeClass> emergencyVisio = new WPFVisualizationEngineHealthCareDepartmentControlUnit<EmergencyActionTypeClass>((DrawingOnCoordinateSystem)args, new Point(0, 0), new Size(), 100);

            visioEngine.VisualizationPerControlUnit.Add(emergency, emergencyVisio);
            visioEngine.VisualizationPerControlUnit.Add(triageRegisterOrgUnit, new WPFVisualizationHealthCareOrganizationalUnit<EmergencyActionTypeClass>((DrawingOnCoordinateSystem)args, 100, emergencyVisio));
            visioEngine.VisualizationPerControlUnit.Add(surgicalOrgUnit, new WPFVisualizationHealthCareOrganizationalUnit<EmergencyActionTypeClass>((DrawingOnCoordinateSystem)args, 100, emergencyVisio));
            visioEngine.VisualizationPerControlUnit.Add(internalOrgUnit, new WPFVisualizationHealthCareOrganizationalUnit<EmergencyActionTypeClass>((DrawingOnCoordinateSystem)args, 100, emergencyVisio));
            visioEngine.VisualizationPerControlUnit.Add(diagnostics, new WPFVisualizationEngineHealthCareDepartmentControlUnit<SpecialServiceActionTypeClass>((DrawingOnCoordinateSystem)args,new Point(), new Size(), 100));
            visioEngine.VisualizationPerControlUnit.Add(outpatientSurgical, new WPFVisualizationEngineOutpatientDepartment((DrawingOnCoordinateSystem)args, new Point(0, 1800), new Size(), 100));

            _simulationDrawingEngine = visioEngine;
        
        } // end of InitializeVisualization

        #endregion

        #region GetModelString

        public override string GetModelString()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

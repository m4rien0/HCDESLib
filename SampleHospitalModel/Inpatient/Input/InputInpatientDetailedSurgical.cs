using Enums;
using SimulationCore.GeneralHealthElements.DepartmentModels.Outpatient.GeneralClasses;
using SimulationCore.MathTool;
using SimulationCore.HCCMElements.ControlUnit;
using SimulationCore.HCCMElements.CoreElements.Control_Units;
using SimulationCore.HCCMElements.Entities.DefaultEntities;
using SimulationCore.HCCMElements.Entities.InpatientEntities;
using SimulationCore.HCCMElements.EventsHandling.Events.InpatientEvents;
using SimulationCore.HCCMElements.Inpatient.GeneralClasses;
using SimulationCore.HCCMElements.InputOutput.Interfaces;
using SimulationCore.HCCMElements.SkillHandling;
using SimulationCore.HCCMElements.TreatmentAdmissionTypes;
using SimulationCore.HCCMElements.TreatmentTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthElements.InputOutput.Inpatient
{
    public class InputInpatientDetailedSurgical : IInputInpatient
    {
        #region Constructor
        
        public InputInpatientDetailedSurgical()
        {
            GetInpatientTreatmentTypes();
        } // end of InputInpatientDetailedSurgical

        #endregion

        //--------------------------------------------------------------------------------------------------
        // InterfaceMethods
        //--------------------------------------------------------------------------------------------------

        #region InpatientTreatmentTypes

        private Dictionary<string, InpatientTreatmentTypes> TreatmentTypes;

        public InpatientTreatmentTypes[] GetInpatientTreatmentTypes()
        {
            TreatmentTypes = new Dictionary<string, InpatientTreatmentTypes>();
            List<InpatientTreatmentTypes> allTreatmentTypes = new List<InpatientTreatmentTypes>();

            allTreatmentTypes.Add(new InpatientTreatmentTypes("InpatientSurgicalTreatment", false));
            allTreatmentTypes.Add(new InpatientTreatmentTypes("InpatientSurgicalPreTreatment", false));
            allTreatmentTypes.Add(new InpatientTreatmentTypes("InpatientSurgicalFollowUp", false));
            allTreatmentTypes.Add(new InpatientTreatmentTypes("InpatientEmergencyTreatment", true));

            foreach (InpatientTreatmentTypes type in allTreatmentTypes)
            {
                TreatmentTypes.Add(type.Identifier, type);
            } // end foreach

            return null;
        } // end of InpatientTreatmentTypes

        #endregion

        #region InpatientArrivalTime

        public TimeSpan InpatientArrivalTime()
        {
            return TimeSpan.FromMinutes(MathTool.Distributions.Instance.Exponential(200));
        } // end of PatientArrivalTime


        #endregion

        #region DurationRound

        public TimeSpan DurationRound(List<EntityWard> wards)
        {
            int totalPatients = 0;

            foreach (EntityWard ward in wards)
            {
                totalPatients += ward.AllBeds.Where(p => p.Occupied).Count();
            } // end foreach

            return TimeSpan.FromMinutes(2 + totalPatients * 4);

        } // end of DurationRound

        #endregion

        #region InpatientEmergencyWhileStayInBed

        public InpatientTreatmentTypes InpatientEmergencyWhileStayInBed(EntityPatient patient, TimeSpan timePlannedInBed)
        {
            if (patient.PatientClass.Priority <= 2)
                return null;
            else if (MathTool.Distributions.Instance.RandomNumberGenerator.NextDouble() <= (0.2 * patient.PatientClass.Priority - 0.1 * patient.NumberInpatientEmergencyTreatments) * timePlannedInBed.TotalDays / 2)
                return TreatmentTypes["InpatientEmergencyTreatment"];

            return null;
        } // end of InpatientEmergencyWhileStayInBed

        #endregion

        #region InpatientDurationToEmergency

        public TimeSpan InpatientDurationToEmergency(EntityPatient patient, TimeSpan timePlannedInBed)
        {
            return TimeSpan.FromMinutes(MathTool.Distributions.Instance.RandomNumberGenerator.NextDouble() * timePlannedInBed.TotalMinutes);
        } // end of InpatientEmergencyWhileStayInBed

        #endregion

        #region NeedInpatientDiagnosticTest

        private Dictionary<string, SpecialFacilityAdmissionTypes> DiagnosticTests;

        public bool NeedInpatientDiagnosticTest(EntityPatient patient, out SpecialFacilityAdmissionTypes treatment)
        {
            treatment = null;
            return false;

            //if (DiagnosticTests == null)
            //{
            //    DiagnosticTests = new Dictionary<string, SpecialFacilityTreatmentTypes>();
            //    DiagnosticTests.Add("DiagnosticsCTTreatment", new SpecialFacilityTreatmentTypes("DiagnosticsCTTreatment"));
            //    DiagnosticTests.Add("DiagnosticsXRayTreatment", new SpecialFacilityTreatmentTypes("DiagnosticsXRayTreatment"));
            //    DiagnosticTests.Add("DiagnosticsMRITreatment", new SpecialFacilityTreatmentTypes("DiagnosticsMRITreatment"));
            //} // end if

            //double test = MathTool.Distributions.Instance.RandomNumberGenerator.NextDouble() / ((double)patient.NumberDiagnosticTests + 1);
            //if (test >= 0.5)
            //{
            //    if (test <= 0.95)
            //    {
            //        treatment = DiagnosticTests["DiagnosticsXRayTreatment"];
            //        return true;
            //    }
            //    else if (test <= 0.99)
            //    {
            //        treatment = DiagnosticTests["DiagnosticsCTTreatment"];
            //        return true;
            //    }
            //    else
            //    {
            //        treatment = DiagnosticTests["DiagnosticsMRITreatment"];
            //        return true;
            //    } // end if
            //}
            //else
            //    return false;
        } // end of NeedDiagnosticTest

        #endregion

        #region GetPatientPriority

        public int GetPatientPriority()
        {
            return MathTool.Distributions.Instance.RandomInteger(0, 5);
        } // end of PatientArrivalTime

        #endregion

        #region NextDispatching

        public TimeSpan NextDispatching()
        {
            return TimeSpan.FromDays(1);
        } // end of LatestFollowUp

        #endregion

        #region InpatienStartTime

        public TimeSpan InPatientStartTime()
        {
            return TimeSpan.FromHours(8);
        } // end of InpatientStartTime

        #endregion

        #region InpatienDayDuration

        public TimeSpan InpatienDayDuration()
        {
            return TimeSpan.FromHours(7);
        } // end of InpatientStartTime

        #endregion

        #region DurationInpatientTreatment

        public TimeSpan DurationInpatientTreatment(EntityPatient patient)
        {
            return TimeSpan.FromHours(1);
        } // end of DurationInpatientTreatment

        #endregion

        #region DurationInpatientEmergencyTreatment

        public TimeSpan DurationInpatientEmergencyTreatment(EntityPatient patient)
        {
            return TimeSpan.FromHours(0.50 + 0.5 * MathTool.Distributions.Instance.RandomNumberGenerator.NextDouble());
        } // end of DurationInpatientTreatment

        #endregion

        #region DurationDoctorOrgWork

        public TimeSpan DurationDoctorOrgWork(EntityDoctor patient)
        {
            return TimeSpan.FromHours(0.5);
        } // end of DurationDoctorOrgWork

        #endregion

        #region InpatientNumberSlotsPerDay

        public int InpatientNumberSlotsPerDay(WardTypes wardType, BedType bedType)
        {
            return 3;
        } // end of InpatientNumberSlotsPerDay

        #endregion

        #region CreateInpatientPath

        public EntityInpatientPath CreateInpatientPath(EntityPatient patient, InpatientAdmissionTypes admission)
        {
            List<TimeSpan> timeGaps1 = new List<TimeSpan>();
            timeGaps1.Add(TimeSpan.FromDays(1));
            timeGaps1.Add(TimeSpan.FromDays(1));


            TreatmentBlock newTreatmentBlocl0 = new TreatmentBlock(
                MathTool.Helpers<InpatientTreatmentTypes>.ToList(TreatmentTypes["InpatientSurgicalTreatment"]),
                timeGaps1,
                WardTypes.IntensiveCare,
                BedType.Intensive,
                true,
                this);

            List<InpatientTreatmentTypes> treatmentsBlock2 = new List<InpatientTreatmentTypes>();

            treatmentsBlock2.Add(TreatmentTypes["InpatientSurgicalPreTreatment"]);
            treatmentsBlock2.Add(TreatmentTypes["InpatientSurgicalTreatment"]);
            treatmentsBlock2.Add(TreatmentTypes["InpatientSurgicalFollowUp"]);

            List<TimeSpan> timeGaps2 = new List<TimeSpan>();
            timeGaps2.Add(TimeSpan.FromDays(1));
            timeGaps2.Add(TimeSpan.FromDays(1));
            timeGaps2.Add(TimeSpan.FromDays(1));
            timeGaps2.Add(TimeSpan.FromDays(1));

            TreatmentBlock newTreatmentBlock1 = new TreatmentBlock(
                treatmentsBlock2,
                timeGaps2,
                WardTypes.General,
                BedType.Scheduled,
                false,
                this);

            List<TreatmentBlock> treatmentBlocks = new List<TreatmentBlock>();
            //if (patient.PatientType == PatientType.Emergency)
            //    treatmentBlocks.Add(newTreatmentBlocl0);
            treatmentBlocks.Add(newTreatmentBlock1);

            return new EntityInpatientPath(patient, treatmentBlocks);
        } // end of EntityInpatientPath

        #endregion

        #region GetDoctors

        public EntityDoctor[] GetDoctors(ControlUnitHealthCare controlUnit)
        {
            List<EntityDoctor> doctors = new List<EntityDoctor>();

            SingleSkill skillDoc1 = new SingleSkill("SurgicalSkill", 4);
            doctors.Add(new EntityDoctor(new SkillSet(skillDoc1.ToArray())));

            SingleSkill skillDoc2 = new SingleSkill("SurgicalSkill", 6);
            doctors.Add(new EntityDoctor(new SkillSet(skillDoc2.ToArray())));

            SingleSkill skillDoc3 = new SingleSkill("SurgicalSkill", 2);
            doctors.Add(new EntityDoctor(new SkillSet(skillDoc3.ToArray())));

            SingleSkill skillDoc4 = new SingleSkill("SurgicalSkill", 2);
            doctors.Add(new EntityDoctor(new SkillSet(skillDoc4.ToArray())));

            return doctors.ToArray();
        } // end of GetDoctors

        #endregion

        #region GetWards

        public Dictionary<WardTypes, List<EntityWard>> GetWards()
        {
            EntityWard ward1;
            EntityWard ward2;
            List<EntityBed> ward1Beds = new List<EntityBed>();
            List<EntityBed> ward2Beds = new List<EntityBed>();

            int bedID = 0;

            for (int i = 0; i < 25; i++)
            {
                ward1Beds.Add(new EntityBed(null, bedID++, BedType.Scheduled));
            } // end for

            bedID = 0;

            for (int i = 0; i < 6; i++)
            {
                ward2Beds.Add(new EntityBed(null, bedID++, BedType.Intensive));
            } // end for

            ward1 = new EntityWard(0, "WardSurgicalGeneral", ward1Beds, WardTypes.General);
            ward2 = new EntityWard(1, "WardSurgicalIntensive", ward2Beds, WardTypes.IntensiveCare);

            foreach (EntityBed bed in ward1Beds)
            {
                bed.ParentWard = ward1;
            } // end foreach

            foreach (EntityBed bed in ward2Beds)
            {
                bed.ParentWard = ward2;
            } // end foreach

            Dictionary<WardTypes, List<EntityWard>> wards = new Dictionary<WardTypes, List<EntityWard>>();

            wards.Add(ward1.WardType, new List<EntityWard>(new EntityWard[] { ward1 }));
            wards.Add(ward2.WardType, new List<EntityWard>(new EntityWard[] { ward2 }));

            return wards;

        } // end of GetWards

        #endregion

        #region InpatientTreatmentIsPreemptable

        public bool InpatientTreatmentIsPreemptable(InpatientTreatmentTypes treatment)
        {
            if (treatment == TreatmentTypes["InpatientSurgicalTreatment"])
                return true;

            return false;
        } // end of InpatientTreatmentIsPreemptable

        #endregion

        #region CreateOutpatientAdmission

        public OutpatientAdmissionTypes CreateOutpatientAdmission(EntityPatient patient)
        {
            return null;

            //if (MathTool.Distributions.Instance.RandomNumberGenerator.NextDouble() >= 0.3)
            //    return null;

            //return new OutpatientAdmissionTypes("OutpatientSurgicalAdmissions", false, false);
        } // end of CreateOutpatientAdmission

        #endregion

        #region GetHandledInpatientAdmissionTypes

        public List<InpatientAdmissionTypes> GetHandledInpatientAdmissionTypes()
        {
            List<InpatientAdmissionTypes> handledSurgicalInpatientAdmissionTypes = new List<InpatientAdmissionTypes>();
            handledSurgicalInpatientAdmissionTypes.Add(new InpatientAdmissionTypes("InpatientOutpatientSurgicalAdmissions", true));
            handledSurgicalInpatientAdmissionTypes.Add(new InpatientAdmissionTypes("InpatientEmergencySurgicalAdmissions", true));
            return handledSurgicalInpatientAdmissionTypes;
        } // end of GetHandledInpatientAdmissionTypes

        #endregion

        #region GetHandledInpatientTreatments

        public List<InpatientTreatmentTypes> GetHandledInpatientTreatments()
        {
            List<InpatientTreatmentTypes> handledSurgicalInpatientTreatments = new List<InpatientTreatmentTypes>();
            handledSurgicalInpatientTreatments.Add(TreatmentTypes["InpatientEmergencyTreatment"]);
            handledSurgicalInpatientTreatments.Add(TreatmentTypes["InpatientSurgicalFollowUp"]);
            handledSurgicalInpatientTreatments.Add(TreatmentTypes["InpatientSurgicalPreTreatment"]);
            handledSurgicalInpatientTreatments.Add(TreatmentTypes["InpatientSurgicalTreatment"]);
            return handledSurgicalInpatientTreatments;
        } // end of GetHandledInpatientTreatments

        #endregion

        #region GetSlotLengthPerAdmission

        public TimeSpan GetSlotLengthPerAdmission(Admission treatment)
        {
            return TimeSpan.FromDays(1);
        } // end of GetSlotLengthPerTreatment

        #endregion

        #region GetSlotCapacityPerAdmission

        public double GetSlotCapacityPerAdmission(Admission treatment)
        {
            return 1 / 10d;
        } // end of GetSlotCapacityPerTreatment

        #endregion

    } // end of InputInpatientDetailedSurgical
}

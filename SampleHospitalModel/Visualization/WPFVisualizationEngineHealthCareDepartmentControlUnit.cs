using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.DepartmentModels.Emergency;
using GeneralHealthCareElements.DepartmentModels.Outpatient;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.ResourceHandling;
using GeneralHealthCareElements.StaffHandling;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using SimulationWPFVisualizationTools.HealthCareObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WpfHealthCareObjects;
using WPFVisualizationBase;
using WPFVisualizationBase.BasicObjects;

namespace SampleHospitalModel.Visualization
{
    /// <summary>
    /// Example visualization of health care department controls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WPFVisualizationEngineHealthCareDepartmentControlUnit<T> : BaseWPFControlUnitVisualizationEngine where T : ActionTypeClass 
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, assigns visualization methods to dictionaries for event and activity types
        /// </summary>
        /// <param name="drawingSystem">Drawing system used for visualization</param>
        /// <param name="position">Position of control unit visualization on drawing system</param>
        /// <param name="size">Size of control unit visualization on drawing system</param>
        /// <param name="personSize">Size in which persons are visualized</param>
        public WPFVisualizationEngineHealthCareDepartmentControlUnit(DrawingOnCoordinateSystem drawingSystem,
                                                                     Point position,
                                                                     Size size,
                                                                     double personSize) : base(drawingSystem)
        {
            _personSize = personSize;
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityPatient), CreatePatient);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityDoctor), CreateDoctor);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityNurse), CreateNurse);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityTreatmentFacility), CreateTreatmentFacility);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityMultiplePatientTreatmentFacility), CreateMultiplePatientTreatmentFacility);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityWaitingArea), CreateWaitingRoom);

            HoldingEntitiesVisualizationMethods.Add(typeof(EntityWaitingArea), DrawHoldingEntity);
            HoldingEntitiesVisualizationMethods.Add(typeof(EntityMultiplePatientTreatmentFacility), DrawHoldingEntity);

            ActivityEndEventVisualizationMethods.Add(typeof(ActivityHealthCareAction<T>), HealthCareActionEnd);
            ActivityStartEventVisualizationMethods.Add(typeof(ActivityHealthCareAction<T>), HealthCareActionStart);

            ActivityEndEventVisualizationMethods.Add(typeof(ActivityWaitInFacility), WaitInFacilityEnd);
            ActivityStartEventVisualizationMethods.Add(typeof(ActivityWaitInFacility), WaitInFacilityStart);

            EventStandaloneDrawingMethods.Add(typeof(EventEmergencyPatientLeave), EventPatientLeave);
            EventStandaloneDrawingMethods.Add(typeof(EventOutpatientPatientLeave), EventPatientLeave);
            EventStandaloneDrawingMethods.Add(typeof(EventStaffLeave), EventLeavingStaff);

            _position = position;
            _size = size;

        } // end of BaseWPFControlUnitVisualizationEngine

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Position

        private Point _position;

        /// <summary>
        /// Position of control unit visualization on drawing system
        /// </summary>
        public Point Position
        {
            get
            {
                return _position;
            }
        } // end of Position

        #endregion

        #region Size

        private Size _size;

        /// <summary>
        /// Size of control unit visualization on drawing system
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }
        } // end of Size

        #endregion

        #region PersonSize

        private double _personSize;

        /// <summary>
        /// Size in which persons are visualized
        /// </summary>
        public double PersonSize
        {
            get
            {
                return _personSize;
            }
        } // end of PersonSize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region CreatePatient

        /// <summary>
        /// Creates a patient drawing object
        /// </summary>
        /// <param name="entity">Patient entity</param>
        /// <returns>A drawing object visualizing a patient</returns>
        public DrawingObject CreatePatient(Entity entity)
        {
            return new DrawPatient(new Point(), PersonSize, entity);
        } // end of CreatePatient

        #endregion

        #region CreateDoctor

        /// <summary>
        /// Creates a doctor drawing object
        /// </summary>
        /// <param name="entity">Doctor entity</param>
        /// <returns>A drawing object visualizing a doctor</returns>
        public DrawingObject CreateDoctor(Entity entity)
        {
            return new DrawDoctor(entity, new Point(), PersonSize, Colors.Gray);
        } // end of CreateDoctor

        #endregion

        #region CreateNurse

        /// <summary>
        /// Creates a nurse drawing object
        /// </summary>
        /// <param name="entity">Nurse entity</param>
        /// <returns>A drawing object visualizing a nurse</returns>
        public DrawingObject CreateNurse(Entity entity)
        {
            return new DrawNurse(entity, new Point(), PersonSize, Colors.Gray);
        } // end of CreateNurse

        #endregion

        #region CreateTreatmentFacility

        /// <summary>
        /// Creates a treatment facility drawing object, different objects for different
        /// skill types of treatment facilities are generated, e.g. CT or MRI
        /// </summary>
        /// <param name="entity">Treatment facility entity</param>
        /// <returns>A drawing object visualizing a treatment facility</returns>
        public DrawingObject CreateTreatmentFacility(Entity entity)
        {
            EntityTreatmentFacility treatFac = (EntityTreatmentFacility)entity;
            DrawingObject drawTreatFac;

            if (treatFac.HasSingleSill("Register"))
                drawTreatFac = new DrawRegisterBooth(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);
            else if (treatFac.HasSingleSill("MRI"))
                drawTreatFac = new DrawMRICTFacility(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);
            else if (treatFac.HasSingleSill("CT"))
                drawTreatFac = new DrawMRICTFacility(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);
            else if (treatFac.HasSingleSill("XRay"))
                drawTreatFac = new DrawXRay(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);
            else
                drawTreatFac = new DrawTreatmentFacility(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);

            drawTreatFac.CaptionSize = 24;
            drawTreatFac.SetCaption(treatFac.ToString(), CustomStringAlignment.Left);
            return drawTreatFac; 
        } // end of CreatePatient

        #endregion

        #region CreateMultiplePatientTreatmentFacility

        /// <summary>
        /// Creates a multiple patient treatment facility drawing object
        /// </summary>
        /// <param name="entity">Treatment facility entity</param>
        /// <returns>A drawing object visualizing a multiple patient treatment facility</returns>
        public DrawingObject CreateMultiplePatientTreatmentFacility(Entity entity)
        {
            EntityTreatmentFacility treatFac = (EntityTreatmentFacility)entity;
            DrawDynamicHoldingEntity drawTreatFac = new DrawDynamicHoldingEntity(treatFac, treatFac.Position, treatFac.Size, PersonSize, Colors.Gray);
            drawTreatFac.CaptionSize = 24;
            drawTreatFac.SetCaption(treatFac.ToString(), CustomStringAlignment.Left);

            return drawTreatFac;
        } // end of CreatePatient

        #endregion

        #region CreateWaitingRoom

        /// <summary>
        /// Creates a waiting room drawing object
        /// <param name="entity">Treatment facility entity</param>
        /// <returns>A drawing object visualizing a waiting room</returns>
        public DrawingObject CreateWaitingRoom(Entity entity)
        {
            EntityWaitingArea waitingArea = (EntityWaitingArea)entity;

            return new DrawDynamicHoldingEntity(waitingArea, waitingArea.Position, waitingArea.Size, PersonSize, Colors.Gray);
        } // end of CreatePatient

        #endregion

        #region DrawHoldingEntity

        /// <summary>
        /// Draws a holding entity, if size is sufficient for all entities holded, they are visualized 
        /// in a grid form, if they do not fit they are all visualized in the middle and a string
        /// representing the count of holded entities is visualized
        /// </summary>
        /// <param name="holdingEntity">Holding entity to visualize</param>
        public void DrawHoldingEntity(IDynamicHoldingEntity holdingEntity)
        {
            DrawDynamicHoldingEntity drawFoHold = (DrawDynamicHoldingEntity)DrawingObjectPerEntity((Entity)holdingEntity);

            if (drawFoHold.MaxNumberPerson < holdingEntity.HoldedEntities.Count)
            {
                drawFoHold.DrawPersonCount.StringToDraw = holdingEntity.HoldedEntities.Count.ToString();

                if (!DrawingSystem.DrawingObjects.Contains(drawFoHold.DrawPersonCount))
                    DrawingSystem.AddObject(drawFoHold.DrawPersonCount);

                foreach (DrawingObject drawObject in holdingEntity.HoldedEntities.Select(p=> DrawingObjectPerEntity(p)))
                {
                    drawObject.SetPosition(drawFoHold.DrawPersonCount.CurrentPosition - new Vector(-PersonSize, 0));
                } // end foreach
            }
            else
            {
                if (DrawingSystem.DrawingObjects.Contains(drawFoHold.DrawPersonCount))
                    DrawingSystem.RemoveObject(drawFoHold.DrawPersonCount);

                int entitiesDrawn = 0;
                double entityW = drawFoHold.SlotWidth;
                double entityH = drawFoHold.SlotHeight;

                for (int i = 0; i < drawFoHold.NumberPersonVertical; i++)
                {
                    for (int j = 0; j < drawFoHold.NumberPersonHorizontal; j++)
                    {
                        if (entitiesDrawn == holdingEntity.HoldedEntities.Count)
                            return;

                        DrawingObject drawEntity = DrawingObjectPerEntity(holdingEntity.HoldedEntities[entitiesDrawn]);

                        drawEntity.SetPosition(drawFoHold.CurrentPosition + new Vector(j * entityW + drawFoHold.SlotWidth / 2, i * entityH));

                        entitiesDrawn++;
                    } // end for
                } // end for
            } // end if
            
        } // end of DrawHoldingEntity

        #endregion

        #region AdditionalStaticVisualization

        /// <summary>
        /// Creates the static visualization, draws all treatment facilities, waiting rooms and multiple patient
        /// treatment facilities for each structural area
        /// </summary>
        /// <param name="initializationTime">Time at which static visualization is generated</param>
        /// <param name="simModel">Simulation model for which the visuslization is generated</param>
        /// <param name="parentControlUnit">Control unit that is visualized</param>
        public override void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            ControlUnitHealthCareDepartment healthDepartmentControl = (ControlUnitHealthCareDepartment)parentControlUnit;

            foreach (StructuralArea structArea in healthDepartmentControl.StructuralAreas)
            {
                foreach (EntityTreatmentFacility treatFac in structArea.TreatmentFacilities)
                {
                    DrawingObjectPerEntity(treatFac);
                } // end foreach

                foreach (EntityTreatmentFacility treatFac in structArea.MultiplePatientTreatmentFacilities)
                {
                    DrawingObjectPerEntity(treatFac);
                } // end foreach

                foreach (EntityWaitingArea waitArea in structArea.WaitingAreasPatients)
                {
                    DrawingObjectPerEntity(waitArea);
                } // end foreach

                DrawingObjectPerEntity(structArea.StaffWaitingRoom);

            } // end foreach

        } // end of AdditionalStaticVisualization

        #endregion

        #region HealthCareActionEnd

        /// <summary>
        /// Method that visualized the end of a health care action, patient is removed from
        /// the drawing system
        /// </summary>
        /// <param name="activity">Instance of health care action</param>
        /// <param name="time">Time the activity is ended</param>
        public void HealthCareActionEnd(Activity activity, DateTime time)
        {
            ActivityHealthCareAction<T> action = (ActivityHealthCareAction<T>)activity;

            if (action.ResourceSet.TreatmentFacility is EntityMultiplePatientTreatmentFacility)
                return;

            RemoveDrawingObjectPerEntity(action.Patient);
        } // end of HealthCareActionEnd

        #endregion

        #region HealthCareActionStart

        /// <summary>
        /// Method that visualized the start of a health care action, patient is added to
        /// the drawing system and all entities (staff and patient) are set to
        /// the corresponding positions in the treatment facility (if it is not a
        /// multiple patient treatment facility).
        /// </summary>
        /// <param name="activity">Instance of health care action</param>
        /// <param name="time">Time the activity is started</param>
        public void HealthCareActionStart(Activity activity, DateTime time)
        {
            ActivityHealthCareAction<T> action = (ActivityHealthCareAction<T>)activity;

            if (action.ResourceSet.TreatmentFacility is EntityMultiplePatientTreatmentFacility)
                return;

            DrawPatient drawPatient = (DrawPatient)DrawingObjectPerEntity(action.Patient);

            DrawBaseTreatmentFacility drawTreatFac = (DrawBaseTreatmentFacility)DrawingObjectPerEntity(action.ResourceSet.TreatmentFacility);

            drawPatient.SetPositionType(drawTreatFac.PatientPositionType);

            drawPatient.SetPosition(drawTreatFac.PatientInRoomPosition);

            int staffCount = 0;

            if (action.ResourceSet.MainDoctor != null)
            {
                DrawDoctor drawMainDoctor = (DrawDoctor)DrawingObjectPerEntity(action.ResourceSet.MainDoctor);
                drawMainDoctor.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                staffCount++;
            } // end if

            if (action.ResourceSet.MainNurse != null)
            {
                DrawNurse drawMainNurse = (DrawNurse)DrawingObjectPerEntity(action.ResourceSet.MainNurse);
                drawMainNurse.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                staffCount++;
            } // end if

            if (action.ResourceSet.AssistingDoctors != null)
            {
                foreach (EntityDoctor ent in action.ResourceSet.AssistingDoctors)
                {
                    DrawDoctor drawAssistDoctor = (DrawDoctor)DrawingObjectPerEntity(ent);
                    drawAssistDoctor.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                    staffCount++;
                } // end foreach
            } // end if

            if (action.ResourceSet.AssistingNurses != null)
            {
                foreach (EntityNurse ent in action.ResourceSet.AssistingNurses)
                {
                    DrawNurse drawAssistNurse = (DrawNurse)DrawingObjectPerEntity(ent);
                    drawAssistNurse.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                    staffCount++;
                } // end foreach
            } // end if

        } // end of HealthCareActionEnd

        #endregion

        #region WaitInFacilityEnd

        /// <summary>
        /// Method that visualized the end of a wait in facility activity, patient is removed from
        /// the drawing system
        /// </summary>
        /// <param name="activity">Instance of wait in facility activity</param>
        /// <param name="time">Time the activity is ended</param>
        public void WaitInFacilityEnd(Activity activity, DateTime time)
        {
            ActivityWaitInFacility waitInFrac = (ActivityWaitInFacility)activity;

            if (waitInFrac.Facility is EntityMultiplePatientTreatmentFacility)
                return;

            RemoveDrawingObjectPerEntity(waitInFrac.Patient);
        } // end of WaitInFacilityEnd

        #endregion

        #region WaitInFacilityStart

        /// <summary>
        /// Method that visualized the start of a wait in facility action, patient is added to
        /// the drawing system and set to the corresponding positions in the treatment facility 
        /// (if it is not a multiple patient treatment facility).
        /// </summary>
        /// <param name="activity">Instance of health care action</param>
        /// <param name="time">Time the activity is started</param>
        public void WaitInFacilityStart(Activity activity, DateTime time)
        {
            ActivityWaitInFacility waitInFac = (ActivityWaitInFacility)activity;

            if (waitInFac.Facility is EntityMultiplePatientTreatmentFacility)
                return;

            DrawPatient drawPatient = (DrawPatient)DrawingObjectPerEntity(waitInFac.Patient);

            DrawTreatmentFacility drawTreatFac = (DrawTreatmentFacility)DrawingObjectPerEntity(waitInFac.Facility);

            drawPatient.SetPositionType(drawTreatFac.PatientPositionType);

            drawPatient.SetPosition(drawTreatFac.PatientInRoomPosition);

        } // end of WaitInFacilityEnd

        #endregion

        #region EventPatientLeave

        /// <summary>
        /// Upon patient leave events the drawing object is removed from the system
        /// </summary>
        /// <param name="ev">Patient leave event</param>
        public void EventPatientLeave(Event ev)
        {
            EntityPatient patient = null;

            if (ev is EventEmergencyPatientLeave)
                patient = ((EventEmergencyPatientLeave)ev).Patient;
            else if (ev is EventOutpatientPatientLeave)
                patient = ((EventOutpatientPatientLeave)ev).Patient;

            RemoveDrawingObjectPerEntity(patient);

        } // end of EventPatientLeave

        #endregion

        #region EventStaffLeave

        /// <summary>
        /// Upon staff leave events the drawing object is removed from the system
        /// </summary>
        /// <param name="ev">Staff leave event</param>
        public void EventLeavingStaff(Event ev)
        {
            EventStaffLeave staffLeave = (EventStaffLeave)ev;

            RemoveDrawingObjectPerEntity(staffLeave.StaffLeaving);

        } // end of EventPatientLeave

        #endregion

    } // end of WPFVisualizationEngineHealthCareDepartmentControlUnit
}

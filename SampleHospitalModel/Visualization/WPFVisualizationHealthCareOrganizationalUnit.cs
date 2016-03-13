using GeneralHealthCareElements.Activities;
using GeneralHealthCareElements.Entities;
using GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths;
using GeneralHealthCareElements.StaffHandling;
using SimulationCore.HCCMElements;
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

namespace SampleHospitalModel.Visualization
{
    /// <summary>
    /// Example visualization of health care organizational controls
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class WPFVisualizationHealthCareOrganizationalUnit<T>  : BaseWPFControlUnitVisualizationEngine where T : ActionTypeClass
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
        public WPFVisualizationHealthCareOrganizationalUnit(DrawingOnCoordinateSystem drawingSystem,
                                                                     double personSize,
                                                            WPFVisualizationEngineHealthCareDepartmentControlUnit<T> parentDepartmentVisualization) : base(drawingSystem)
        {
            _personSize = personSize;
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityPatient), CreatePatient);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityDoctor), CreateDoctor);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityNurse), CreateNurse);

            HoldingEntitiesVisualizationMethods.Add(typeof(EntityMultiplePatientTreatmentFacility), DrawHoldingEntity);

            ActivityEndEventVisualizationMethods.Add(typeof(ActivityHealthCareAction<T>), HealthCareActionEnd);
            ActivityStartEventVisualizationMethods.Add(typeof(ActivityHealthCareAction<T>), HealthCareActionStart);

            ActivityEndEventVisualizationMethods.Add(typeof(ActivityWaitInFacility), WaitInFacilityEnd);
            ActivityStartEventVisualizationMethods.Add(typeof(ActivityWaitInFacility), WaitInFacilityStart);

            EventStandaloneDrawingMethods.Add(typeof(EventStaffLeave), EventLeavingStaff);

            _parentDepartmentVisaulization = parentDepartmentVisualization;

        } // end of BaseWPFControlUnitVisualizationEngine

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

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

        #region ParentDepartmentVisualization

        private WPFVisualizationEngineHealthCareDepartmentControlUnit<T> _parentDepartmentVisaulization;

        public WPFVisualizationEngineHealthCareDepartmentControlUnit<T> ParentDepartmentVisualization
        {
            get
            {
                return _parentDepartmentVisaulization;
            }
        } // end of ParentDepartmentVisualization

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

        #region HealthCareActionEnd

        /// <summary>
        /// Method that visualized the end of a health care action, patient and other human resources are removed from
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

            if (action.ResourceSet.MainDoctor != null)
            {
                RemoveDrawingObjectPerEntity(action.ResourceSet.MainDoctor);
            } // end if

            if (action.ResourceSet.MainNurse != null)
            {
                RemoveDrawingObjectPerEntity(action.ResourceSet.MainNurse);
            } // end if

            if (action.ResourceSet.AssistingDoctors != null)
            {
                foreach (EntityDoctor ent in action.ResourceSet.AssistingDoctors)
                {
                    RemoveDrawingObjectPerEntity(ent);
                } // end foreach
            } // end if

            if (action.ResourceSet.AssistingNurses != null)
            {
                foreach (EntityNurse ent in action.ResourceSet.AssistingNurses)
                {
                    RemoveDrawingObjectPerEntity(ent);
                } // end foreach
            } // end if

        } // end of HealthCareActionEnd

        #endregion

        #region HealthCareActionStart

        /// <summary>
        /// Method that visualized the start of a health care action, patient is added to
        /// the drawing system and all entities (staff and patient) are set to
        /// the corresponding positions in the treatment facility (if it is not a
        /// multiple patient treatment facility). Further, human resources are
        /// removed from the parent visualization engine to avoid double drawing.
        /// </summary>
        /// <param name="activity">Instance of health care action</param>
        /// <param name="time">Time the activity is started</param>
        public void HealthCareActionStart(Activity activity, DateTime time)
        {
            ActivityHealthCareAction<T> action = (ActivityHealthCareAction<T>)activity;

            if (action.ResourceSet.TreatmentFacility is EntityMultiplePatientTreatmentFacility)
                return;

            DrawPatient drawPatient = (DrawPatient)DrawingObjectPerEntity(action.Patient);

            DrawBaseTreatmentFacility drawTreatFac = (DrawBaseTreatmentFacility)ParentDepartmentVisualization.DrawingObjectPerEntity(action.ResourceSet.TreatmentFacility);

            drawPatient.SetPositionType(drawTreatFac.PatientPositionType);

            drawPatient.SetPosition(drawTreatFac.PatientInRoomPosition);

            int staffCount = 0;

            if (action.ResourceSet.MainDoctor != null)
            {
                ParentDepartmentVisualization.RemoveDrawingObjectPerEntity(action.ResourceSet.MainDoctor);
                DrawDoctor drawMainDoctor = (DrawDoctor)DrawingObjectPerEntity(action.ResourceSet.MainDoctor);
                drawMainDoctor.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                staffCount++;
            } // end if

            if (action.ResourceSet.MainNurse != null)
            {
                ParentDepartmentVisualization.RemoveDrawingObjectPerEntity(action.ResourceSet.MainNurse);
                DrawNurse drawMainNurse = (DrawNurse)DrawingObjectPerEntity(action.ResourceSet.MainNurse);
                drawMainNurse.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                staffCount++;
            } // end if

            if (action.ResourceSet.AssistingDoctors != null)
            {
                foreach (EntityDoctor ent in action.ResourceSet.AssistingDoctors)
                {
                    ParentDepartmentVisualization.RemoveDrawingObjectPerEntity(ent);
                    DrawDoctor drawAssistDoctor = (DrawDoctor)DrawingObjectPerEntity(ent);
                    drawAssistDoctor.SetPosition(drawTreatFac.GetStaffPosition(staffCount));
                    staffCount++;
                } // end foreach
            } // end if

            if (action.ResourceSet.AssistingNurses != null)
            {
                foreach (EntityNurse ent in action.ResourceSet.AssistingNurses)
                {
                    ParentDepartmentVisualization.RemoveDrawingObjectPerEntity(ent);
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

            DrawTreatmentFacility drawTreatFac = (DrawTreatmentFacility)ParentDepartmentVisualization.DrawingObjectPerEntity(waitInFac.Facility);

            drawPatient.SetPositionType(drawTreatFac.PatientPositionType);

            drawPatient.SetPosition(drawTreatFac.PatientInRoomPosition);

        } // end of WaitInFacilityEnd

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


    } // end of WPFVisualizationHealthCareOrganizationalUnit
}

using SimpleQueueExample.ModelElements;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using SimulationWPFVisualizationTools.HealthCareObjects;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace SimpleQueueExample.Drawing
{
    /// <summary>
    /// Very simple and basic visualization of queuing model, a bit hacky at some points
    /// </summary>
    public class QueueControlVisualizationEngine : BaseWPFControlUnitVisualizationEngine
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, sets entity drawing object methods and event and activity visualization methods
        /// </summary>
        /// <param name="drawingSystem"></param>
        public QueueControlVisualizationEngine(DrawingOnCoordinateSystem drawingSystem) : base(drawingSystem)
        {
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityClient), CreateClient);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityQueue), CreateQueue);
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(EntityServer), CreateServer);
            ActivityEndEventVisualizationMethods.Add(typeof(ActivityGetServed), GetServedEnd);
            ActivityStartEventVisualizationMethods.Add(typeof(ActivityGetServed), GetServedStart);
            HoldingEntitiesVisualizationMethods.Add(typeof(EntityQueue), DrawHoldingEntity);
        } // end of BaseWPFControlUnitVisualizationEngine

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region CreateClient

        /// <summary>
        /// Create client drawing object as a person visuaization object
        /// </summary>
        /// <param name="entity">Client entity</param>
        /// <returns>DrawPerson visualization object</returns>
        public DrawingObject CreateClient(Entity entity)
        {
            return new DrawPerson(new Point(0, 0), 30, Colors.LightGray);
        } // end of CreateClient

        #endregion CreateClient

        #region CreateQueue

        /// <summary>
        /// Create queue drawing object as a rectangle with caption
        /// </summary>
        /// <param name="entity">Queue entity</param>
        /// <returns>DrawingRectangleObject visualization object</returns>
        public DrawingObject CreateQueue(Entity entity)
        {
            DrawDynamicHoldingEntity queue = new DrawDynamicHoldingEntity(entity, new Point(20, 0), new Size(200, 40), 30, Colors.LightGray);
            queue.SetCaption(string.Format("Queue: {0}", entity.Identifier), CustomStringAlignment.Left);
            return queue;
        } // end of CreateQueue

        #endregion CreateQueue

        #region CreateServer

        /// <summary>
        /// Create server drawing object as a rectangle with caption
        /// </summary>
        /// <param name="entity">Server entity</param>
        /// <returns>DrawingRectangleObject visualization object</returns>
        public DrawingObject CreateServer(Entity entity)
        {
            DrawDynamicHoldingEntity newServer = new DrawDynamicHoldingEntity(entity, new Point(300, 0), new Size(40, 40), 30, Colors.LightGray);
            newServer.SetCaption(string.Format("Server: {0}", entity.Identifier), CustomStringAlignment.Left);
            return newServer;
        } // end of CreateServer

        #endregion CreateServer

        #region GetServedStart

        /// <summary>
        /// Method to change visualiztion at service start, sets client in the server rectangle
        /// </summary>
        /// <param name="activity">GetServed activity</param>
        /// <param name="time">Time activity is started</param>
        public void GetServedStart(Activity activity, DateTime time)
        {
            DrawingObject clientDraw = DrawingObjectPerEntity(((ActivityGetServed)activity).Client);
            DrawingObject serverDraw = DrawingObjectPerEntity(((ActivityGetServed)activity).Server);

            clientDraw.SetPosition(new Point(serverDraw.CurrentPosition.X + serverDraw.DrawingShape.Data.Bounds.Width / 2, serverDraw.CurrentPosition.Y + 5));
        } // end of GetServedStart

        #endregion GetServedStart

        #region GetServedEnd

        /// <summary>
        /// Method to change visualiztion at service end, remove client drawing object from drawing system
        /// </summary>
        /// <param name="activity">GetServed activity</param>
        /// <param name="time">Time activity is ended</param>
        public void GetServedEnd(Activity activity, DateTime time)
        {
            DrawingSystem.RemoveObject(DrawingObjectPerEntity(((ActivityGetServed)activity).Client));
        } // end of GetServedStart

        #endregion GetServedEnd

        #region DrawHoldingEntity

        /// <summary>
        /// Visualized a dynamic holding entity (in this example a queue) by adding clients in the rectangle,
        /// no overflow handling (will draw infinitely)
        /// </summary>
        /// <param name="holdingEntity"></param>
        public void DrawHoldingEntity(IDynamicHoldingEntity holdingEntity)
        {
            DrawDynamicHoldingEntity drawFoHold = (DrawDynamicHoldingEntity)DrawingObjectPerEntity((Entity)holdingEntity);

            if (drawFoHold.MaxNumberPerson < holdingEntity.HoldedEntities.Count)
            {
                drawFoHold.DrawPersonCount.StringToDraw = holdingEntity.HoldedEntities.Count.ToString();

                if (!DrawingSystem.DrawingObjects.Contains(drawFoHold.DrawPersonCount))
                    DrawingSystem.AddObject(drawFoHold.DrawPersonCount);

                foreach (DrawingObject drawObject in holdingEntity.HoldedEntities.Select(p => DrawingObjectPerEntity(p)))
                {
                    drawObject.SetPosition(drawFoHold.DrawPersonCount.CurrentPosition - new Vector(-30, 0));
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

        #endregion DrawHoldingEntity

        #region AdditionalStaticVisualization

        /// <summary>
        /// Static visualization of model, draws servers and queues
        /// </summary>
        /// <param name="initializationTime">Time static visualization is initialized</param>
        /// <param name="simModel">Simulation model to draw</param>
        /// <param name="parentControlUnit">Control unit to draw</param>
        public override void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            ControlUnitQueuingModel queueControl = (ControlUnitQueuingModel)parentControlUnit;

            DrawingSystem.ClearSystem();

            for (int i = 0; i < queueControl.Queues.Count; i++)
            {
                DrawingObject drawForQueue = DrawingObjectPerEntity(queueControl.Queues[i]);
                drawForQueue.SetPosition(new Point(20, i * 80));
            } // end for

            for (int i = 0; i < queueControl.Servers.Count; i++)
            {
                DrawingObject drawForServer = DrawingObjectPerEntity(queueControl.Servers[i]);
                drawForServer.SetPosition(new Point(300, i * 80));
            } // end for
        } // end of AdditionalStaticVisualization

        #endregion AdditionalStaticVisualization
    } // end of QueueControlVisualizationEngine
}
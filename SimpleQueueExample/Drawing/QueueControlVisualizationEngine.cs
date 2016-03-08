using SimpleQueueExample.ModelElements;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;
using WPFVisualizationBase.BasicObjects;

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

        #endregion

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

        #endregion

        #region CreateQueue

        /// <summary>
        /// Create queue drawing object as a rectangle with caption
        /// </summary>
        /// <param name="entity">Queue entity</param>
        /// <returns>DrawingRectangleObject visualization object</returns>
        public DrawingObject CreateQueue(Entity entity)
        {
            DrawingRectangleObject newQueue = new DrawingRectangleObject(new Point(20, 0), 200, 40, Colors.LightGray, false);
            newQueue.SetCaption(string.Format("Queue: {0}", entity.Identifier), CustomStringAlignment.Left);
            return newQueue;
        } // end of CreateQueue

        #endregion

        #region CreateServer

        /// <summary>
        /// Create server drawing object as a rectangle with caption
        /// </summary>
        /// <param name="entity">Server entity</param>
        /// <returns>DrawingRectangleObject visualization object</returns>
        public DrawingObject CreateServer(Entity entity)
        {
            DrawingRectangleObject newServer = new DrawingRectangleObject(new Point(300, 0), 40, 40, Colors.LightGray, false);
            newServer.SetCaption(string.Format("Server: {0}", entity.Identifier), CustomStringAlignment.Left);
            return newServer;
        } // end of CreateServer

        #endregion

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

        #endregion

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

        #endregion

        #region DrawHoldingEntity

        /// <summary>
        /// Visualized a dynamic holding entity (in this example a queue) by adding clients in the rectangle,
        /// no overflow handling (will draw infinitely)
        /// </summary>
        /// <param name="holdingEntity"></param>
        public void DrawHoldingEntity(IDynamicHoldingEntity holdingEntity)
        {
            DrawingObject rectforQueue = DrawingObjectPerEntity((Entity)holdingEntity);

            for (int i = 0; i < holdingEntity.HoldedEntities.Count; i++)
            {
                DrawingObjectPerEntity(holdingEntity.HoldedEntities[i]).SetPosition(
                    new Point(rectforQueue.CurrentPosition.X + rectforQueue.DrawingShape.ActualWidth - 15 - i * 40, rectforQueue.CurrentPosition.Y + 5));

            } // end for
        } // end of DrawHoldingEntity

        #endregion

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

        #endregion

    } // end of QueueControlVisualizationEngine
}

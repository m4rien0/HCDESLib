using ActorDemo2.Model;
using ActorDemo2.Model.Platform;
using ActorDemo2.Model.ProductionAssets;
using ActorDemo2.Model.Transactions;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace ActorDemo2.Visualization
{
    public class ActorDemoVisualizationEngine : BaseWPFControlUnitVisualizationEngine
    {
        private double _currentY = 0.0;

        public ActorDemoVisualizationEngine(DrawingOnCoordinateSystem drawingSystem) : base(drawingSystem)
        {
            ActivityStartEventVisualizationMethods.Add(typeof(OrderActivity), DrawOrderStart);
            ActivityEndEventVisualizationMethods.Add(typeof(OrderActivity), DrawOrderEnd);

            ActivityStartEventVisualizationMethods.Add(typeof(TransactionActivity), DrawTransactionStart);
            ActivityEndEventVisualizationMethods.Add(typeof(TransactionActivity), DrawTransactionEnd);

            ActivityStartEventVisualizationMethods.Add(typeof(CheckActivity), DrawCheckStart);
            ActivityEndEventVisualizationMethods.Add(typeof(CheckActivity), DrawCheckEnd);

            EventStandaloneDrawingMethods.Add(typeof(LineInstalledEvent), DrawEnd);
        }

        public override void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            ActorDemoPlatformControlUnit controlUnit = (ActorDemoPlatformControlUnit)parentControlUnit;

            DrawingSystem.ClearSystem();
        }

        private void DrawCheckEnd(Activity activity, DateTime time)
        {
            CheckActivity checkActivity = (CheckActivity)activity;
            string content = $"{checkActivity.PlatformStaff} finished checking transaction {checkActivity.Transaction}";
            DrawText(content, time);
        }

        private void DrawCheckStart(Activity activity, DateTime time)
        {
            CheckActivity checkActivity = (CheckActivity)activity;
            string content = $"{checkActivity.PlatformStaff} started checking transaction {checkActivity.Transaction}";
            DrawText(content, time);
        }

        private void DrawEnd(Event ev)
        {
            DrawingObjectString text = new(new Point(0, _currentY), "Line successfully installed!", CustomStringAlignment.Left, 12, Colors.Black);
            DrawingSystem.AddObject(text);
        }

        private void DrawOrderEnd(Activity activity, DateTime time)
        {
            OrderActivity orderActivity = (OrderActivity)activity;
            string content = $"Order ({orderActivity.Order.Id}) by {orderActivity.Order.Creator} ended. {orderActivity.Order.Transactions.Count} transactions were executed.";
            DrawText(content, time);
        }

        private void DrawOrderStart(Activity activity, DateTime time)
        {
            OrderActivity orderActivity = (OrderActivity)activity;
            string content = $"New order ({orderActivity.Order.Id}) started by {orderActivity.Order.Creator}";
            DrawText(content, time);
        }

        private void DrawText(string content, DateTime time)
        {
            DrawingObjectString text = new(new Point(0, _currentY), $"{time}: {content}", CustomStringAlignment.Left, 12, Colors.Black);
            _currentY -= 15;
            DrawingSystem.AddObject(text);
        }

        private void DrawTransactionEnd(Activity activity, DateTime time)
        {
            TransactionActivity transactionActivity = (TransactionActivity)activity;
            string content = $"{transactionActivity.Transaction} ended.";
            DrawText(content, time);
        }

        private void DrawTransactionStart(Activity activity, DateTime time)
        {
            TransactionActivity transactionActivity = (TransactionActivity)activity;
            string content = $"{transactionActivity.Transaction} started.";
            DrawText(content, time);
        }
    }
}
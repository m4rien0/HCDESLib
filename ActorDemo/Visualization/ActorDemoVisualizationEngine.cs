using ActorDemo.Model.Activities;
using ActorDemo.Model.ControlUnits;
using ActorDemo.Model.Entities;
using ActorDemo.Model.Events;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace ActorDemo.Visualization
{
    public class ActorDemoVisualizationEngine : BaseWPFControlUnitVisualizationEngine
    {
        private readonly DrawingObject _magnifyingGlass;
        private DrawingObject? _tradeArrow;

        public ActorDemoVisualizationEngine(DrawingOnCoordinateSystem drawingSystem) : base(drawingSystem)
        {
            EntityVisualizationObjectCreatingMethodsPerType.Add(typeof(FactoryOwner), DrawFactoryOwner);

            EventStandaloneDrawingMethods.Add(typeof(EquipmentNeededEvent), DrawSearch);
            EventStandaloneDrawingMethods.Add(typeof(SearchCompletedEvent), DrawSearchDone);

            ActivityStartEventVisualizationMethods.Add(typeof(BuyActivity), DrawTradeStart);
            ActivityEndEventVisualizationMethods.Add(typeof(BuyActivity), DrawTradeEnd);

            ActivityStartEventVisualizationMethods.Add(typeof(TransportActivity), DrawTransportStart);
            ActivityEndEventVisualizationMethods.Add(typeof(TransportActivity), DrawTransportEnd);

            _magnifyingGlass = new DrawMagnifyingGlass(new Point(0, 0));
        }

        public override void AdditionalStaticVisualization(DateTime initializationTime, SimulationModel simModel, ControlUnit parentControlUnit)
        {
            ActorDemoPlatformControlUnit controlUnit = (ActorDemoPlatformControlUnit)parentControlUnit;

            DrawingSystem.ClearSystem();

            DrawingObject seller = DrawingObjectPerEntity(controlUnit.Seller);
            seller.SetPosition(new Point(50, 0));

            DrawingObject buyer = DrawingObjectPerEntity(controlUnit.Buyer);
            buyer.SetPosition(new Point(1050, 0));
        }

        private DrawingObject DrawFactoryOwner(Entity entity)
        {
            return new DrawActor(new Point(0, 0));
        }

        private void DrawSearch(Event ev)
        {
            DrawingObject buyer = DrawingObjectPerEntity(((EquipmentNeededEvent)ev).Buyer);

            DrawingSystem.AddObject(_magnifyingGlass);
            _magnifyingGlass.SetPosition(new Point(buyer.CurrentPosition.X + 200, buyer.CurrentPosition.Y + 200));
        }

        private void DrawSearchDone(Event ev)
        {
            DrawingSystem.RemoveObject(_magnifyingGlass);
        }

        private void DrawTradeEnd(Activity activity, DateTime time)
        {
            DrawingSystem.RemoveObject(_tradeArrow);
            _tradeArrow = null;
        }

        private void DrawTradeStart(Activity activity, DateTime time)
        {
            BuyActivity buyActivity = (BuyActivity)activity;
            DrawingObject seller = DrawingObjectPerEntity(buyActivity.Seller);
            DrawingObject buyer = DrawingObjectPerEntity(buyActivity.Buyer);

            _tradeArrow = new DrawTwoSidedArrow(new Point(0, 0), buyer.CurrentPosition.X - 50 - 193 - 20);
            DrawingSystem.AddObject(_tradeArrow);
            _tradeArrow.SetPosition(new Point(seller.CurrentPosition.X + 193 + 10, seller.CurrentPosition.Y + 125));
            _tradeArrow.CaptionSize = 10;
            _tradeArrow.CaptionTypeFace = new Typeface("Arial");
        }

        private void DrawTransportEnd(Activity activity, DateTime time)
        {
            if (_tradeArrow == null)
            {
                return;
            }

            // CAUTION: Hack here to remove caption, will need a proper implementation
            // if this should be needed later in the simulation

            GeometryGroup arrowGeometry = (GeometryGroup)_tradeArrow.DrawingShape.Data;
            arrowGeometry.Children.RemoveAt(arrowGeometry.Children.Count - 1);
        }

        private void DrawTransportStart(Activity activity, DateTime time)
        {
            _tradeArrow?.SetCaption("Transporting goods from seller to buyer");
        }

        private class DrawMagnifyingGlass : DrawingObject
        {
            public DrawMagnifyingGlass(Point startPosition) : base(startPosition)
            {
                EllipseGeometry glass = new(startPosition, 20, 20);
                LineGeometry lineSegment = new(new Point(13.5, -13.5), new Point(40, -40));

                GeometryGroup g = new() { Children = [glass, lineSegment] };

                _drawingShape.Data = g;
                DrawingShape.Stroke = new SolidColorBrush(Colors.Black);
                DrawingShape.StrokeThickness = 2;
            }
        }

        private class DrawTwoSidedArrow : DrawingObject
        {
            public DrawTwoSidedArrow(Point startPosition, double length) : base(startPosition)
            {
                PathFigure arrowPath = new(startPosition,
                    [
                        new LineSegment(new Point(25, 25), true),
                        new LineSegment(new Point(0, 0), false),
                        new LineSegment(new Point(25, -25), true),
                        new LineSegment(new Point(0, 0), false),
                        new LineSegment(new Point(length, 0), true),
                        new LineSegment(new Point(length - 25, 25), true),
                        new LineSegment(new Point(length, 0), false),
                        new LineSegment(new Point(length - 25, - 25), true),
                    ],
                    false
                );

                _drawingShape.Data = new GeometryGroup()
                {
                    Children = [new PathGeometry() { Figures = [arrowPath] }]
                };
                DrawingShape.StrokeThickness = 2.0;
                DrawingShape.Stroke = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
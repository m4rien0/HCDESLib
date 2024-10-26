using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace ActorDemo.Visualization
{
    public class DrawActor : DrawingObject
    {
        public DrawActor(Point startPosition) : base(startPosition)
        {
            PathFigure factoryPath = new(startPosition,
                [
                    new LineSegment(new Point(5, 250), true),
                    new LineSegment(new Point(35, 250), true),
                    new LineSegment(new Point(43, 100), true),
                    new LineSegment(new Point(93, 125), true),
                    new LineSegment(new Point(93, 100), true),
                    new LineSegment(new Point(143, 125), true),
                    new LineSegment(new Point(143, 100), true),
                    new LineSegment(new Point(193, 125), true),
                    new LineSegment(new Point(193, 0), true)
                ],
                true
            );

            _drawingShape.Data = new PathGeometry() { Figures = [factoryPath] };
            DrawingShape.Fill = new SolidColorBrush(Colors.Transparent);
            DrawingShape.StrokeThickness = 2.0;
            DrawingShape.Stroke = new SolidColorBrush(Colors.Black);
        }
    }
}
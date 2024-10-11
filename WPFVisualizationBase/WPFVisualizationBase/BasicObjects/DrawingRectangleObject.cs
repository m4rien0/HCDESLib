using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase.BasicObjects
{
    /// <summary>
    /// Basis object to draw a rectangle
    /// </summary>
    public class DrawingRectangleObject : DrawingObject
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startPosition">Start position of rectangle</param>
        /// <param name="widht">Width</param>
        /// <param name="height">Height</param>
        /// <param name="color">Color to draw</param>
        /// <param name="fill">True if rectangle should be filled</param>
        /// <param name="thickness">Thickness of outline</param>
        /// <param name="radiusX">Corner radius of outline</param>
        /// <param name="radiusY">Corner radius of outline</param>
        public DrawingRectangleObject(Point startPosition,
            double widht,
            double height,
            Color color,
            bool fill,
            double thickness = 10,
            double radiusX = 0,
            double radiusY = 0) : base(startPosition)
        {
            GeometryGroup geometries = new GeometryGroup();

            geometries.Children.Add(new RectangleGeometry(new Rect(new Point(0, 0), new Vector(widht, height)), radiusX, radiusY));

            if (fill)
                DrawingShape.Fill = new SolidColorBrush(color);

            DrawingShape.Stroke = new SolidColorBrush(color);
            DrawingShape.StrokeThickness = thickness;

            _drawingShape.Data = geometries;
        } // end of DrawingObject

        #endregion Constructor
    } // end of DrawingRectangleObject
}
using System;
using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase
{
    /// <summary>
    /// Basic drawing object to visualize the outline of an analog clock without pointers
    /// </summary>
    public class AnalogClockOutlineDrawingObject : DrawingObject
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, constructs gemoetry of clock outline
        /// </summary>
        /// <param name="startPosition">Position of clock (center of clock)</param>
        /// <param name="radius">Size of clock</param>
        /// <param name="color">Color to display</param>
        public AnalogClockOutlineDrawingObject(Point startPosition, double radius,
                             Color color) : base(startPosition)
        {
            Point origin = new Point(0, 0);
            EllipseGeometry clockOutline = new EllipseGeometry(origin, radius, radius);
            Vector upVector = new Vector(0, 1);
            Vector sideVector = new Vector(1, 0);
            GeometryGroup geometries = new GeometryGroup();

            geometries.Children.Add(clockOutline);

            float PI = (float)Math.PI;

            // draw hour indicators
            for (int i = 0; i < 12; i++)
            {
                double lengthShare;

                // full quatars are displayed longer
                if (i % 3 == 0)
                    lengthShare = 0.75;
                else
                    lengthShare = 0.9;

                double hourRad = i * 360 / 12 * PI / 180;
                LineGeometry line = new LineGeometry(origin + (Math.Cos(hourRad) * sideVector + Math.Sin(hourRad) * upVector) * radius * lengthShare,
                    origin + (Math.Cos(hourRad) * sideVector + Math.Sin(hourRad) * upVector) * radius);

                geometries.Children.Add(line);
            } // end for

            _drawingShape.Data = geometries;
            DrawingShape.Stroke = new SolidColorBrush(color);
        } // end of DrawingObject

        #endregion Constructor
    } // end of AnalogClockDrawingObject
}
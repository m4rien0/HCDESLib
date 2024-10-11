using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase
{
    /// <summary>
    /// Basis object to draw a line
    /// </summary>
    public class DrawObjectLine : DrawingObject
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startPosition">Start position of rectangle</param>
        /// <param name="vector">Vector representing line</param>
        /// <param name="color">Color to draw</param>
        public DrawObjectLine(Point startPosition, Vector vector,
                             Color color) : base(startPosition)
        {
            List<Geometry> geometries = new List<Geometry>();

            DrawingShape.Stroke = new SolidColorBrush(color);

            DrawingShape.Data = new LineGeometry(new Point(0, 0), new Point(0, 0) + vector);
        } // end of DrawingObject

        #endregion Constructor
    } // end of DrawObjectLine
}
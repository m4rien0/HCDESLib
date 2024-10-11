using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase
{
    /// <summary>
    /// Basic visualization of a human being in upright position
    /// </summary>
    public class DrawPerson : DrawingObject
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startPosition">Start position of numan, bottom of human and centered</param>
        /// <param name="size">Size of human (height)</param>
        /// <param name="color">Color in which human should be displayed</param>
        public DrawPerson(Point startPosition, double size, Color color)
            : base(startPosition)
        {
            double sizeHead = 0.2 * size;
            double sizeBody = 0.4 * size;
            double sizeLegs = 0.4 * size;

            _width = size * 0.4;
            double legspace = _width / 5;
            double armspace = _width / 6.5;
            double headDiameter = sizeHead * 3 / 4;

            //head
            EllipseGeometry head = new EllipseGeometry(new Point(0, sizeBody + sizeLegs + (sizeHead - headDiameter / 2)), headDiameter / 2, headDiameter / 2);

            // body
            RectangleGeometry body = new RectangleGeometry(new Rect(new Point(-_width / 4, sizeLegs), new Point(_width / 4, sizeLegs + sizeBody)));

            // legs
            PathGeometry leftLeg = Geometry.Combine(new RectangleGeometry(new Rect(new Point(-_width / 4, legspace / 2), new Point(-(_width / 4 - legspace), sizeLegs))),
                                                    new EllipseGeometry(new Rect((-_width / 4), 0, legspace, legspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            PathGeometry rightLeg = Geometry.Combine(new RectangleGeometry(new Rect(new Point(_width / 4, legspace / 2), new Point((_width / 4 - legspace), sizeLegs))),
                                                     new EllipseGeometry(new Rect((_width / 4 - legspace), 0, legspace, legspace)),
                                                     GeometryCombineMode.Union,
                                                     Transform.Identity);

            // shoulders
            PathGeometry leftShoulder = new PathGeometry();
            LineSegment lowLineLeft = new LineSegment(new Point(-_width / 2, sizeLegs + sizeBody - _width / 4), true);

            ArcSegment arcSegmentLeft = new ArcSegment();
            arcSegmentLeft.Point = new Point(-_width / 4, sizeLegs + sizeBody);
            arcSegmentLeft.Size = new Size(_width / 4, _width / 4);

            PathFigure sholderFigureLeft = new PathFigure(new Point(-_width / 4, sizeLegs + sizeBody - _width / 4), new PathSegment[] { lowLineLeft, arcSegmentLeft }, true);
            leftShoulder.Figures.Add(sholderFigureLeft);

            PathGeometry rightShoulder = new PathGeometry();
            LineSegment lowLineRight = new LineSegment(new Point(_width / 4, sizeLegs + sizeBody - _width / 4), true);

            ArcSegment arcSegmentRight = new ArcSegment();
            arcSegmentRight.Point = new Point(_width / 2, sizeLegs + sizeBody - _width / 4);
            arcSegmentRight.Size = new Size(_width / 4, _width / 4);

            PathFigure sholderFigureRight = new PathFigure(new Point(_width / 4, sizeLegs + sizeBody), new PathSegment[] { arcSegmentRight, lowLineRight }, true);
            rightShoulder.Figures.Add(sholderFigureRight);

            // arms
            // legs
            PathGeometry leftArm = Geometry.Combine(new RectangleGeometry(new Rect(new Point(-_width / 2, armspace / 2 + sizeLegs), new Point(-(_width / 2 - armspace), sizeLegs + sizeBody - _width / 4))),
                                                    new EllipseGeometry(new Rect((-_width / 2), sizeLegs, armspace, armspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            PathGeometry rightArm = Geometry.Combine(new RectangleGeometry(new Rect(new Point(_width / 2 - armspace, armspace / 2 + sizeLegs), new Point(_width / 2, sizeLegs + sizeBody - _width / 4))),
                                                    new EllipseGeometry(new Rect((_width / 2 - armspace), sizeLegs, armspace, armspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            GeometryGroup geometries = new GeometryGroup();

            foreach (Geometry geom in new Geometry[] { head, body, leftShoulder, rightShoulder, leftLeg, rightLeg, leftArm, rightArm })
            {
                geometries.Children.Add(geom);
            } // end foreach

            _drawingShape.Data = geometries;
            DrawingShape.Fill = new SolidColorBrush(color);
            DrawingShape.Stroke = new SolidColorBrush(color);
        } // end of Constructor

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Width

        private double _width;

        /// <summary>
        /// Width of human, autoamtically set with respect to height
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
        } // end of Width

        #endregion Width
    } // end of DrawPerson
}
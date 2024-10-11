using SimulationCore.HCCMElements;
using SimulationWPFVisualizationTools;
using System.Windows;
using System.Windows.Media;

namespace WpfHealthCareObjects
{
    /// <summary>
    /// Example of how to visualize a doctor, not nicely coded
    /// </summary>
    public class DrawDoctor : DrawingObjectForEntity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, all geometries for path shape are added in constructor
        /// </summary>
        /// <param name="correspondingEntity">Corresponding doctor</param>
        /// <param name="startPosition">Poistion where doctor should be visualized</param>
        /// <param name="size">Height of doctor</param>
        /// <param name="color">Color in which doctor should be visualized</param>
        public DrawDoctor(Entity correspondingEntity, Point startPosition, double size, Color color)
            : base(startPosition, correspondingEntity)
        {
            double sizeHead = 0.2 * size;
            double sizeBody = 0.4 * size;
            double sizeLegs = 0.4 * size;

            _width = size * 0.4;
            double legspace = _width / 5;
            double armspace = _width / 6.5;
            double headDiameter = sizeHead * 3 / 4;

            //head
            GeometryGroup head = new GeometryGroup();

            EllipseGeometry headBig = new EllipseGeometry(new Point(0, sizeBody + sizeLegs + (sizeHead - headDiameter / 2)), headDiameter / 2, headDiameter / 2);

            EllipseGeometry headCutOut = new EllipseGeometry(new Point(0, sizeBody + sizeLegs + (sizeHead - headDiameter / 10)), headDiameter / 3, headDiameter / 6);

            PathGeometry headMain = Geometry.Combine(headBig, headCutOut, GeometryCombineMode.Exclude, Transform.Identity);

            EllipseGeometry headTopBase = new EllipseGeometry(new Point(0, sizeBody + sizeLegs + sizeHead * 1.1), headDiameter / 4, headDiameter / 4);
            EllipseGeometry headTopCutOut = new EllipseGeometry(new Point(0, sizeBody + sizeLegs + sizeHead * 1.1), headDiameter / 8, headDiameter / 8);
            PathGeometry headTop = Geometry.Combine(headTopBase, headTopCutOut, GeometryCombineMode.Exclude, Transform.Identity);

            head.Children.Add(headMain);
            head.Children.Add(headTop);

            // body

            PathGeometry body = new PathGeometry();

            RectangleGeometry mainBody = new RectangleGeometry(new Rect(new Point(-_width / 4, sizeLegs), new Point(_width / 4, sizeLegs + sizeBody)));

            GeometryGroup statoscopeLeft = new GeometryGroup();
            statoscopeLeft.FillRule = FillRule.Nonzero;

            statoscopeLeft.Children.Add(new LineGeometry(new Point(-_width / 10, sizeLegs + sizeBody), new Point(-_width / 10, sizeLegs + sizeBody * 5 / 6.02)));
            statoscopeLeft.Children.Add(new LineGeometry(new Point(-_width / 10, sizeLegs + sizeBody * 5 / 6), new Point(-_width / 10 - _width / 10, sizeLegs + sizeBody * 2 / 3)));
            statoscopeLeft.Children.Add(new LineGeometry(new Point(-_width / 10, sizeLegs + sizeBody * 5 / 6), new Point(-_width / 10 + _width / 10, sizeLegs + sizeBody * 2 / 3)));
            statoscopeLeft.Children.Add(new LineGeometry(new Point(-_width / 10 - _width / 10, sizeLegs + sizeBody * 2.06 / 3), new Point(-_width / 10 - _width / 10, sizeLegs + sizeBody * 1 / 2)));
            statoscopeLeft.Children.Add(new LineGeometry(new Point(-_width / 10 + _width / 10, sizeLegs + sizeBody * 2.06 / 3), new Point(-_width / 10 + _width / 10, sizeLegs + sizeBody * 1 / 2)));

            GeometryGroup statoscopeRight = new GeometryGroup();
            statoscopeRight.FillRule = FillRule.Nonzero;

            statoscopeRight.Children.Add(new LineGeometry(new Point(_width / 8, sizeLegs + sizeBody), new Point(_width / 8, sizeLegs + sizeBody * 5 / 6)));
            statoscopeRight.Children.Add(new EllipseGeometry(new Point(_width / 8, sizeLegs + sizeBody * 5 / 6 - _width / 15), _width / 15, _width / 15));

            Pen strokePen = new Pen(new SolidColorBrush(color), _width / 15);
            PathGeometry pathstatoscopeLeft = statoscopeLeft.GetWidenedPathGeometry(strokePen);
            PathGeometry pathstatoscopeRight = statoscopeRight.GetWidenedPathGeometry(strokePen);

            body = Geometry.Combine(mainBody, pathstatoscopeLeft, GeometryCombineMode.Exclude, Transform.Identity);
            body = Geometry.Combine(body, pathstatoscopeRight, GeometryCombineMode.Exclude, Transform.Identity);

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
            PathGeometry leftArm = Geometry.Combine(new RectangleGeometry(new Rect(new Point(-_width / 2, armspace / 2 + sizeLegs), new Point(-(_width / 2 - armspace), sizeLegs + sizeBody - _width / 4))),
                                                    new EllipseGeometry(new Rect((-_width / 2), sizeLegs, armspace, armspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            PathGeometry rightArm = Geometry.Combine(new RectangleGeometry(new Rect(new Point(_width / 2 - armspace, armspace / 2 + sizeLegs), new Point(_width / 2, sizeLegs + sizeBody - _width / 4))),
                                                    new EllipseGeometry(new Rect((_width / 2 - armspace), sizeLegs, armspace, armspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            //SetGeometries(new Geometry[] { head, body, leftShoulder, rightShoulder, leftLeg, rightLeg, leftArm, rightArm });
            GeometryGroup geometries = new GeometryGroup();

            foreach (Geometry geom in new Geometry[] { head, body, leftShoulder, rightShoulder, leftLeg, rightLeg, leftArm, rightArm })
            {
                geometries.Children.Add(geom);
            } // end foreach

            DrawingShape.Data = geometries;

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
        /// Widht of doctor
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
        } // end of Width

        #endregion Width
    } // end of DrawPatient
}
using SimulationCore.HCCMElements;
using SimulationWPFVisualizationTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace WpfHealthCareObjects
{
    /// <summary>
    /// Example of how to visualize a nurse, not nicely coded
    /// </summary>
    public class DrawNurse : DrawingObjectForEntity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, all geometries for path shape are added in constructor
        /// </summary>
        /// <param name="correspondingEntity">Corresponding nurse</param>
        /// <param name="startPosition">Poistion where nurse should be visualized</param>
        /// <param name="size">Height of nurse</param>
        /// <param name="color">Color in which nurse should be visualized</param>
        public DrawNurse(Entity correspondingEntity, Point startPosition, double size, Color color)
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

            PathGeometry nurseHat = new PathGeometry();
            List<PathSegment> hatSegments = new List<PathSegment>();
            hatSegments.Add(new LineSegment(new Point(-headDiameter / 2 -headDiameter / 5, headDiameter/ 3 + headDiameter * 3 / 2 + sizeBody + sizeLegs), true));
            hatSegments.Add(new LineSegment(new Point(headDiameter / 2 + headDiameter / 5, headDiameter / 3 + headDiameter * 3 / 2 + sizeBody + sizeLegs), true));
            hatSegments.Add(new LineSegment(new Point(headDiameter / 2, headDiameter / 3 + headDiameter + sizeBody + sizeLegs), true));
            hatSegments.Add(new ArcSegment(new Point(-headDiameter / 2, headDiameter / 3.3 + headDiameter + sizeBody + sizeLegs), new Size(headDiameter, headDiameter), 0, false, SweepDirection.Clockwise, true));
            PathFigure hatFigure = new PathFigure(new Point(-headDiameter / 2, headDiameter / 3.3 + headDiameter + sizeBody + sizeLegs), hatSegments, true);
            nurseHat.Figures.Add(hatFigure);

            nurseHat = Geometry.Combine(nurseHat, new RectangleGeometry(new Rect(new Point(-headDiameter / 5, headDiameter * 3.3 / 2 + sizeBody + sizeLegs - headDiameter / 10), new Point(headDiameter / 5, headDiameter * 3.3 / 2 + sizeBody + sizeLegs + headDiameter / 10))), GeometryCombineMode.Exclude, Transform.Identity);
            nurseHat = Geometry.Combine(nurseHat, new RectangleGeometry(new Rect(new Point(-headDiameter / 10, headDiameter * 3.3 / 2 + sizeBody + sizeLegs - headDiameter / 5), new Point(headDiameter / 10, headDiameter * 3.3 / 2 + sizeBody + sizeLegs + headDiameter / 5.5))), GeometryCombineMode.Exclude, Transform.Identity);

            head.Children.Add(headBig);
            head.Children.Add(nurseHat);

            // body

            //RectangleGeometry body = new RectangleGeometry(new Rect(new Point(-_width / 4, sizeLegs + 2 * sizeBody / 3), new Point(_width / 4, sizeLegs + sizeBody)));

            PathGeometry body = new PathGeometry();
            List<PathSegment> bodySegments = new List<PathSegment>();
            bodySegments.Add(new LineSegment(new Point(-_width / 4, sizeLegs + 2 * sizeBody / 3), true));
            bodySegments.Add(new LineSegment(new Point(-_width / 2, sizeLegs), true));
            bodySegments.Add(new LineSegment(new Point(_width / 2, sizeLegs), true));
            bodySegments.Add(new LineSegment(new Point(_width / 4, sizeLegs + 2 * sizeBody / 3), true));
            bodySegments.Add(new LineSegment(new Point(_width / 4, sizeLegs + sizeBody), true));
            PathFigure bodyFigure = new PathFigure(new Point(-_width / 4, sizeLegs + sizeBody), bodySegments, true);
            body.Figures.Add(bodyFigure);

            // legs
            PathGeometry leftLeg = Geometry.Combine(new RectangleGeometry(new Rect(new Point(-_width / 4, legspace / 2), new Point(-(_width / 4 - legspace), sizeLegs))),
                                                    new EllipseGeometry(new Rect((-_width / 4), 0, legspace, legspace)),
                                                    GeometryCombineMode.Union,
                                                    Transform.Identity);

            PathGeometry rightLeg = Geometry.Combine(new RectangleGeometry(new Rect(new Point(_width / 4, legspace / 2), new Point((_width / 4 - legspace), sizeLegs))),
                                                     new EllipseGeometry(new Rect((_width / 4 - legspace), 0, legspace, legspace)),
                                                     GeometryCombineMode.Union,
                                                     Transform.Identity);

            // left arm and shoulder
            PathGeometry leftArmShoulder = new PathGeometry();
            List<PathSegment> leftArmSegments = new List<PathSegment>();
            leftArmSegments.Add(new LineSegment(new Point(-_width / 4 - armspace, sizeLegs + sizeBody), true));
            leftArmSegments.Add(new LineSegment(new Point(-_width / 2 - armspace, sizeLegs * 1.3), true));
            leftArmSegments.Add(new ArcSegment(new Point(- _width / 2, sizeLegs * 1.3), new Size(armspace / 2, armspace / 2), 0, true, SweepDirection.Clockwise, true));
            leftArmSegments.Add(new LineSegment(new Point(-_width / 4, sizeLegs + sizeBody * 0.9 ), true));

            PathFigure sholderFigureLeft = new PathFigure(new Point(-_width / 4, sizeLegs + sizeBody), leftArmSegments, true);
            leftArmShoulder.Figures.Add(sholderFigureLeft);

            // left arm and shoulder
            PathGeometry rightArmShoulder = new PathGeometry();
            List<PathSegment> rightArmSegments = new List<PathSegment>();
            rightArmSegments.Add(new LineSegment(new Point(_width / 4 + armspace, sizeLegs + sizeBody), true));
            rightArmSegments.Add(new LineSegment(new Point(_width / 2 + armspace, sizeLegs * 1.3), true));
            rightArmSegments.Add(new ArcSegment(new Point(_width / 2, sizeLegs * 1.3), new Size(armspace / 2,armspace / 2 ), 0, false, SweepDirection.Counterclockwise, true));
            rightArmSegments.Add(new LineSegment(new Point(_width / 4, sizeLegs + sizeBody * 0.9), true));

            PathFigure sholderFigureRight = new PathFigure(new Point(_width / 4, sizeLegs + sizeBody), rightArmSegments, true);
            rightArmShoulder.Figures.Add(sholderFigureRight);

            GeometryGroup geometries = new GeometryGroup();

                foreach (Geometry geom in new Geometry[] { head, body, leftLeg, rightLeg, leftArmShoulder, rightArmShoulder})
            {
                geometries.Children.Add(geom);
            } // end foreach

            DrawingShape.Data = geometries;
            DrawingShape.Fill = new SolidColorBrush(color);
            DrawingShape.Stroke = new SolidColorBrush(color);
        } // end of Constructor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Width

        private double _width;

        /// <summary>
        /// Widht of nurse
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
        } // end of Width

        #endregion

    } // end of DrawNurse
}

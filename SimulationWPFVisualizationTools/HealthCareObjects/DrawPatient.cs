using SimulationCore.HCCMElements;
using SimulationWPFVisualizationTools.HealthCareObjects;
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
    /// Visualization of a patient, basically a standard person visualization. However,
    /// three geometries are computed that can then be assigned to the shape for the different
    /// position types of a patient, upright, lying in bed (upper body slightly upright) and
    /// flag lying.
    /// </summary>
    public class DrawPatient : DrawPerson
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, additional two geometries are computed (lying in bed and flat lying), upright is computed 
        /// in the base constructor
        /// </summary>
        /// <param name="startPosition">Position of patient</param>
        /// <param name="size">Height of patient</param>
        /// <param name="correspondingEntity">Corresponding patient entity</param>
        public DrawPatient(Point startPosition, double size, Entity correspondingEntity) : base(startPosition, size, Colors.Gray)
        {
            _correspondingEntity = correspondingEntity;
            _positionType = PatientPositionInRoomType.UpRight;
            _uprightGeometry = DrawingShape.Data;

            double sizeHead = 0.2 * size;
            double sizeBody = 0.4 * size;
            double sizeLegs = 0.4 * size;

            double legspace = Width / 5;
            double armspace = Width / 6.5;
            double headDiameter = sizeHead * 3 / 4;

            #region InBedGeometry

            GeometryGroup inBedPatient = new GeometryGroup();

            inBedPatient.FillRule = FillRule.Nonzero;

            //body + leg
            List<PathSegment> bodySegments = new List<PathSegment>();

            double sin30 = Math.Sin(30 * Math.PI / 180);
            double cos30 = Math.Cos(30 * Math.PI / 180);

            bodySegments.Add(new ArcSegment(new Point(legspace / 2, legspace), new Size(legspace / 2, legspace / 2), 0, false, SweepDirection.Counterclockwise, true));
            bodySegments.Add(new LineSegment(new Point(sizeLegs, legspace), true));
            Point upperBodyFront = new Point(sizeLegs + cos30 * sizeBody, legspace + sin30 * sizeBody);
            bodySegments.Add(new LineSegment(upperBodyFront, true));
            Vector backVector = new Vector(sin30 * legspace, -cos30 * legspace);
            Point upperBodyBack = upperBodyFront + new Vector(sin30 * legspace, -cos30 * legspace);
            bodySegments.Add(new ArcSegment(upperBodyBack, new Size(legspace / 2, legspace / 2), 0, true, SweepDirection.Counterclockwise, true));
            Point lowerBodyBack = upperBodyBack + new Vector(-cos30 * sizeBody, -sin30 * sizeBody);
            bodySegments.Add(new LineSegment(lowerBodyBack, true));
            PathFigure bodyFigure = new PathFigure(new Point(legspace / 2, 0), bodySegments, true);

            PathGeometry bodyGeom = new PathGeometry();
            bodyGeom.Figures.Add(bodyFigure);

            inBedPatient.Children.Add(bodyGeom);

            // head

            //head
            EllipseGeometry head = new EllipseGeometry(upperBodyFront + new Vector(0, legspace * 1.3), headDiameter / 2, headDiameter / 2);

            inBedPatient.Children.Add(head);

            // arm

            List<PathSegment> armSegments = new List<PathSegment>();
            armSegments.Add(new ArcSegment(new Point(sizeLegs * 1.2, size / 6 + armspace), new Size(armspace / 2, armspace / 2), 0, false, SweepDirection.Counterclockwise, true));
            armSegments.Add(new LineSegment(new Point(sizeLegs * 1.7, size / 6 + armspace), true));
            armSegments.Add(new LineSegment(new Point(sizeLegs * 1.7, size / 6), true));

            PathFigure armFigure = new PathFigure(new Point(sizeLegs * 1.2, size / 6), armSegments, true);

            PathGeometry armGeom = new PathGeometry();
            armGeom.Figures.Add(armFigure);

            inBedPatient.Children.Add(armGeom);

            _inBedGeometry = inBedPatient;

            #endregion

            #region FlatLyingGeometry

            GeometryGroup flatPatient = new GeometryGroup();

            flatPatient.FillRule = FillRule.Nonzero;

            //legs

            PathGeometry leg = Geometry.Combine(new RectangleGeometry(new Rect(new Point(legspace / 2, 0), new Point(sizeLegs - legspace / 6, legspace))),
                                                     new EllipseGeometry(new Rect(0, 0, legspace, legspace)),
                                                     GeometryCombineMode.Union,
                                                     Transform.Identity);

            flatPatient.Children.Add(leg);

            //body

            PathGeometry body = Geometry.Combine(new RectangleGeometry(new Rect(new Point(sizeLegs + legspace / 6, 0), new Point(sizeBody + sizeLegs - legspace / 2, legspace))),
                                                     new EllipseGeometry(new Rect(sizeBody + sizeLegs - legspace, 0, legspace, legspace)),
                                                     GeometryCombineMode.Union,
                                                     Transform.Identity);

            flatPatient.Children.Add(body);

            // head

            EllipseGeometry headFlat = new EllipseGeometry(new Rect(sizeBody + sizeLegs + legspace/5, 0, headDiameter, headDiameter));

            flatPatient.Children.Add(headFlat);

            _flatGeometry = flatPatient;

            #endregion

            DrawingShape.ToolTip = correspondingEntity.ToString();
        } // end of DrawPatient

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region UprightGeometry

        private Geometry _uprightGeometry;

        /// <summary>
        /// Geometry used for visualization of the patient in an upright position
        /// </summary>
        public Geometry UprightGeometry
        {
            get
            {
                return _uprightGeometry;
            }
        } // end of UprightGeometry

        #endregion

        #region InBedGeometry

        private Geometry _inBedGeometry;

        /// <summary>
        /// Geometry used for visualization of the patient in an inbed position
        /// </summary>
        public Geometry InBedGeometry
        {
            get
            {
                return _inBedGeometry;
            }
        } // end of InBedGeometry

        #endregion

        #region FlatGeometry

        private Geometry _flatGeometry;

        /// <summary>
        /// Geometry used for visualization of the patient in a flat lying position
        /// </summary>
        public Geometry FlatGeometry
        {
            get
            {
                return _flatGeometry;
            }
        } // end of FlatGeometry

        #endregion

        #region PositionType

        private PatientPositionInRoomType _positionType;

        /// <summary>
        /// Current position type that is visualized
        /// </summary>
        public PatientPositionInRoomType PositionType
        {
            get
            {
                return _positionType;
            }
        } // end of PositionType

        #endregion

        #region SetPositionType

        /// <summary>
        /// Method to change the visualization type, the correpsonding geometry is chosen
        /// and assigned to the path shape property of DrawingObject
        /// </summary>
        /// <param name="positionType">Position type in which patient should be visualized</param>
        public void SetPositionType(PatientPositionInRoomType positionType)
        {
            switch (positionType)
            {
                case PatientPositionInRoomType.UpRight:
                    DrawingShape.Data = UprightGeometry;
                    break;
                case PatientPositionInRoomType.LyingInBed:
                    DrawingShape.Data = InBedGeometry;
                    break;
                case PatientPositionInRoomType.FlatLying:
                    DrawingShape.Data = FlatGeometry;
                    break;
                default:
                    DrawingShape.Data = UprightGeometry;
                    break;
            }

            _positionType = positionType; ;

        } // end of SetPositionType
        
        #endregion

        #region CorrespondingEntity

        private Entity _correspondingEntity;

        /// <summary>
        /// Corresponding patient entity
        /// </summary>
        public Entity CorrespondingEntity
        {
            get
            {
                return _correspondingEntity;
            }
        } // end of CorrespondingEntity

        #endregion

    } // end of DrawPatient
}

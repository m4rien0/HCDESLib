using SimulationCore.HCCMElements;
using System.Windows;
using System.Windows.Media;

namespace SimulationWPFVisualizationTools.HealthCareObjects
{
    /// <summary>
    /// Extending the DrawBaseTreatmentFacility to visualize register facilities.
    /// In the contructor a desk with a screen with iconized data is added to the geometry collection.
    /// Patient is displayed left of desk, staff right of desk
    /// </summary>
    public class DrawRegisterBooth : DrawBaseTreatmentFacility
    {
        #region Constructor

        /// <summary>
        /// Geomtries are created,a desk with a screen with iconized data is added to the geometry collection.
        /// Patient is displayed left of desk, staff right of desk.
        /// </summary>
        /// <param name="correspondingEntity">Treatment facility the drawing object represents</param>
        /// <param name="startPosition">Position where the treatment facility should be drawn</param>
        /// <param name="size">Size of treatment facility</param>
        /// <param name="personSize">Height used to visualize persons</param>
        /// <param name="color">Color in which the facility should be displayed</param>
        public DrawRegisterBooth(Entity correspondingEntity,
                                     Point startPosition,
                                     Size size,
                                     double personSize,
                                     Color color)
            : base(correspondingEntity, startPosition, size, personSize, color)
        {
            double lineSize = personSize / 15;

            Point origin = new Point(0, 0);

            _patientPositionType = PatientPositionInRoomType.UpRight;

            _staffStartPosition = origin + new Vector(origin.X + 2 * personSize, personSize * 0.2);

            GeometryGroup geometries = new GeometryGroup();

            // outline
            geometries.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(origin, size), lineSize, lineSize),
                                                     new RectangleGeometry(new Rect(origin + new Vector(lineSize, lineSize), new Size(size.Width - 2 * lineSize, size.Height - 2 * lineSize)), lineSize, lineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            // desk
            GeometryGroup desk = new GeometryGroup();

            desk.FillRule = FillRule.Nonzero;

            Point deskStart = origin + new Vector(origin.X + personSize / 1.2, personSize * 0.2);

            desk.Children.Add(new RectangleGeometry(new Rect(deskStart, new Size(lineSize, personSize / 2)), lineSize / 2, lineSize / 2));
            desk.Children.Add(new RectangleGeometry(new Rect(deskStart + new Vector(lineSize * 10, 0), new Size(lineSize, personSize / 2)), lineSize / 2, lineSize / 2));
            desk.Children.Add(new RectangleGeometry(new Rect(deskStart + new Vector(-lineSize, personSize / 2), new Size(lineSize * 13, lineSize)), lineSize / 2, lineSize / 2));

            geometries.Children.Add(desk);

            // monitor
            double monitorLineSize = lineSize / 2;
            Size monitorSize = new Size(personSize / 3, personSize / 4);

            GeometryGroup monitor = new GeometryGroup();

            Point monitorStart = deskStart + new Vector(10.5 * lineSize, 3 * lineSize) + new Vector(-personSize / 2, personSize / 2);

            monitor.FillRule = FillRule.Nonzero;

            monitor.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(monitorStart, monitorSize), monitorLineSize, monitorLineSize),
                                                     new RectangleGeometry(new Rect(monitorStart + new Vector(monitorLineSize, monitorLineSize), new Size(monitorSize.Width - 2 * monitorLineSize, monitorSize.Height - 2 * monitorLineSize)), monitorLineSize, monitorLineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            monitor.Children.Add(new RectangleGeometry(new Rect(monitorStart + new Vector(monitorSize.Width / 2 - monitorLineSize / 2, -2 * lineSize), new Size(monitorLineSize, 2 * lineSize)), monitorLineSize, monitorLineSize));

            double dataLength = monitorSize.Width - 2 * lineSize;

            monitor.Children.Add(new LineGeometry(monitorStart + new Vector(lineSize, lineSize), monitorStart + new Vector(lineSize + dataLength, lineSize)));
            monitor.Children.Add(new LineGeometry(monitorStart + new Vector(lineSize, 1.5 * lineSize), monitorStart + new Vector(lineSize + dataLength, 1.5 * lineSize)));
            monitor.Children.Add(new LineGeometry(monitorStart + new Vector(lineSize, 2 * lineSize), monitorStart + new Vector(lineSize + dataLength, 2 * lineSize)));
            monitor.Children.Add(new LineGeometry(monitorStart + new Vector(lineSize, 2.5 * lineSize), monitorStart + new Vector(lineSize + dataLength, 2.5 * lineSize)));

            geometries.Children.Add(monitor);

            _patientInRoomPosition = new Point(origin.X + 2 * lineSize + personSize / 4, origin.Y + 2 * lineSize);

            _drawingShape.Data = geometries;
        } // end of Constructor

        #endregion Constructor
    } // end of DrawRegisterBooth
}
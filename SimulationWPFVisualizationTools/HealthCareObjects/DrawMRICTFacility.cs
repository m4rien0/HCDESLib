﻿using SimulationCore.HCCMElements;
using System.Windows;
using System.Windows.Media;

namespace SimulationWPFVisualizationTools.HealthCareObjects
{
    /// <summary>
    /// Extending the DrawBaseTreatmentFacility to visualize MRI and CT facilities
    /// In the contructor a tube icon is added to the geometries. Patient is in
    /// a flat lying position in this facility
    /// </summary>
    public class DrawMRICTFacility : DrawBaseTreatmentFacility
    {
        #region Constructor

        /// <summary>
        /// Geomtries are created, a tube icon is added to the geometries. Patient is in
        /// a flat lying position in this facility. Staff is displayed  on the bottom of the facility
        /// </summary>
        /// <param name="correspondingEntity">Treatment facility the drawing object represents</param>
        /// <param name="startPosition">Position where the treatment facility should be drawn</param>
        /// <param name="size">Size of treatment facility</param>
        /// <param name="personSize">Height used to visualize persons</param>
        /// <param name="color">Color in which the facility should be displayed</param>
        public DrawMRICTFacility(Entity correspondingEntity,
                                     Point startPosition,
                                     Size size,
                                     double personSize,
                                     Color color)
            : base(correspondingEntity, startPosition, size, personSize, color)
        {
            double lineSize = personSize / 15;

            _patientPositionType = PatientPositionInRoomType.FlatLying;

            Point origin = new Point(0, 0);

            GeometryGroup geometries = new GeometryGroup();

            // outline
            geometries.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(origin, size), lineSize, lineSize),
                                                     new RectangleGeometry(new Rect(origin + new Vector(lineSize, lineSize), new Size(size.Width - 2 * lineSize, size.Height - 2 * lineSize)), lineSize, lineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            // bed
            GeometryGroup bed = new GeometryGroup();

            bed.FillRule = FillRule.Nonzero;

            Point bedStart = origin + new Vector(origin.X + personSize / 1.5 + lineSize, personSize * 1.6);

            bed.Children.Add(new RectangleGeometry(new Rect(bedStart, new Size(personSize - lineSize, lineSize)), lineSize / 2, lineSize / 2));

            geometries.Children.Add(bed);

            // tube

            EllipseGeometry tubeBig = new EllipseGeometry(bedStart + new Vector(personSize - lineSize, lineSize), personSize / 2, personSize / 2);

            EllipseGeometry tubeInner = new EllipseGeometry(bedStart + new Vector(personSize - lineSize, lineSize), personSize / 4, personSize / 4);

            PathGeometry tubeCutoutInner = Geometry.Combine(tubeBig, tubeInner, GeometryCombineMode.Exclude, Transform.Identity);

            RectangleGeometry rectCutout = new RectangleGeometry(new Rect(bedStart + new Vector(0.4 * personSize, -personSize / 10), new Size(personSize / 2, personSize / 2.8)));

            PathGeometry tube = Geometry.Combine(tubeCutoutInner, rectCutout, GeometryCombineMode.Exclude, Transform.Identity);

            geometries.Children.Add(tube);

            // monitor
            double monitorLineSize = lineSize / 2;
            Size monitorSize = new Size(personSize / 3, personSize / 4);

            Point monitorStart = bedStart - new Vector(0, personSize / 3);

            GeometryGroup monitor = new GeometryGroup();

            monitor.FillRule = FillRule.Nonzero;

            monitor.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(monitorStart + new Vector(-personSize / 2, personSize / 2), monitorSize), monitorLineSize, monitorLineSize),
                                                     new RectangleGeometry(new Rect(monitorStart + new Vector(-personSize / 2 + monitorLineSize, personSize / 2 + monitorLineSize), new Size(monitorSize.Width - 2 * monitorLineSize, monitorSize.Height - 2 * monitorLineSize)), monitorLineSize, monitorLineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            monitor.Children.Add(new RectangleGeometry(new Rect(monitorStart + new Vector(-personSize / 2 + monitorSize.Width / 2 - monitorLineSize / 2, monitorLineSize), new Size(monitorLineSize, personSize / 2 - monitorLineSize)), monitorLineSize, monitorLineSize));
            monitor.Children.Add(new RectangleGeometry(new Rect(monitorStart + new Vector(-personSize / 2 + monitorLineSize, 0), new Size(monitorSize.Width - 2 * monitorLineSize, monitorLineSize)), monitorLineSize, monitorLineSize));

            geometries.Children.Add(monitor);

            _staffStartPosition = new Point(origin.X + 2 * lineSize + personSize / 4, origin.Y + 2 * lineSize);

            _patientInRoomPosition = bedStart + new Vector(0, 1.5 * lineSize);

            _drawingShape.Data = geometries;
        } // end of Constructor

        #endregion Constructor
    } // end of DrawMRICTFacility
}
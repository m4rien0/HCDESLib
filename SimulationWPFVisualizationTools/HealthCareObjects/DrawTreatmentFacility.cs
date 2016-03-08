using SimulationCore.HCCMElements;
using SimulationWPFVisualizationTools;
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
    /// General treatment facility, patient is in a inbed position
    /// </summary>
    public class DrawTreatmentFacility : DrawBaseTreatmentFacility
    {
        #region Constructor

        /// <summary>
        /// Geomtries are created, a bed is created, patient is in a inbed position in the bed. A screen
        /// with iconized vital data is added, as well as an iconized infusion. Staff members are visualized
        /// at the bottom of the facility.
        /// </summary>
        /// <param name="correspondingEntity">Treatment facility the drawing object represents</param>
        /// <param name="startPosition">Position where the treatment facility should be drawn</param>
        /// <param name="size">Size of treatment facility</param>
        /// <param name="personSize">Height used to visualize persons</param>
        /// <param name="color">Color in which the facility should be displayed</param>
        public DrawTreatmentFacility(Entity correspondingEntity, 
                                     Point startPosition,
                                     Size size,
                                     double personSize,
                                     Color color)
            : base(correspondingEntity, startPosition, size, personSize, color)
        {
            double lineSize = personSize / 15;

            _patientPositionType = PatientPositionInRoomType.LyingInBed;

            Point origin = new Point(0, 0);

            _staffStartPosition = new Point(origin.X + 2 * lineSize + personSize / 4, origin.Y + 2 * lineSize);

            GeometryGroup geometries = new GeometryGroup();

            // outline
            geometries.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(origin, size), lineSize, lineSize),
                                                     new RectangleGeometry(new Rect(origin + new Vector(lineSize, lineSize), new Size(size.Width - 2 * lineSize, size.Height - 2 * lineSize)), lineSize, lineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));
          
            // bed
            GeometryGroup bed = new GeometryGroup();

            bed.FillRule = FillRule.Nonzero;

            Point bedStart = origin + new Vector(origin.X + personSize / 1.5, personSize * 1.4);

            bed.Children.Add(new RectangleGeometry(new Rect(bedStart, new Size(lineSize, personSize / 2)), lineSize / 2, lineSize / 2));
            bed.Children.Add(new RectangleGeometry(new Rect(bedStart + new Vector(personSize,0), new Size(lineSize, 2 * personSize / 3)), lineSize / 2, lineSize / 2));
            bed.Children.Add(new RectangleGeometry(new Rect(bedStart + new Vector(lineSize, personSize / 5), new Size(personSize - lineSize, lineSize)), lineSize / 2, lineSize / 2));

            bed.Children.Add(new RectangleGeometry(new Rect(bedStart + new Vector(2 * personSize / 3.2, personSize / 5 + lineSize), new Size(personSize / 2.3, lineSize)), lineSize / 2, lineSize / 2, new RotateTransform(30, bedStart.X + 2 * personSize / 3, bedStart.Y + personSize / 5 + lineSize)));

            geometries.Children.Add(bed);

            _patientInRoomPosition = bedStart + new Vector(personSize / 10, personSize / 3.5);

            // monitor
            double monitorLineSize = lineSize / 2;
            Size monitorSize = new Size(personSize / 3, personSize / 4);

            GeometryGroup monitor = new GeometryGroup();

            monitor.FillRule = FillRule.Nonzero;

            monitor.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(bedStart + new Vector(-personSize / 2, personSize / 2), monitorSize), monitorLineSize, monitorLineSize),
                                                     new RectangleGeometry(new Rect(bedStart + new Vector(-personSize / 2 + monitorLineSize, personSize / 2 + monitorLineSize), new Size(monitorSize.Width - 2 * monitorLineSize, monitorSize.Height - 2 * monitorLineSize)), monitorLineSize, monitorLineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            monitor.Children.Add(new RectangleGeometry(new Rect(bedStart + new Vector(-personSize / 2 + monitorSize.Width / 2 - monitorLineSize / 2, monitorLineSize), new Size(monitorLineSize, personSize / 2- monitorLineSize)), monitorLineSize, monitorLineSize));
            monitor.Children.Add(new RectangleGeometry(new Rect(bedStart + new Vector(-personSize / 2 + monitorLineSize, 0), new Size(monitorSize.Width - 2 * monitorLineSize, monitorLineSize)), monitorLineSize, monitorLineSize));

            Point dataPointStart = bedStart + new Vector(-personSize / 2 + 2 * monitorLineSize, personSize / 2 + 3 * monitorLineSize);

            Point nextDataPoint = dataPointStart + new Vector(monitorSize.Width / 8, monitorSize.Height / 3);
            monitor.Children.Add(new LineGeometry(dataPointStart, nextDataPoint));
            monitor.Children.Add(new LineGeometry(nextDataPoint, nextDataPoint + new Vector(2 * monitorSize.Width / 8, -monitorSize.Height / 2)));
            nextDataPoint = nextDataPoint + new Vector(2 * monitorSize.Width / 8, -monitorSize.Height / 2);
            monitor.Children.Add(new LineGeometry(nextDataPoint, nextDataPoint + new Vector(2 * monitorSize.Width / 8, monitorSize.Height / 6)));

            geometries.Children.Add(monitor);

            // infusion
            GeometryGroup infusion = new GeometryGroup();

            infusion.FillRule = FillRule.Nonzero;

            Point infusionPosition = bedStart + new Vector(personSize * 1.3, 0);

            Size infusionSize = new Size(personSize / 8, personSize / 4);

            infusion.Children.Add(new RectangleGeometry(new Rect(infusionPosition + new Vector(0 - infusionSize.Width / 2 , 2 * personSize / 3), infusionSize), 2 * monitorLineSize, 2 * monitorLineSize));

            infusion.Children.Add(new RectangleGeometry(new Rect(infusionPosition + new Vector( -monitorLineSize / 2, monitorLineSize), new Size(monitorLineSize, 2 * personSize / 3 - monitorLineSize)), monitorLineSize, monitorLineSize));
            infusion.Children.Add(new RectangleGeometry(new Rect(infusionPosition + new Vector(-monitorSize.Width / 2 + monitorLineSize, 0), new Size(monitorSize.Width - 2 * monitorLineSize, monitorLineSize)), monitorLineSize, monitorLineSize));

            infusion.Children.Add(new LineGeometry(infusionPosition + new Vector(- monitorLineSize, 2 * personSize / 3), infusionPosition + new Vector(- 2 * infusionSize.Width, personSize / 3)));

            geometries.Children.Add(infusion);

            _drawingShape.Data = geometries;

            DrawingShape.Fill = new SolidColorBrush(color);
            DrawingShape.Stroke = new SolidColorBrush(color);
        } // end of Constructor

        #endregion


    } // end of DrawTreatmentFacility
}

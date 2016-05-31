using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using WPFVisualizationBase;

namespace SimulationWPFVisualizationTools.HealthCareObjects
{
    /// <summary>
    /// Basic example to visualize a waiting facility, draws a rectangle and visualizes
    /// persons in a grid manner within it. When full persons are all shifted to the middle
    /// and a string count of the number of persons in the facility is shown
    /// </summary>
    public class DrawDynamicHoldingEntity : DrawingObjectForEntity
    {
         #region Constructor

        /// <summary>
        /// Basic constructor, geometry outline (a rectangle) and caption is created
        /// </summary>
        /// <param name="correspondingEntity">Corresponding waiting facility</param>
        /// <param name="startPosition">Poistion of waiting facility</param>
        /// <param name="size">Size of waiting facility</param>
        /// <param name="personSize">Visualization height of persons</param>
        /// <param name="color">Color in which facility is displayed</param>
        public DrawDynamicHoldingEntity(Entity correspondingEntity, 
                                     Point startPosition,
                                     Size size,
                                     double personSize,
                                     Color color)
            : base(startPosition, correspondingEntity)
        {
            _size = size;
            _personSize = personSize;

            double lineSize = personSize / 15;

            Point origin = new Point(0, 0);

            GeometryGroup geometries = new GeometryGroup();

            // outline
            geometries.Children.Add(Geometry.Combine(new RectangleGeometry(new Rect(origin, size), lineSize, lineSize),
                                                     new RectangleGeometry(new Rect(origin + new Vector(lineSize, lineSize), new Size(size.Width - 2 * lineSize, size.Height - 2 * lineSize)), lineSize, lineSize),
                                                     GeometryCombineMode.Exclude,
                                                     Transform.Identity));

            DrawingShape.Data = geometries;
            DrawingShape.Fill = new SolidColorBrush(color);
            DrawingShape.Stroke = new SolidColorBrush(color);

            _slotWidth = PersonSize * 0.5;
            _slotHeight = PersonSize * 1.2;

            _numberPersonHorizontal = (int)(DrawingShape.Data.Bounds.Width / SlotWidth);
            _numberPersonVertical = (int)(DrawingShape.Data.Bounds.Height / SlotHeight);

            _drawPerdsonCount = new DrawingObjectString(startPosition + new Vector(DrawingShape.Data.Bounds.Width / 2 + (PersonSize * 5) / 6, DrawingShape.Data.Bounds.Height / 2 + (PersonSize * 5) / 6),
                                                                                   "0",
                                                                                   CustomStringAlignment.Center,
                                                                                   (int)(PersonSize * 5)/6,
                                                                                   Colors.Gray);

        } // end of Constructor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Size

        private Size _size;

        /// <summary>
        /// Size of waiting facility
        /// </summary>
        public Size Size
        {
            get
            {
                return _size;
            }
        } // end of Size

        #endregion

        #region PersoneSize

        private double _personSize;

        /// <summary>
        /// Visualization height of persons
        /// </summary>
        public double PersonSize
        {
            get
            {
                return _personSize;
            }
        } // end of PersoneSize

        #endregion

        #region NumberPersonHorizontal

        private int _numberPersonHorizontal;

        /// <summary>
        /// Number of people that can be visualized horizontally in the grid, automatically determined
        /// by the person visualization width and the size of the facility
        /// </summary>
        public int NumberPersonHorizontal
        {
            get
            {
                return _numberPersonHorizontal;
            }
        } // end of NumberPersonHorizontal

        #endregion

        #region NumberPersonVertical

        private int _numberPersonVertical;

        /// <summary>
        /// Number of people that can be visualized vertically in the grid, automatically determined
        /// by the person visualization height and the size of the facility
        /// </summary>
        public int NumberPersonVertical
        {
            get
            {
                return _numberPersonVertical;
            }
        } // end of NumberPersonVertical

        #endregion

        #region MaxNumberPerson

        /// <summary>
        /// Total number of slots in the facility to visualize people
        /// </summary>
        public int MaxNumberPerson
        {
            get
            {
                return NumberPersonHorizontal * NumberPersonVertical;
            }
        } // end of MaxNumberPerson

        #endregion

        #region SlotWidth

        private double _slotWidth;

        /// <summary>
        /// Width of a slot in the grid reserved for a person
        /// </summary>
        public double SlotWidth
        {
            get
            {
                return _slotWidth;
            }
        } // end of SlotWidth

        #endregion

        #region SlotHeight

        private double _slotHeight;

        /// <summary>
        /// Height of a slot in the grid reserved for a person
        /// </summary>
        public double SlotHeight
        {
            get
            {
                return _slotHeight;
            }
        } // end of SlotHeight

        #endregion

        #region DrawPersonCount

        private DrawingObjectString _drawPerdsonCount;

        /// <summary>
        /// String drawing object used when the number of people exceeds MaxNumberPerson
        /// </summary>
        public DrawingObjectString DrawPersonCount
        {
            get
            {
                return _drawPerdsonCount;
            }
        } // end of DrawPersonCount

        #endregion

        #region SetPosition

        public override void SetPosition(Point newPosition)
        {
            DrawPersonCount.SetPosition(newPosition + new Vector(DrawingShape.Data.Bounds.Width / 2 + (PersonSize * 5) / 6, PersonSize / 6));

            base.SetPosition(newPosition);
        } // end of SetPosition

        #endregion

    } // end of DrawDynamicHoldingEntity
}

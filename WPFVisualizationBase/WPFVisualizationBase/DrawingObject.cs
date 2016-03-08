using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFVisualizationBase
{
    public enum CustomStringAlignment
    {
        Left,
        Right,
        Center
    } // end of CustomStringAlignment

    /// <summary>
    /// A base drawing object that can be added to and moved on DrawingOnCoordinateSystem
    /// Actual drawing shapes are implemented as a Path drawing object, the drawing should be
    /// done in the constructor using a local coordinate system with (0,0) origin. Do not use
    /// the start position as a drawing reference point. Drawings will be automatically shifted
    /// to that position
    /// </summary>
    abstract public class DrawingObject
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, adds mouse events for entering the object and leaving it, sets caption parameters
        /// and initializes the drawing shape
        /// </summary>
        /// <param name="startPosition"></param>
        public DrawingObject(Point startPosition)
        {
            _currentPosition = startPosition;
            _currentAngle = 0;
            _drawingShape = new Path();

            UpdateRendering();

            DrawingShape.MouseEnter += DrawingShape_MouseEnter;
            DrawingShape.MouseLeave += DrawingShape_MouseLeave;

            _captionSize = 12;
            _captionTypeFace = new Typeface("Arial");

        } // end of DrawingObject

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region ObjectCaption

        private TextBox _objectCaption;

        /// <summary>
        /// Possible caption of the drawign object
        /// </summary>
        public TextBox ObjectCaption
        {
            get
            {
                return _objectCaption;
            }
        } // end of ObjectCaption

        #endregion

        #region TextColor

        private Color _textColor;

        /// <summary>
        /// Text color for object caption
        /// </summary>
        public Color TextColor
        {
            get
            {
                return _textColor;
            }
        } // end of TextColor

        #endregion

        #region CurrentPosition

        private Point _currentPosition;

        /// <summary>
        /// Position of object on DrawingOnCoordinateSystem
        /// </summary>
        public Point CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
        } // end of CurrentPosition

        #endregion

        #region CurrentAngle

        private double _currentAngle;

        /// <summary>
        /// Current rotation angle of object on DrawingOnCoordinateSystem
        /// </summary>
        public double CurrentAngle
        {
            get
            {
                return _currentAngle;
            }
        } // end of CurrentAngle

        #endregion

        #region DrawingShape

        protected Path _drawingShape;

        /// <summary>
        /// The actual shape to be visualized
        /// </summary>
        public Path DrawingShape
        {
            get
            {
                return _drawingShape;
            }
        } // end of DrawingShape

        #endregion

        #region ParentCoordinateSystem

        private DrawingOnCoordinateSystem _parentCoordinateSystem;

        /// <summary>
        /// The drawing system the object is visualized on
        /// </summary>
        public DrawingOnCoordinateSystem ParentCoordinateSystem
        {
            get
            {
                return _parentCoordinateSystem;
            }
            set
            {
                _parentCoordinateSystem = value;
            }
        } // end of ParentCoordinateSystem

        #endregion

        #region CaptionTypeFace

        private Typeface _captionTypeFace;

        /// <summary>
        /// Typeface for caption of object
        /// </summary>
        public Typeface CaptionTypeFace
        {
            get
            {
                return _captionTypeFace;
            }
            set
            {
                _captionTypeFace = value;
            }
        } // end of CaptionTypeFace

        #endregion

        #region CaptionSize

        private int _captionSize;

        /// <summary>
        /// Size of possible caption
        /// </summary>
        public int CaptionSize
        {
            get
            {
                return _captionSize;
            }
            set
            {
                _captionSize = value;
            }
        } // end of CaptionSize

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region SetCaption

        /// <summary>
        /// Autmatically sets a caption above the drawing object
        /// </summary>
        /// <param name="caption">Caption to visualize</param>
        /// <param name="stringAlign">Alignment of caption with respect to drawing object</param>
        public void SetCaption(string caption, CustomStringAlignment stringAlign = CustomStringAlignment.Center)
        {
            GeometryGroup geometryGroup = (GeometryGroup)DrawingShape.Data;

            Point stringRefPoint = new Point(0, 0);
            double geometryWidth = 0;

            FormattedText ft = new FormattedText(
            caption,
            Thread.CurrentThread.CurrentCulture,
            System.Windows.FlowDirection.LeftToRight,
            CaptionTypeFace, CaptionSize, new SolidColorBrush(TextColor));

            Geometry stringGeometry = ft.BuildGeometry(new Point());

            if (geometryGroup.Children.Count > 0)
            {
                stringRefPoint = geometryGroup.Bounds.BottomLeft + new Vector(0, stringGeometry.Bounds.Height + CaptionSize);
                geometryWidth = geometryGroup.Bounds.Width;
            } // end if

            switch (stringAlign)
            {
                case CustomStringAlignment.Left:
                    break;
                case CustomStringAlignment.Right:
                    stringRefPoint.X += geometryWidth - stringGeometry.Bounds.Width;
                    break;
                case CustomStringAlignment.Center:
                    stringRefPoint.X += geometryWidth / 2 - stringGeometry.Bounds.Width / 2;
                    break;
                default:
                    break;
            }

            stringGeometry.Transform = new MatrixTransform(1, 0, 0, -1, stringRefPoint.X, stringRefPoint.Y);

            geometryGroup.Children.Add(stringGeometry);

            _drawingShape.Data = geometryGroup;

        } // end of SetCaption

        #endregion

        #region SetPosition

        /// <summary>
        /// Sets the object to a new position on DrawingOnCoordinateSystem, here
        /// the standard coordinate system can be used
        /// </summary>
        /// <param name="newPosition">New position object should be visualized</param>
        public void SetPosition(Point newPosition)
        {
            _currentPosition = newPosition;

            UpdateRendering();

        } // end of SetPosition

        #endregion

        #region ShiftObject

        /// <summary>
        /// Shifts an object to a new position on DrawingOnCoordinateSystem, here
        /// the standard coordinate system can be used
        /// </summary>
        /// <param name="shiftVector">Shift vector for object</param>
        public void ShiftObject(Vector shiftVector)
        {
            _currentPosition += shiftVector;

            UpdateRendering();
        } // end of ShiftObject

        #endregion

        #region SetRotationAngle

        /// <summary>
        /// Sets the rotation angle of object on DrawingOnCoordinateSystem, rotation is done with center of the current position
        /// </summary>
        /// <param name="angle"></param>
        public void SetRotationAngle(double angle)
        {
            _currentAngle = angle;

            UpdateRendering();
        } // end of SetRotationAngle

        #endregion

        #region UpdateRendering

        public void UpdateRendering()
        {
            TransformGroup transformGroup = new TransformGroup();

            transformGroup.Children.Add(new RotateTransform(CurrentAngle));
            transformGroup.Children.Add(new TranslateTransform(CurrentPosition.X, CurrentPosition.Y));

            if (ParentCoordinateSystem != null)
                transformGroup.Children.Add(ParentCoordinateSystem.CurrentTransform);

            DrawingShape.RenderTransform = transformGroup;
        } // end of UpdateRendering

        #endregion

        //--------------------------------------------------------------------------------------------------
        // EventHandling 
        //--------------------------------------------------------------------------------------------------

        #region DrawingShape_MouseLeave

        /// <summary>
        /// Can be overwritten to change visualization upong mouse enter
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DrawingShape_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {

        } // end of DrawingShape_MouseLeave


        #endregion

        #region DrawingShape_MouseEnter

        /// <summary>
        /// Can be overwritten to change visualization upong mouse leave
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void DrawingShape_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {

        } // end of DrawingShape_MouseEnter

        #endregion

    } // end of DrawingObject
}

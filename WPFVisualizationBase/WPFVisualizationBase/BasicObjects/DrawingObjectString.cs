using System.Threading;
using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase
{
    /// <summary>
    /// Basic drawing object to draw strings on a DrawingOnCoordinateSystem
    /// </summary>
    public class DrawingObjectString : DrawingObject
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basuc constructor
        /// </summary>
        /// <param name="startPosition">Start position of string</param>
        /// <param name="stringToDraw">The string to visualize</param>
        /// <param name="alignment">Sets the alignment with respect to the position passed</param>
        /// <param name="fontSize">Size of string</param>
        /// <param name="color">Color to display</param>
        /// <param name="typeFaceName">Typface used for string</param>
        public DrawingObjectString(Point startPosition,
            string stringToDraw,
            CustomStringAlignment alignment,
            int fontSize,
            Color color,
            string typeFaceName = "Arial") : base(startPosition)
        {
            _stringToDraw = stringToDraw;

            _typeFaceName = typeFaceName;
            _fontSize = fontSize;
            _alignment = alignment;
            _stringColor = color;

            DrawingShape.Stroke = new SolidColorBrush(color);
            DrawingShape.Fill = new SolidColorBrush(color);

            SetStringGeometry();
        } // end of DrawingObject

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StringColor

        private Color _stringColor;

        /// <summary>
        /// Color to display
        /// </summary>
        public Color StringColor
        {
            get
            {
                return _stringColor;
            }
        } // end of StringColor

        #endregion StringColor

        #region StringToDraw

        private string _stringToDraw;

        /// <summary>
        /// The string to visualize
        /// </summary>
        public string StringToDraw
        {
            get
            {
                return _stringToDraw;
            }
            set
            {
                _stringToDraw = value;
                SetStringGeometry();
            }
        } // end of StringToDraw

        #endregion StringToDraw

        #region TypeFaceName

        private string _typeFaceName;

        /// <summary>
        /// Typface used for string
        /// </summary>
        public string TypeFaceName
        {
            get
            {
                return _typeFaceName;
            }
            set
            {
                _typeFaceName = value;
                SetStringGeometry();
            }
        } // end of TypeFaceName

        #endregion TypeFaceName

        #region FontSize

        private int _fontSize;

        /// <summary>
        /// Size of string
        /// </summary>
        public int FontSize
        {
            get
            {
                return _fontSize;
            }
            set
            {
                _fontSize = value;
                SetStringGeometry();
            }
        } // end of FontSize

        #endregion FontSize

        #region StringAlignment

        private CustomStringAlignment _alignment;

        /// <summary>
        /// Sets the alignment with respect to the position passed
        /// </summary>
        public CustomStringAlignment StringAlignment
        {
            get
            {
                return _alignment;
            }
            set
            {
                _alignment = value;
                SetStringGeometry();
            }
        } // end of StringAlignment

        #endregion StringAlignment

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region SetStringGeometry

        /// <summary>
        /// Transforms the string in a string geometry and applies transforms with respect to position and alignment
        /// </summary>
        protected void SetStringGeometry()
        {
            FormattedText ft = new FormattedText(
                StringToDraw,
                Thread.CurrentThread.CurrentCulture,
                System.Windows.FlowDirection.LeftToRight,
                new Typeface(TypeFaceName), FontSize, new SolidColorBrush(StringColor));

            Geometry stringGeometry = ft.BuildGeometry(new Point(0, 0));

            switch (StringAlignment)
            {
                case CustomStringAlignment.Left:
                    stringGeometry.Transform = new MatrixTransform(1, 0, 0, -1, 0, stringGeometry.Bounds.Height);
                    break;

                case CustomStringAlignment.Right:
                    stringGeometry.Transform = new MatrixTransform(1, 0, 0, -1, -stringGeometry.Bounds.Width, stringGeometry.Bounds.Height);
                    break;

                case CustomStringAlignment.Center:
                    stringGeometry.Transform = new MatrixTransform(1, 0, 0, -1, -stringGeometry.Bounds.Width / 2, stringGeometry.Bounds.Height);
                    break;

                default:
                    break;
            }

            DrawingShape.Data = stringGeometry;

            UpdateRendering();
        } // end of SetStringGeometry

        #endregion SetStringGeometry
    } // end of DrawingObjectString
}
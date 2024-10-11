using System;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace WPFVisualizationBase
{
    /// <summary>
    /// A special drawing system for visualizing the time on an analog clock
    /// </summary>
    public class DrawAnalogClockWithDate : DrawingOnCoordinateSystem
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="clockColor">Color inwhich clock is displayed</param>
        /// <param name="initaliShiftX">Shift X, to adjust to size of drawing system</param>
        /// <param name="initialShiftY">Shift Y, to adjust to size of drawing system</param>
        public DrawAnalogClockWithDate(Color clockColor, double initaliShiftX, double initialShiftY)
            : base(initaliShiftX, initialShiftY, true)
        {
            _clockBrush = new SolidColorBrush(clockColor);
            ClipToBounds = true;
        } // end of Constructor

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Intializes the clock to start time
        /// </summary>
        /// <param name="startTime">First time to display</param>
        public void Initialize(DateTime startTime = default(DateTime))
        {
            _clockCenter = new Point(ActualWidth / 2, -ActualHeight / 2 * 1.3);

            _clockRadius = ActualWidth / 2 * 0.6;

            // create clock outline
            AnalogClockOutlineDrawingObject clockOutline = new AnalogClockOutlineDrawingObject(ClockCenter, ClockRadius, Colors.LightGray);

            // create pointer
            _hourPointer = new DrawObjectLine(ClockCenter, new Vector(1, 0) * 0.6 * ClockRadius, Colors.LightGray);
            _minutePointer = new DrawObjectLine(ClockCenter, new Vector(1, 0) * 0.8 * ClockRadius, Colors.LightGray);

            // create date string
            _dateString = new DrawingObjectString(ClockCenter + new Vector(0, 1) * 1.2 * ClockRadius, GetTwoLineDateAndWeekdayString(startTime), CustomStringAlignment.Center, 12, Colors.LightGray);

            // add all objects to drawing system
            AddObject(clockOutline);
            AddObject(HourPointer);
            AddObject(MinutePointer);
            AddObject(DateString);

            SetDateTime(startTime);
        } // end of Initialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region ClockCenter

        private Point _clockCenter;

        /// <summary>
        /// Center of clock
        /// </summary>
        public Point ClockCenter
        {
            get
            {
                return _clockCenter;
            }
        } // end of ClockCenter

        #endregion ClockCenter

        #region ClockRadius

        private double _clockRadius;

        /// <summary>
        /// Size of clock
        /// </summary>
        public double ClockRadius
        {
            get
            {
                return _clockRadius;
            }
        } // end of ClockRadius

        #endregion ClockRadius

        #region ClockBrush

        private Brush _clockBrush;

        /// <summary>
        /// Brush used to visualize clock
        /// </summary>
        public Brush ClockBrush
        {
            get
            {
                return _clockBrush;
            }
        } // end of ClockBrush

        #endregion ClockBrush

        #region HourPointer

        private DrawObjectLine _hourPointer;

        /// <summary>
        /// Pointer to indicate current hour
        /// </summary>
        protected DrawObjectLine HourPointer
        {
            get
            {
                return _hourPointer;
            }
        } // end of HourPointer

        #endregion HourPointer

        #region MinutePointer

        private DrawObjectLine _minutePointer;

        /// <summary>
        /// Pointer to indicate current minute
        /// </summary>
        protected DrawObjectLine MinutePointer
        {
            get
            {
                return _minutePointer;
            }
        } // end of MinutePointer

        #endregion MinutePointer

        #region DateString

        private DrawingObjectString _dateString;

        /// <summary>
        /// String drawing object for current date
        /// </summary>
        public DrawingObjectString DateString
        {
            get
            {
                return _dateString;
            }
        } // end of DateString

        #endregion DateString

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region SetDateTime

        /// <summary>
        /// Sets the time that should be displayed on clock
        /// </summary>
        /// <param name="dateTime">Time to display</param>
        public void SetDateTime(DateTime dateTime)
        {
            float hourAngle = -(dateTime.Hour % 12 + (dateTime.Minute % 60) / 60f) * 360 / 12 + 90;
            float minAngle = -dateTime.Minute * 360 / 60 + 90;

            HourPointer.SetRotationAngle(hourAngle);
            MinutePointer.SetRotationAngle(minAngle);

            DateString.StringToDraw = GetTwoLineDateAndWeekdayString(dateTime);
        } // end of SetDateTime

        #endregion SetDateTime

        #region GetTwoLineDateAndWeekdayString

        /// <summary>
        /// Helping method to create a date string
        /// </summary>
        /// <param name="time">Time from which to extract date</param>
        /// <returns>string representing current date</returns>
        public string GetTwoLineDateAndWeekdayString(DateTime time)
        {
            string amPmString;

            if (time.Hour < 12)
                amPmString = "am";
            else
                amPmString = "pm";

            StringBuilder dateString = new StringBuilder();

            dateString.AppendLine(time.ToShortDateString() + " " + amPmString);
            dateString.AppendLine(time.DayOfWeek.ToString());

            return dateString.ToString();
        } // end of GetTwoLineDateAndWeekdayString

        #endregion GetTwoLineDateAndWeekdayString
    } // end of DrawAnalogClockWithDate
}
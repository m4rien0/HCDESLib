using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace SimulationWPFVisualizationTools.HealthCareObjects
{
    public enum PatientPositionInRoomType
    {
        UpRight,
        LyingInBed,
        FlatLying
    }

    /// <summary>
    /// Basic drawing object for a treatment facility, provides general elements common to
    /// all treatment facility objects used in this example
    /// </summary>
    public class DrawBaseTreatmentFacility : DrawingObjectForEntity
    {
        #region Constructor

        /// <summary>
        /// Basic constructor, sets sizes and colors used for drawing
        /// </summary>
        /// <param name="correspondingEntity">Treatment facility the drawing object represents</param>
        /// <param name="startPosition">Position where the treatment facility should be drawn</param>
        /// <param name="size">Size of treatment facility</param>
        /// <param name="personSize">Height used to visualize persons</param>
        /// <param name="color">Color in which the facility should be displayed</param>
        public DrawBaseTreatmentFacility(Entity correspondingEntity, 
                                     Point startPosition,
                                     Size size,
                                     double personSize,
                                     Color color)
            : base(startPosition, correspondingEntity)
        {
            _size = size;
            _personSize = personSize;
            double lineSize = personSize / 15;

            DrawingShape.Fill = new SolidColorBrush(color);
            DrawingShape.Stroke = new SolidColorBrush(color);
        } // end of Constructor

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Size

        private Size _size;

        /// <summary>
        /// Size of treatment facility
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
        /// Height used to visualize persons
        /// </summary>
        public double PersoneSize
        {
            get
            {
                return _personSize;
            }
        } // end of PersoneSize

        #endregion

        #region StaffStartPosition

        protected Point _staffStartPosition;

        /// <summary>
        /// Position in treatment facility where the first staff should be visualized
        /// </summary>
        public Point StaffStartPosition
        {
            get
            {
                return _staffStartPosition;
            }
        } // end of StaffStartPosition

        #endregion

        #region PatientPositionType

        protected PatientPositionInRoomType _patientPositionType;

        /// <summary>
        /// Patient position type in which he/she should be visualized
        /// </summary>
        public PatientPositionInRoomType PatientPositionType
        {
            get
            {
                return _patientPositionType;
            }
        } // end of PatientPositionType

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region GetStaffPosition

        /// <summary>
        /// Poistion to visualize the n-th staff member, uses the position of the facility, the staff starting position
        /// and displays staff members along a (1,0) vector from there
        /// </summary>
        /// <param name="staffIndex">If staff members are seen in a vector this is the position in the vector, i.e. the i-th staff member</param>
        /// <returns></returns>
        public Point GetStaffPosition(int staffIndex)
        {
            return CurrentPosition + new Vector(PersoneSize * (staffIndex) * 0.7 + StaffStartPosition.X, StaffStartPosition.Y);
        } // end of GetStaffPosition

        #endregion

        #region PatientInRoomPosition

        protected Point _patientInRoomPosition;

        public Point PatientInRoomPosition
        {
            get
            {
                return new Point(CurrentPosition.X + _patientInRoomPosition.X, CurrentPosition.Y + _patientInRoomPosition.Y);
            }
        } // end of PatientInBedPosition

        #endregion

    } // end of DrawBaseTreatmentFacility
}

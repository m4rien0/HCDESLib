using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.MathTool.GeometricClasses
{
    public class MyPoint
    {
        #region Constructor

        public MyPoint()
        {
            _x = 0;
            _y = 0;
            _drawingPoint = new PointF(0, 0);
        } // end of Point

        public MyPoint(float x, float y, float z = 0)
        {
            _x = x;
            _y = y;
            _z = z;
            _drawingPoint = new PointF(x, y);
        } // end of Point

        public MyPoint(double x, double y, double z = 0)
        {
            _x = (float)x;
            _y = (float)y;
            _z = (float)z;
            _drawingPoint = new PointF((float)x, (float)y);
        } // end of Point

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region X

        private float _x;

        public float X
        {
            get
            {
                return _x;
            }
            set
            {
                _drawingPoint.X = value;
                _x = value;
            }
        } // end of X

        #endregion

        #region Y

        private float _y;

        public float Y
        {
            get
            {
                return _y;
            }
            set
            {
                _drawingPoint.Y = value;
                _y = value;
            }
        } // end of Y

        #endregion

        #region Z

        private float _z;

        public float Z
        {
            get
            {
                return _z;
            }
            set
            {
                _z = value;
            }
        } // end of Z

        #endregion

        #region DrawingPoint

        private PointF _drawingPoint;

        public PointF DrawingPoint
        {
            get
            {
                return _drawingPoint;
            }
        } // end of DrawingPoint

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region Operator +

        public static MyPoint operator +(MyPoint p1, MyPoint p2)
        {
            return new MyPoint(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);

        } // end of operator /

        #endregion

        #region Operator -

        public static MyPoint operator -(MyPoint p1, MyPoint p2)
        {
            return new MyPoint(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);

        } // end of operator /

        #endregion

        #region operator *

        public static MyPoint operator *(MyPoint v1, float d)
        {
            return new MyPoint(v1.X * d, v1.Y * d, v1.Z *d);

        } // end of operator *

        #endregion

        #region ToString

        public override string ToString()
        {
            return String.Format("({0},{1})", X, Y);
        } // end of ToString

        #endregion

        #region Clone

        public MyPoint Clone()
        {
            return new MyPoint(X, Y);
        } // end of Clone

        #endregion

    } // end of Point
}

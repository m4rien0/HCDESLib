using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.MathTool.GeometricClasses
{
    public class Vector : MyPoint
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        public Vector(MyPoint p)
            : base(p.X, p.Y, p.Z)
        {
            float x = p.X;
            float y = p.Y;
            float z = p.Z;

            _length = (float)Math.Pow(x * x + y * y + z * z, 0.5);

            if (_length > 0)
            {
                _normalizedX = x / _length;
                _normalizedY = y / _length;
                _normalizedZ = z / _length;
                _normalizedVector = new MyPoint(_normalizedX, _normalizedY, _normalizedZ);
            } // end if
        } // end of

        public Vector(float x, float y, float z = 0)
            : base(x, y)
        {
            _length = (float)Math.Pow(x * x + y * y + z * z, 0.5);

            if (_length > 0)
            {
                _normalizedX = x / _length;
                _normalizedY = y / _length;
                _normalizedZ = z / _length;
                _normalizedVector = new MyPoint(_normalizedX, _normalizedY, _normalizedZ);
            } // end if

        } // end of Vector

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region NormalizedX

        private float _normalizedX;

        public float NormalizedX
        {
            get
            {
                return _normalizedX;
            }
        } // end of NormalizedX

        #endregion

        #region NormalizedY

        private float _normalizedY;

        public float NormalizedY
        {
            get
            {
                return _normalizedY;
            }
        } // end of NormalizedY

        #endregion

        #region NormalizedZ

        private float _normalizedZ;

        public float NormalizedZ
        {
            get
            {
                return _normalizedZ;
            }
        } // end of NormalizedZ

        #endregion

        #region Length

        private float _length;

        public float Length
        {
            get
            {
                return _length;
            }
        } // end of Length

        #endregion

        #region NormalizedVector

        private MyPoint _normalizedVector;

        public MyPoint NormalizedVector
        {
            get
            {
                return _normalizedVector;
            }
            set
            {
                _normalizedVector = value;
            }
        } // end of NormalizedVector

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetAntiClockWiseTransposedNormalizedVector

        public Vector GetAntiClockWiseTransposedNormalizedVector()
        {
            return new Vector(NormalizedY, -NormalizedX);
        } // end of GetAntiClockWiseTransposedNormalizedVector

        #endregion

        #region GetClockWiseTransposedNormalizedVector

        public Vector GetClockWiseTransposedNormalizedVector()
        {
            return new Vector(-NormalizedY, NormalizedX);
        } // end of GetAntiClockWiseTransposedNormalizedVector

        #endregion


        //--------------------------------------------------------------------------------------------------
        // Operators
        //--------------------------------------------------------------------------------------------------

        #region Operator *

        public static Vector operator *(Vector v1, float d)
        {
            return new Vector(v1.X * d, v1.Y * d, v1.Z * d);

        } // end of operator *

        #endregion

        #region Operator /

        public static Vector operator /(Vector v1, float d)
        {
            if (d == 0)
                return null;

            return new Vector(v1.X / d, v1.Y / d, v1.Z / d);

        } // end of operator /

        #endregion

    } // end of Vector
}

using System;

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

        #endregion Constructor

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

        #endregion NormalizedX

        #region NormalizedY

        private float _normalizedY;

        public float NormalizedY
        {
            get
            {
                return _normalizedY;
            }
        } // end of NormalizedY

        #endregion NormalizedY

        #region NormalizedZ

        private float _normalizedZ;

        public float NormalizedZ
        {
            get
            {
                return _normalizedZ;
            }
        } // end of NormalizedZ

        #endregion NormalizedZ

        #region Length

        private float _length;

        public float Length
        {
            get
            {
                return _length;
            }
        } // end of Length

        #endregion Length

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

        #endregion NormalizedVector

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetAntiClockWiseTransposedNormalizedVector

        public Vector GetAntiClockWiseTransposedNormalizedVector()
        {
            return new Vector(NormalizedY, -NormalizedX);
        } // end of GetAntiClockWiseTransposedNormalizedVector

        #endregion GetAntiClockWiseTransposedNormalizedVector

        #region GetClockWiseTransposedNormalizedVector

        public Vector GetClockWiseTransposedNormalizedVector()
        {
            return new Vector(-NormalizedY, NormalizedX);
        } // end of GetAntiClockWiseTransposedNormalizedVector

        #endregion GetClockWiseTransposedNormalizedVector

        //--------------------------------------------------------------------------------------------------
        // Operators
        //--------------------------------------------------------------------------------------------------

        #region Operator *

        public static Vector operator *(Vector v1, float d)
        {
            return new Vector(v1.X * d, v1.Y * d, v1.Z * d);
        } // end of operator *

        #endregion Operator *

        #region Operator /

        public static Vector operator /(Vector v1, float d)
        {
            if (d == 0)
                return null;

            return new Vector(v1.X / d, v1.Y / d, v1.Z / d);
        } // end of operator /

        #endregion Operator /
    } // end of Vector
}
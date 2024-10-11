using System;
using System.Drawing;
using System.Linq;

namespace SimulationCore.MathTool.GeometricClasses
{
    public class PolygonialLine
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        public PolygonialLine(MyPoint[] points)
        {
            if (points == null)
                throw new ArgumentException();

            _points = points;

            _vectors = new Vector[points.Length - 1];

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vectors[i] = new Vector(points[i + 1].X - points[i].X, points[i + 1].Y - points[i].Y, points[i + 1].Z - points[i].Z);
            } // end for

            _length = 0;
            _aggregateLengths = new float[_points.Length];
            _aggregateLengths[0] = 0;

            for (int i = 0; i < Vectors.Length; i++)
            {
                _length += Vectors[i].Length;
                _aggregateLengths[i + 1] = _aggregateLengths[i] + Vectors[i].Length;
            } // end for
        } // end of PolygonalLine

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Points

        private MyPoint[] _points;

        public MyPoint[] Points
        {
            get
            {
                return _points;
            }
            set
            {
                _points = value;
            }
        } // end of Points

        #endregion Points

        #region Vectors

        private Vector[] _vectors;

        public Vector[] Vectors
        {
            get
            {
                return _vectors;
            }
        } // end of Vectors

        #endregion Vectors

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

        #region AggregateLenghts

        private float[] _aggregateLengths;

        #endregion AggregateLenghts

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetAbsolutePositionOnLine

        //--------------------------------------------------------------------------------------------------
        // Given a length passed on path this method returns the actual position in coordinates
        //--------------------------------------------------------------------------------------------------
        public MyPoint GetAbsolutePositionOnLine(float positionInLength)
        {
            //--------------------------------------------------------------------------------------------------
            // Checking arbitrary cases
            //--------------------------------------------------------------------------------------------------

            if (positionInLength <= 0)
                return Points.First();

            if (positionInLength > Length)
                return Points.Last();

            //--------------------------------------------------------------------------------------------------
            // Find the corresponding segment in which position is nested
            //--------------------------------------------------------------------------------------------------
            int middleIndex = (_aggregateLengths.Length - 1) / 2;
            int upperIndex = _aggregateLengths.Length - 1;
            int lowerIndex = 0;

            while (upperIndex - lowerIndex > 1)
            {
                if (_aggregateLengths[middleIndex] == positionInLength)
                {
                    return _points[middleIndex];
                }
                else if (_aggregateLengths[middleIndex] < positionInLength)
                {
                    lowerIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                }
                else
                {
                    upperIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                } // end if
            } // end while

            //--------------------------------------------------------------------------------------------------
            // return the absolute position
            //--------------------------------------------------------------------------------------------------

            return _points[lowerIndex] + _vectors[lowerIndex].NormalizedVector * (positionInLength - _aggregateLengths[lowerIndex]);
        } // end of GetAbsolutePositionOnLine

        #endregion GetAbsolutePositionOnLine

        #region GetDirectionOfPointOnPath

        //--------------------------------------------------------------------------------------------------
        // Given a length passed on path this method returns the direction position as a vector
        //--------------------------------------------------------------------------------------------------
        public Vector GetDirectionOfPointOnPath(float positionInLength)
        {
            //--------------------------------------------------------------------------------------------------
            // Checking arbitrary cases
            //--------------------------------------------------------------------------------------------------

            if (positionInLength <= 0)
                return Vectors.First();

            if (positionInLength >= Length)
                return Vectors.Last();

            //--------------------------------------------------------------------------------------------------
            // Find the corresponding segment in which position is nested
            //--------------------------------------------------------------------------------------------------
            int middleIndex = (_aggregateLengths.Length - 1) / 2;
            int upperIndex = _aggregateLengths.Length - 1;
            int lowerIndex = 0;

            while (upperIndex - lowerIndex > 1)
            {
                if (_aggregateLengths[middleIndex] == positionInLength)
                {
                    return _vectors[middleIndex - 1];
                }
                else if (_aggregateLengths[middleIndex] < positionInLength)
                {
                    lowerIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                }
                else
                {
                    upperIndex = middleIndex;
                    middleIndex = (upperIndex - lowerIndex) / 2 + lowerIndex;
                } // end if
            } // end while

            //--------------------------------------------------------------------------------------------------
            // return the absolute position
            //--------------------------------------------------------------------------------------------------

            return _vectors[lowerIndex];
        } // end of GetAbsolutePositionOnLine

        #endregion GetDirectionOfPointOnPath

        #region DrawLine

        public void DrawLine(Graphics graphics, Pen pen)
        {
            for (int i = 0; i < Points.Length - 1; i++)
            {
                graphics.DrawLine(pen, Points[i].DrawingPoint, Points[i + 1].DrawingPoint);
            } // end for
        } // end of DrawLine

        #endregion DrawLine
    } // end of PolygonialLine
}
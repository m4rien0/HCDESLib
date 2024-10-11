using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.Helpers
{
    /// <summary>
    /// Class that contains helping methods often used in the simulation code
    /// </summary>
    /// <typeparam name="T">Type T may vary for different usage</typeparam>
    public static class Helpers<T>
    {
        #region GetEnumValues

        /// <summary>
        /// Returns a enumerable object of all enum values. T must be of enum type.
        /// </summary>
        /// <returns>Enumerable object of all enum-values</returns>
        public static IEnumerable<T> GetEnumValues()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        } // en of GetEnumValues

        #endregion GetEnumValues

        #region EmptyArray

        /// <summary>
        /// Retruns empty array of type T, no restrictions on Type T
        /// </summary>
        /// <returns>Empty T array</returns>
        public static T[] EmptyArray()
        {
            return new T[] { };
        } // end of EmptyArray

        #endregion EmptyArray

        #region EmptyList

        /// <summary>
        /// Gets empty list of type T
        /// </summary>
        /// <returns>Empty List</returns>
        public static List<T> EmptyList()
        {
            return new List<T>();
        } // end of EmptyArray

        #endregion EmptyList

        #region ToArray

        /// <summary>
        /// Builds an array of a single item
        /// </summary>
        /// <param name="item">item that should be contained in the array</param>
        /// <returns>Array with a single item</returns>
        public static T[] ToArray(T item)
        {
            return new T[] { item };
        } // end of ToArray

        #endregion ToArray

        #region ToMultipleArray

        /// <summary>
        /// Builds a n-array with identical items
        /// </summary>
        /// <param name="item">Item that the array consists of</param>
        /// <param name="number">Size of array</param>
        /// <returns>N-dimensional array consisting of identical items</returns>
        public static T[] ToMultipleArray(T item, int number)
        {
            List<T> elements = new List<T>();

            for (int i = 0; i < number; i++)
            {
                elements.Add(item);
            } // end for

            return elements.ToArray();
        } // end of ToArray

        #endregion ToMultipleArray

        #region ToList

        /// <summary>
        /// Builds a list with a single item in it
        /// </summary>
        /// <param name="item">Item for the list</param>
        /// <returns>List with single item</returns>
        public static List<T> ToList(T item)
        {
            List<T> l = new List<T>();
            l.Add(item);

            return l;
        } // end of ToArray

        #endregion ToList

        #region MultiplyTimeSpan

        /// <summary>
        /// Multiplier function for the time span class
        /// </summary>
        /// <param name="span">The original time span</param>
        /// <param name="factor">Factor by that time span is multiplied</param>
        /// <returns>Time span multiplied by factor</returns>
        public static TimeSpan MultiplyTimeSpan(TimeSpan span, double factor)
        {
            return TimeSpan.FromTicks((long)(span.Ticks * factor));
        } // end of MultiplyTimeSpan

        #endregion MultiplyTimeSpan

        #region GetNumericalPrecission

        /// <summary>
        /// Centralized numerical precission. Can be used to define precission for operations.
        /// </summary>
        /// <returns>Specified numerical precission</returns>
        public static double GetNumbericalPrecission()
        {
            return 0.0001;
        } // end of GetNumbericalPrecission

        #endregion GetNumericalPrecission

        #region EqualsWithNumericalPrecission

        /// <summary>
        /// Equal operator considering numerical precission
        /// </summary>
        /// <param name="a">Left hand side value</param>
        /// <param name="b">Right hand side value</param>
        /// <returns>Returns true if the absolute difference of left and right hand side values
        /// is smaller than the numerical precission</returns>
        public static bool EqualsWithNumericalPrecission(double a, double b)
        {
            return Math.Abs(a - b) <= GetNumbericalPrecission();
        } // end EqualsWithNumericalPrecission

        #endregion EqualsWithNumericalPrecission
    } // end of Helpers
}
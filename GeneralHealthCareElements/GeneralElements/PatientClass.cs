using System;

namespace GeneralHealthCareElements
{
    /// <summary>
    /// Class to define different patient classes over basic attributes, such as priority, arrival mode and a general category
    /// </summary>
    public class PatientClass : ICloneable
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="category">Category of patient, e.g. pediatric</param>
        /// <param name="priority">Priority of patient</param>
        /// <param name="arrivalMode">Arrival mode of patient, e.g. ambulance or walk-in</param>
        public PatientClass(string category, int priority, string arrivalMode = "WalkIn")
        {
            _category = category;
            _priority = priority;
            _arrivalMode = arrivalMode;
        } // end of PatientClass

        /// <summary>
        /// Default constructor, string values set to "General"
        /// </summary>
        public PatientClass()
        {
            _category = "General";
            _priority = 0;
            _arrivalMode = "General";
        } // end of PatientClass

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Category

        private string _category;

        /// <summary>
        /// Category of patient, e.g. pediatric
        /// </summary>
        public string Category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        } // end of Category

        #endregion Category

        #region Priority

        private int _priority;

        /// <summary>
        /// Priority of patient
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
            set
            {
                _priority = value;
            }
        } // end of Priority

        #endregion Priority

        #region ArrivalMode

        private string _arrivalMode;

        /// <summary>
        /// Arrival mode of patient, e.g. ambulance or walk-in
        /// </summary>
        public string ArrivalMode
        {
            get
            {
                return _arrivalMode;
            }
            set
            {
                _arrivalMode = value;
            }
        } // end of ArrivalMode

        #endregion ArrivalMode

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Equals

        /// <summary>
        /// Overrides equal method for patient category objects, performs equal calls on all attributes
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            PatientClass otherClass = obj as PatientClass;

            if (otherClass.ArrivalMode != ArrivalMode || otherClass.Priority != Priority || otherClass.ArrivalMode != ArrivalMode)
                return false;
            else
                return true;
        } // end of Eqals

        #endregion Equals

        #region GetHashCode

        /// <summary>
        /// For hashable objects, sums hash codes of attributes
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Priority.GetHashCode() + Category.GetHashCode() + ArrivalMode.GetHashCode();
        } // end of GetHashCode

        #endregion GetHashCode

        #region Clone

        public object Clone()
        {
            return new PatientClass(Category, Priority, ArrivalMode);
        } // end of Clone

        #endregion Clone
    }
} // end of PatientClass
using Enums;
using System;
using System.Text;

namespace GeneralHealthCareElements.TreatmentAdmissionTypes
{
    /// <summary>
    /// Defines the type of a health care action
    /// </summary>
    public abstract class TreatmentType
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="type">Type of department the action is defined for</param>
        /// <param name="identifier">String identifier of the treatment type</param>
        public TreatmentType(TreatmentTypeClass type, string identifier)
        {
            _type = type;
            _identifier = identifier;
        } // end of TreatmentType

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Type

        private TreatmentTypeClass _type;

        /// <summary>
        /// Type of department the action is defined for
        /// </summary>
        public TreatmentTypeClass Type
        {
            get
            {
                return _type;
            }
        } // end of Type

        #endregion Type

        #region Identifier

        private string _identifier;

        /// <summary>
        /// String identifier of the treatment type
        /// </summary>
        public string Identifier
        {
            get
            {
                return _identifier;
            }
        } // end of Identifier

        #endregion Identifier

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Equals

        /// <summary>
        /// Overrides equal method
        /// </summary>
        /// <param name="obj">Other treatment type</param>
        /// <returns>Returns true if both treatment types have same type, i.e. are for same type of departments and have same identifier</returns>
        public override bool Equals(Object obj)
        {
            TreatmentType otherType;
            try
            {
                otherType = (TreatmentType)obj;
            }
            catch (Exception e)
            {
                return false;
            } // end try

            return otherType.Type == Type && otherType.Identifier == Identifier;
        } // end of Equals

        #endregion Equals

        #region GetHashCode

        /// <summary>
        /// For use in hashable objects, translates type and identifier to int
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)Type + BitConverter.ToInt32(Encoding.ASCII.GetBytes(Identifier), 0);
        } // end of GetHashCode

        #endregion GetHashCode

        #region ToString

        public override string ToString()
        {
            return Identifier;
        } // end of ToString

        #endregion ToString
    } // end of TreatmentType
}
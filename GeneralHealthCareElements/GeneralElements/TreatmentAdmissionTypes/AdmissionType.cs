using Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.TreatmentAdmissionTypes
{
    /// <summary>
    /// Class to define the type of an admission
    /// </summary>
    public abstract class AdmissionType
    {
        #region Constructor

        /// <summary>
        /// Basic construtor
        /// </summary>
        /// <param name="type">Defines for which type of department model the admission is made</param>
        /// <param name="identifier">String representation of the admission type</param>
        public AdmissionType(AdmissionTypeClass type, string identifier)
        {
            _type = type;
            _identifier = identifier;
        } // end of AdmissionType

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region Type

        private AdmissionTypeClass _type;

        /// <summary>
        /// Defines for which type of department model the admission is made
        /// </summary>
        public AdmissionTypeClass Type
        {
            get
            {
                return _type;
            }
        } // end of Type

        #endregion

        #region Identifier

        private string _identifier;

        /// <summary>
        /// String representation of the admission type
        /// </summary>
        public string Identifier
        {
            get
            {
                return _identifier;
            }
        } // end of Identifier

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region Equals

        /// <summary>
        /// Overrides equal method
        /// </summary>
        /// <param name="obj">Other admission type</param>
        /// <returns>Returns true if both admissiontypes have same type, i.e. are for same type of departments and have same identifier</returns>
        public override bool Equals(Object obj)
        {
            AdmissionType otherType;
            try
            {
                otherType = (AdmissionType)obj;
            }
            catch (Exception e)
            {
                return false;
            } // end try

            return otherType.Type == Type && otherType.Identifier == Identifier;
        } // end of Equals

        #endregion

        #region GetHashCode

        /// <summary>
        /// For use in hashable objects, translates type and identifier to int
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (int)Type + BitConverter.ToInt32(Encoding.ASCII.GetBytes(Identifier), 0);
        } // end of GetHashCode

        #endregion

        #region ToString

        public override string ToString()
        {
            return Identifier;
        } // end of ToString

        #endregion

    } // end of AdmissionType
}

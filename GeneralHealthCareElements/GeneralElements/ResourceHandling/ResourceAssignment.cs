using Enums;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.ResourceHandling
{
    /// <summary>
    /// Class to define the assignment of a resource to an organizational unit upon initialization
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResourceAssignment<T>
    {
        #region Contructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="resource">The resource to be assigned</param>
        /// <param name="organizationalUnit">The organizational unit it is assigned to, per default the root department model</param>
        /// <param name="assignmentType">The type of assignement, fixed or shared</param>
        public ResourceAssignment(T resource,
            string organizationalUnit = null,
            AssignmentType assignmentType = AssignmentType.Fixed
            )
        {
            _resource = resource;
            _assignmentType = assignmentType;

            if (organizationalUnit == null)
                _organizationalUnit = "RootDepartment";
            else
                _organizationalUnit = organizationalUnit;
        } // end of ResourceOrganizationalUnitAssignment

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region Resource

        private T _resource;

        /// <summary>
        /// The resource to be assigned
        /// </summary>
        public T Resource
        {
            get
            {
                return _resource;
            }
            set
            {
                _resource = value;
            }
        } // end of Resource

        #endregion

        #region OrganizationalUnit

        private string _organizationalUnit;

        /// <summary>
        /// The organizational unit it is assigned to, per default the root department model
        /// </summary>
        public string OrganizationalUnit
        {
            get
            {
                return _organizationalUnit;
            }
            set
            {
                _organizationalUnit = value;
            }
        } // end of OrganizationalUnit

        #endregion

        #region AssignmentType

        private AssignmentType _assignmentType;

        /// <summary>
        /// The type of assignement, fixed or shared
        /// </summary>
        public AssignmentType AssignmentType
        {
            get
            {
                return _assignmentType;
            }
            set
            {
                _assignmentType = value;
            }
        } // end of AssignmentType

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return String.Format("{0}, {1}, {2}", Resource.ToString(), OrganizationalUnit, AssignmentType.ToString());
        } // end of ToString

        #endregion

    } // end of ResourceOrganizationalUnitAssignment
}

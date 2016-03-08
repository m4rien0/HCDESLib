using Enums;
using GeneralHealthCareElements.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.ResourceHandling
{
    /// <summary>
    /// Resource assignment for health care staff
    /// </summary>
    public class ResourceAssignmentStaff : ResourceAssignment<EntityHealthCareStaff>
    {
        #region Contructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="resource">The resource to be assigned</param>
        /// <param name="organizationalUnit">The organizational unit it is assigned to, per default the root department model</param>
        /// <param name="assignmentType">The type of assignement, fixed or shared</param>
        public ResourceAssignmentStaff(EntityHealthCareStaff resource,
            string organizationalUnit = "RootDepartment",
            AssignmentType assignmentType = AssignmentType.Fixed
            ) : base(resource, organizationalUnit, assignmentType)
        {
        } // end of ResourceOrganizationalUnitAssignment

        #endregion

    } // end of
}

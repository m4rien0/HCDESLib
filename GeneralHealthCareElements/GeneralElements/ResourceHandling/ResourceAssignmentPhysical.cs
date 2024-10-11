using Enums;

namespace GeneralHealthCareElements.ResourceHandling
{
    /// <summary>
    /// Class for assignement of physical resources. Provides an additional property for the structural area it is assigned to
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ResourceAssignmentPhysical<T> : ResourceAssignment<T>
    {
        #region Contructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="resource">The resource to be assigned</param>
        /// <param name="organizationalUnit">The organizational unit it is assigned to, per default the root department model</param>
        /// <param name="structuralArea">The structural area it is assigned to, per default the root department model</param>
        /// <param name="assignmentType">The type of assignement, fixed or shared</param>
        public ResourceAssignmentPhysical(T resource,
            string organizationalUnit = null,
            string structuralArea = null,
            AssignmentType assignmentType = AssignmentType.Fixed
            ) : base(resource, organizationalUnit, assignmentType)
        {
            _structuralArea = structuralArea;
            if (structuralArea == null)
                _structuralArea = "RootDepartment";
            else
                _structuralArea = structuralArea;
        } // end of ResourceOrganizationalUnitAssignment

        #endregion Contructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StructuralArea

        private string _structuralArea;

        /// <summary>
        /// The structural area it is assigned to, per default the root department model
        /// </summary>
        public string StructuralArea
        {
            get
            {
                return _structuralArea;
            }
        } // end of StructuralArea

        #endregion StructuralArea
    } // end of ResourceAssignmentPhysical
}
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLStaffAssignment
    {
        public XMLStaffAssignment()
        {
            DefinedBySkillSets = false;
            OrganizationalUnit = "RootDepartment";
            AssignmentType = "Fixed";
        } // end of XMLStaffAssignment

        [XmlAttribute]
        public bool DefinedBySkillSets { get; set; }

        [XmlAttribute]
        public int StaffID { get; set; }

        [XmlAttribute]
        public int SkillID { get; set; }

        [XmlAttribute]
        public string OrganizationalUnit { get; set; }

        [XmlAttribute]
        public string AssignmentType { get; set; }
    } // end of XMLStaffAssignment
}
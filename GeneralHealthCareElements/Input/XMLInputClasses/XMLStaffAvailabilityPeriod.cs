using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLStaffAvailabilityPeriod
    {
        public XMLStaffAvailabilityPeriod()
        {
            DefinedBySkillSets = false;
        } // end of XMLStaffAvailabilityPeriod

        [XmlAttribute]
        public bool DefinedBySkillSets { get; set; }

        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public double StartHour { get; set; }

        [XmlAttribute]
        public double EndHour { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Assignment")]
        public List<XMLStaffAssignment> DoctorAssignments { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Assignment")]
        public List<XMLStaffAssignment> NurseAssignments { get; set; }

    } // end of XMLStaffAvailabilityPeriod
}

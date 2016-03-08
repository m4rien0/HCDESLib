using GeneralHealthCareElements.Input.XMLInputClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input
{
    public class XMLInputHealthCareDepartment
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "SkillSet")]
        public List<XMLSkillSet> SkillSets { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Area")]
        public List<string> StructuralAreas { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Unit")]
        public List<string> OrganizationalUnits { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Facility")]
        public List<XMLTreatmentFacility> TreatmentFacilities { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "WaitingRoom")]
        public List<XMLWaitingRoom> WaitingRooms { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "ActionType")]
        public List<XMLActionType> ActionTypes { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Path")]
        public List<XMLPatientPath> Paths { get; set; }

        [XmlElement]
        public bool ConsiderPatientClassForPath { get; set; }

        [XmlElement]
        public bool ConsiderAdmissionForPath { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "PatientClass")]
        public List<XMLPatientClass> PatientClasses { get; set; }

        [XmlElement]
        public XMLStaffHandler StaffHandling { get; set; }


    } // end of XMLInputHealthCareDepartment
}

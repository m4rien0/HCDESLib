using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLTreatmentFacility
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public bool IsMultiplePatient { get; set; }

        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public double Width { get; set; }

        [XmlAttribute]
        public double Height { get; set; }

        [XmlElement]
        public int SkillSetID { get; set; }

        [XmlElement]
        public string OrganizationalUnit { get; set; }

        [XmlElement]
        public string StructuralArea { get; set; }

        [XmlElement]
        public string AssignmentType { get; set; }
    } // end of XMLTreatmentFacility
}
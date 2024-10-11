using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLWaitingRoom
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public string Identifier { get; set; }

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public double X { get; set; }

        [XmlAttribute]
        public double Y { get; set; }

        [XmlAttribute]
        public double Width { get; set; }

        [XmlAttribute]
        public double Height { get; set; }

        [XmlElement]
        public string OrganizationalUnit { get; set; }

        [XmlElement]
        public string StructuralArea { get; set; }
    } // end of XMLWaitingRoom
}
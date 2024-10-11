using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLAdmission
    {
        public XMLAdmission()
        {
            Probability = 1;
            MaxDays = double.MaxValue;
        } // end of XMLAdmission

        [XmlAttribute]
        public string AdmissionType { get; set; }

        [XmlAttribute]
        public double MaxDays { get; set; }

        [XmlAttribute]
        public double MinDays { get; set; }

        [XmlAttribute]
        public double Probability { get; set; }
    } // end of XMLAdmission
}
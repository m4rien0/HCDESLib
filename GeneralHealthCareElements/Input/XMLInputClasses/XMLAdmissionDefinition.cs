using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLAdmissionDefinition
    {

        [XmlAttribute]
        public string AdmissionType { get; set; }

        [XmlAttribute]
        public int Length { get; set; }

        [XmlAttribute]
        public double Capacity { get; set; }

        [XmlElement]
        public double NoShowProbability { get; set; }

        [XmlElement]
        public double ShowUpDeviationTriangularEarly { get; set; }

        [XmlElement]
        public double ShowUpDeviationTriangularMean { get; set; }

        [XmlElement]
        public double ShowUpDeviationTriangularLate { get; set; }

    } // end of XMLSlotLengthAndAdmissionDefinition
}

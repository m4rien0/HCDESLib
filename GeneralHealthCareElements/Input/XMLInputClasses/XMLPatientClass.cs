using SimulationCore.MathTool.Distributions;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLPatientClass
    {
        public XMLPatientClass()
        {
            ArrivalMode = "WalkIn";
            SinglePath = -1;
        } // end of XMLPatientClass

        [XmlAttribute]
        public string Category { get; set; }

        [XmlAttribute]
        public int Priority { get; set; }

        [XmlAttribute]
        public string ArrivalMode { get; set; }

        [XmlAttribute]
        public string AdmissionType { get; set; }

        [XmlAttribute]
        public double Probability { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "ID")]
        public List<int> PathIDs { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "PathProbability")]
        public List<double> PathIDProbabilities { get; set; }

        [XmlAttribute]
        public int SinglePath { get; set; }

        [XmlIgnore]
        public EmpiricalDiscreteDistribution<int> PathDistribution { get; set; }
    } // end of
}
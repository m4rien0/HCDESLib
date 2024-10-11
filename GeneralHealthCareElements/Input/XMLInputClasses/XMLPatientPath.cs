using SimulationCore.MathTool.Distributions;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLPatientPath
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Step")]
        public List<XMLPatientPathStep> PathSteps { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Admission")]
        public List<XMLAdmission> OutpatientAdmissions { get; set; }

        [XmlIgnore]
        public EmpiricalDiscreteDistribution<XMLAdmission> OutpatientAdmissionDistribution { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Admission")]
        public List<XMLAdmission> InpatientAdmissions { get; set; }

        [XmlIgnore]
        public EmpiricalDiscreteDistribution<XMLAdmission> InpatientAdmissionDistribution { get; set; }
    } // end of XMLPath
}
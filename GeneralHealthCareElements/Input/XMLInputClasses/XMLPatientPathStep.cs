﻿using SimulationCore.MathTool.Distributions;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLPatientPathStep
    {
        public XMLPatientPathStep()
        {
            StepDistribution = null;
        } // end of XMLPatientPathStep

        [XmlArray]
        [XmlArrayItem(ElementName = "Action")]
        public List<string> Actions { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Probability")]
        public List<double> ActionProbabilities { get; set; }

        [XmlAttribute]
        public string SingleAction { get; set; }

        [XmlIgnore]
        public EmpiricalDiscreteDistribution<string> StepDistribution { get; set; }
    }
}
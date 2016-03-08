using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLSkillSet
    {
        [XmlAttribute]
        public int ID { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "SingleSkill")]
        public List<SingleSkill> Skills { get; set; }
    } // end of 
}

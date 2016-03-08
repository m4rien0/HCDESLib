using GeneralHealthCareElements.BookingModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLTimeLinePerWeekdayDefinition
    {
        [XmlAttribute]
        public string Weekday { get; set; }

        [XmlElement]
        public TimeAtomStartEndIncrementConfig StartEndIncrementConfig { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Atom")]
        public List<TimeAtomConfig> TimeAtomConfigs { get; set; }

    } // end of XMLTimeLineDefinition
}

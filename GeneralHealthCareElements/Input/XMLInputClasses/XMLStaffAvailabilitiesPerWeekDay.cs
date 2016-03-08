using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLStaffAvailabilitiesPerWeekDay
    {
        [XmlAttribute]
        public string Weekday { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "ID")]
        public List<int> StaffAvailabilityIDs { get; set; }

    } // end of XMLStaffAvailabilitiesPerWeekDay
}

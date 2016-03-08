using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLStaffHandler
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "Doctor")]
        public List<XMLStaff> Doctors { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Nurse")]
        public List<XMLStaff> Nurses { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Period")]
        public List<XMLStaffAvailabilityPeriod> StaffAvailabilityPeriods { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "StaffPerWeek")]
        public List<XMLStaffAvailabilitiesPerWeekDay> StaffPerWeekdays { get; set; }

    } // end of XMLStaffHandler
}

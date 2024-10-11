using GeneralHealthCareElements.Input.XMLInputClasses;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input
{
    public class XMLInputHealthCareWithWaitingList : XMLInputHealthCareDepartment
    {
        [XmlArray]
        [XmlArrayItem(ElementName = "TimesPerDay")]
        public List<XMLOutpatientWaitingListDispatchingTimes> WaitingListDispatchingTimes { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Admission")]
        public List<XMLAdmissionDefinition> AdmissionDefinitions { get; set; }

        [XmlElement]
        public bool UseImmediateBookingModel { get; set; }

        [XmlElement]
        public bool DispatchWaitingListAtTimes { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "TimeLineConfig")]
        public List<XMLTimeLinePerWeekdayDefinition> TimeLineConfigsPerDay { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Day")]
        public List<string> BlockedDates { get; set; }
    } // end of XMLInputHealthCareWithWaitingList
}
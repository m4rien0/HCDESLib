﻿using System.Collections.Generic;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLOutpatientWaitingListDispatchingTimes
    {
        [XmlAttribute]
        public string Weekday { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "Hour")]
        public List<double> HoursForDispatching { get; set; }
    } // end of
}
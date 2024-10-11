﻿using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLStaff
    {
        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public int ID { get; set; }

        [XmlAttribute]
        public int SkillID { get; set; }
    } // end of XMLStaff
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneralHealthCareElements.Input.XMLInputClasses
{
    public class XMLActionType
    {
        public XMLActionType()
        {
            FacilitySkillSetID = -1;
            MainDoctorSkillSetID = -1;
            MainNurseSkillSetID = -1;
            AssistingDoctorSkillSetIDs = null;
            AssistingNurseSkillSetIDs = null;
            BusyFactorMainDoctor = 1;
            BusyFactorMainNurse = 1;
            BusyFactorAssistingDoctors = null;
            BusyFactorAssistingNurses = null;
        } // end of XMLActionType

        [XmlAttribute]
        public string Type { get; set; }

        [XmlAttribute]
        public string Identifier { get; set; }

        [XmlElement]
        public int FacilitySkillSetID { get; set; }

        [XmlElement]
        public int MainDoctorSkillSetID { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "SkillSet")]
        public int[] AssistingDoctorSkillSetIDs { get; set; }

        [XmlElement]
        public int MainNurseSkillSetID { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "SkillSet")]
        public int[] AssistingNurseSkillSetIDs { get; set; }

        [XmlElement]
        public double BusyFactorMainDoctor { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "BusyFactor")]
        public double[] BusyFactorAssistingDoctors { get; set; }

        [XmlElement]
        public double BusyFactorMainNurse { get; set; }

        [XmlArray]
        [XmlArrayItem(ElementName = "BusyFactor")]
        public double[] BusyFactorAssistingNurses { get; set; }

        [XmlElement]
        public bool DefinesCorrespondingDoctorStart { get; set; }

        [XmlElement]
        public bool DefinesCorrespondingDoctorEnd { get; set; }

        [XmlElement]
        public bool DefinesCorrespondingNurseStart { get; set; }

        [XmlElement]
        public bool DefinesCorrespondingNurseEnd { get; set; }

        [XmlElement]
        public bool DefinesFacilityOccupationStart { get; set; }

        [XmlElement]
        public bool DefinesFacilityOccupationEnd { get; set; }

        [XmlElement]
        public bool IsPreemptable { get; set; }

        [XmlElement]
        public bool IsHold { get; set; }

    } // end of XMLActionType
}

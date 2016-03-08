using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Describes a single skill with its type and level of expertise.
    /// </summary>
    public class SingleSkill
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="skill">Type of skill</param>
        /// <param name="levelOfExpertise">Level to which skill is mastered</param>
        public SingleSkill(string skill, int levelOfExpertise)
        {
            _skill = skill;
            _level = levelOfExpertise;
        } // end of SingleDoctoralSkill

        /// <summary>
        /// Empty constructor
        /// </summary>
        public SingleSkill()
        {
        } // end of SingleDoctoralSkill

        #endregion

        #region Skill

        private string _skill;

        /// <summary>
        /// Type of skill
        /// </summary>
        [XmlAttribute]
        public string Skill
        {
            get
            {
                return _skill;
            }
            set
            {
                _skill = value;
            }
        } // end of Skill

        #endregion

        #region Level

        private int _level;

        /// <summary>
        /// Level of expertise in this skill
        /// </summary>
        [XmlAttribute]
        public int Level
        {
            get
            {
                return _level;
            }
            set
            {
                _level = value;
            }
        } // end of LevelOfExpertis

        #endregion

        #region ToArray

        public SingleSkill[] ToArray()
        {
            return new SingleSkill[] { this };
        } // end of ToArray

        #endregion

    } // end of SingleDoctoralSkill
}

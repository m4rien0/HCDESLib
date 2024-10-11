using System.Collections.Generic;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Skill sets of entities consist of an array of single skills.
    /// </summary>
    public class SkillSet
    {
        #region Constructor

        /// <summary>
        /// Constructor that takes a single skill and transforms it into a skill set.
        /// </summary>
        /// <param name="skill">Single skill that defines the skill set</param>
        public SkillSet(SingleSkill skill)
        {
            _skills = skill.ToArray();
            _skillsPerType = new Dictionary<string, SingleSkill>();

            _skillsPerType.Add(skill.Skill, skill);
        } // end of SkillSet

        /// <summary>
        /// Constructor that takes an array of skills.
        /// </summary>
        /// <param name="skills">All skills to be included in skill set</param>
        public SkillSet(SingleSkill[] skills)
        {
            _skills = skills;
            _skillsPerType = new Dictionary<string, SingleSkill>();

            foreach (SingleSkill skill in skills)
            {
                _skillsPerType.Add(skill.Skill, skill);
            } // end foreach
        } // end of SkillSet

        /// <summary>
        /// Empty skill set.
        /// </summary>
        public SkillSet()
        {
            _skills = new SingleSkill[0];
            _skillsPerType = new Dictionary<string, SingleSkill>();
        } // end of SkillSet

        #endregion Constructor

        #region Skills

        private SingleSkill[] _skills;

        /// <summary>
        /// Gets all available skills
        /// </summary>
        public SingleSkill[] Skills
        {
            get
            {
                return _skills;
            }
        } // end of Skills

        #endregion Skills

        #region Indexer

        /// <summary>
        /// Indexer for skill set, the skill type can be indexed and the level of expertise is returned. In case
        /// that skill type is not covered by skill set a negative level of expertise is returned.
        /// </summary>
        /// <param name="skill">Skill type to be indexed</param>
        /// <returns>Level of expertise, if skill is not covered -1 is returned</returns>
        public int this[string skill]
        {
            get
            {
                if (SkillsPerType.ContainsKey(skill))
                    return SkillsPerType[skill].Level;
                else
                    return -1;
            }
        } // end of Indexer

        #endregion Indexer

        #region SkillsPerType

        private Dictionary<string, SingleSkill> _skillsPerType;

        /// <summary>
        /// Skills are held per type in this property
        /// </summary>
        public Dictionary<string, SingleSkill> SkillsPerType
        {
            get
            {
                return _skillsPerType;
            }
        } // end of SkillsPerType

        #endregion SkillsPerType
    } // end of SkillSet
}
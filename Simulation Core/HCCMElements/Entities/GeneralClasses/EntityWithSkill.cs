using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Entity that has one or several skills
    /// </summary>
    abstract public class EntityWithSkill : Entity
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor.
        /// </summary>
        /// <param name="identifier">Identifier of entity</param>
        /// <param name="skillSet">Skill set of entity</param>
        public EntityWithSkill(int identifier, SkillSet skillSet)
            : base(identifier)
        {
            _skillSet = skillSet;
        } // end of EntityWithSkill

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region SkillSet

        private SkillSet _skillSet;

        /// <summary>
        /// Skill set of entity
        /// </summary>
        public SkillSet SkillSet
        {
            get
            {
                return _skillSet;
            }
        } // end of SkillSet

        #endregion        

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region SatisfiesSkillSet

        /// <summary>
        /// Checks if the skill set of entity satisfies the required skill set passed as an argument.
        /// </summary>
        /// <param name="requiredSillSet">The required skills.</param>
        /// <returns>Returns true if the entities skill set satisfies the required skill set.</returns>
        public bool SatisfiesSkillSet(SkillSet requiredSillSet)
        {
            foreach (SingleSkill reqSkill in requiredSillSet.Skills)
            {
                if (reqSkill.Level > SkillSet[reqSkill.Skill])
                    return false;
            } // end foreach

            return true;

        } // end of SatisfiesSkillSet

        #endregion

        #region HasSingleSill

        /// <summary>
        /// Checks if the skill set of the entities includes a single skill type. Level of expertise is not checked.
        /// </summary>
        /// <param name="skillType">The identitfier of the single skill.</param>
        /// <returns>Returns true if the entities skill set includels the passed skill type.</returns>
        public bool HasSingleSill(string skillType)
        {

            if (SkillSet.SkillsPerType.ContainsKey(skillType))
                return SkillSet[skillType] >= 0;
            else
                return false;
        } // end of EntityNurse

        #endregion

    } // end of 
}

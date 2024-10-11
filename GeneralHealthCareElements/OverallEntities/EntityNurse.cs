﻿using SimulationCore.HCCMElements;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Entity representing a nurse
    /// </summary>
    public class EntityNurse : EntityHealthCareStaff
    {
        private static int RunningID = 0;

        #region Constructor

        /// <summary>
        /// Constructor which uses a default ID generation
        /// </summary>
        /// <param name="skillSet">Skill set of generated nurse</param>
        public EntityNurse(SkillSet skillSet)
            : base(RunningID++, skillSet)
        {
        } // end of EntityDoctor

        /// <summary>
        /// Constructor that uses an ID specified by user
        /// </summary>
        /// <param name="ID">ID of nurse</param>
        /// <param name="skillSet">Skill set of generated nurse</param>
        public EntityNurse(int ID, SkillSet skillSet)
            : base(ID, skillSet)
        {
        } // end of EntityDoctor

        #endregion Constructor

        #region ToString

        public override string ToString()
        {
            return "Nurse: " + Identifier.ToString();
        } // end of

        #endregion ToString

        #region Clone

        public override Entity Clone()
        {
            EntityNurse newNurse = new EntityNurse(Identifier, SkillSet);
            newNurse.BusyFactor = BusyFactor;
            return newNurse;
        } // end of Clone

        #endregion Clone
    } // end of EntityNurse
}
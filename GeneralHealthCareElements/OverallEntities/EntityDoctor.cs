using SimulationCore.HCCMElements;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Entity representing a doctor
    /// </summary>
    public class EntityDoctor : EntityHealthCareStaff
    {
        private static int RunningID = 0;

        #region Constructor

        /// <summary>
        /// Constructor which uses a default ID generation
        /// </summary>
        /// <param name="skillSet">Skill set of generated doctor</param>
        public EntityDoctor(SkillSet skillSet)
            : base(RunningID++, skillSet)
        {
        } // end of EntityDoctor

        /// <summary>
        /// Constructor that uses an ID specified by user
        /// </summary>
        /// <param name="ID">ID of doctor</param>
        /// <param name="skillSet">Skill set of generated doctor</param>
        public EntityDoctor(int ID, SkillSet skillSet)
            : base(ID, skillSet)
        {
        } // end of EntityDoctor

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Attributes
        //--------------------------------------------------------------------------------------------------

        #region AssociatedStaff

        private EntityStaff[] _associatedStaff;

        /// <summary>
        /// List of staff that might be associated with doctor, e.g. assistants or nurses
        /// </summary>
        public EntityStaff[] AssociatedStaff
        {
            get
            {
                return _associatedStaff;
            }
            set
            {
                _associatedStaff = value;
            }
        } // end of AssociatedStaff

        #endregion AssociatedStaff

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Doctor: " + Identifier.ToString();
        } // end of

        #endregion ToString

        #region Clone

        public override Entity Clone()
        {
            EntityDoctor newDoctor = new EntityDoctor(Identifier, SkillSet);
            newDoctor.BusyFactor = BusyFactor;
            return newDoctor;
        } // end of Clone

        #endregion Clone
    } // end of EntityDoctor
}
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Entity for a treatmetn facility that can accomodate multiple patients at the same time, hence inherits
    /// from a treatment facility and holding entity
    /// </summary>
    public class EntityMultiplePatientTreatmentFacility : EntityTreatmentFacility, IDynamicHoldingEntity
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="ID">ID of treatment facility</param>
        /// <param name="skillSet">Skill set defining the facility</param>
        /// <param name="type">Type of facility</param>
        /// <param name="position">Structural position of the facility</param>
        /// <param name="size">Size in the structure of the department</param>
        public EntityMultiplePatientTreatmentFacility(
            int ID, 
            SkillSet skillSet,
            string type,
            Point position,
            Size size)
            : base(ID, skillSet, type, position, size)
        {
            _containedEntities = new List<Entity>();
        } // end of EntityTreatmentBooth

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region HoldedEntities

        private List<Entity> _containedEntities;

        /// <summary>
        /// List implementing the hodling entity interface, lists all entities currently in the treatment facility
        /// </summary>
        public List<Entity> HoldedEntities
        {
            get
            {
                return _containedEntities;
            }
            set
            {
                _containedEntities = value;
            }
        } // end of WaitingEntities

        #endregion

        #region ParentControlUnit

        /// <summary>
        /// This changes the parent control unit property as a treatment facility does not change
        /// its parent control (this is handled via assigned facilities) by activities
        /// </summary>
        public override ControlUnit ParentControlUnit
        {
            get
            {
                return base.ParentControlUnit;
            }
            set
            {
                if (base.ParentControlUnit == null)
                    base.ParentControlUnit = value;
            }
        } // end of ParentControlUnit

        #endregion
        
    } // end of EntityMultiplePatientTreatmentFacility
}

using GeneralHealthCareElements.ControlUnits;
using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Delegates
{
    public class DelegateSentDocForAssistedTreatment : IDelegate
    {
        #region Constructor

        /// <summary>
        /// The sending delegate of a chosen doctor for assisting
        /// </summary>
        /// <param name="originControlUnit">The control unit that sends staff</param>
        /// <param name="requiredSkillSet">Skill set of the required doctor</param>
        public DelegateSentDocForAssistedTreatment(ControlUnitHealthCare originControlUnit, SkillSet requiredSkillSet)
        {
            _originControlUnit = originControlUnit;
            _doctoralSkillSet = requiredSkillSet;
        } // end of DelegateSentDoc

        #endregion

        #region OriginControlUnit

        private ControlUnitHealthCare _originControlUnit;

        /// <summary>
        /// The original control unit that sends the doctor
        /// </summary>
        public ControlUnit OriginControlUnit
        {
            get
            {
                return _originControlUnit;
            }
        } // end of OriginControlUnit

        #endregion

        #region DoctoralSkillSet

        private SkillSet _doctoralSkillSet;

        /// <summary>
        /// Required skill set
        /// </summary>
        public SkillSet DoctoralSkillSet
        {
            get
            {
                return _doctoralSkillSet;
            }
        } // end of DoctoralSkillSet

        #endregion        

        #region DoctorAssigned

        private EntityDoctor _doctorAssigned = null;

        /// <summary>
        /// Doctor assigned for sending
        /// </summary>
        public EntityDoctor DoctorAssigned
        {
            get
            {
                return _doctorAssigned;
            }
            set
            {
                _doctorAssigned = value;
            }
        } // end of DoctorAssigned

        #endregion
        
    } // end of DelegateSentDoc
}

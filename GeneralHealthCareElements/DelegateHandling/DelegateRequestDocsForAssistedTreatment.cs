using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Delegates
{
    /// <summary>
    /// A default delegate for assistance of extern doctors
    /// </summary>
    public class DelegateRequestDocsForAssisting : IDelegate
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="originControlUnit">Control unit that filed the request for assistance</param>
        /// <param name="requiredSkillSets">The required skill set of assisting doctors</param>
        public DelegateRequestDocsForAssisting(ControlUnit originControlUnit, List<SkillSet> requiredSkillSets)
        {
            _originControlUnit = originControlUnit;
            _requiredSkillSets = requiredSkillSets;
        } // end of DelegateEmergencyRequestDocs

        #endregion

        #region OriginControlUnit

        private ControlUnit _originControlUnit;

        /// <summary>
        /// Control unit that filed the request
        /// </summary>
        public ControlUnit OriginControlUnit
        {
            get
            {
                return _originControlUnit;
            }
        } // end of OriginControlUnit

        #endregion

        #region RequiredSkillSets

        private List<SkillSet> _requiredSkillSets;

        /// <summary>
        /// Required skill sets for assisting
        /// </summary>
        public List<SkillSet> RequiredSkillSets
        {
            get
            {
                return _requiredSkillSets;
            }
            set
            {
                _requiredSkillSets = value;
            }
        } // end of RequiredSkillSets

        #endregion
        
    } // end of DelegateEmergencyRequestDocs
}

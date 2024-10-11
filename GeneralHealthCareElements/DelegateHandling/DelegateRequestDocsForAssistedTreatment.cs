using SimulationCore.HCCMElements;
using System.Collections.Generic;

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

        #endregion Constructor

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

        #endregion OriginControlUnit

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

        #endregion RequiredSkillSets
    } // end of DelegateEmergencyRequestDocs
}
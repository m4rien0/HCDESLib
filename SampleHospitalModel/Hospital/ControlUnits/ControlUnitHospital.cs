using GeneralHealthCareElements.Management;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.Hospital
{
    /// <summary>
    /// Very basic example of hospital control, just uses default delegate handling
    /// for moving patients and assisting docs
    /// </summary>
    public class ControlUnitHospital : ControlUnitManagement
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="name">String identifier of control unit</param>
        /// <param name="parentControlUnit">Parent control, equals null if this control is root</param>
        /// <param name="parentSimulationModel">Parent simulation control</param>
        /// <param name="inputData">Input data for management control</param>
        public ControlUnitHospital(string name,
                            ControlUnit parentControlUnit,
                            SimulationModel parentSimulationModel,
                            IInputManagement inputData)
            : base(name, parentControlUnit, parentSimulationModel)
        {
            _inputData = inputData;
        }

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Rule Handling
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Empty rule set
        /// </summary>
        /// <param name="startTime">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns>False</returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            return false;

        } // end of PerformAssessment

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Input
        //--------------------------------------------------------------------------------------------------

        #region InputData

        private IInputManagement _inputData;

        override public IInputManagement InputData
        {
            get
            {
                return _inputData;
            }
        } // end of InputData

        #endregion

    } // end of HositpalControlUnit
}

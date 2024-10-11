﻿using GeneralHealthCareElements.Management;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using System;

namespace SampleHospitalModel.Hospital
{
    /// <summary>
    /// Sample input for management control
    /// </summary>
    public class InputHospital : IInputManagement
    {
        #region DurationMove

        /// <summary>
        /// Duration of moves between control, at the moment using just random durations
        /// </summary>
        /// <param name="entity">Moving entity</param>
        /// <param name="origin">Origin control unit of move</param>
        /// <param name="destination">Destination control unit of move</param>
        /// <returns>Duration of move</returns>
        public TimeSpan DurationMove(Entity entity, ControlUnit origin, ControlUnit destination)
        {
            return TimeSpan.FromMinutes(Distributions.Instance.RandomInteger(2, 5));
        } // end of DurationMove

        #endregion DurationMove
    } // end of InputHospital
}
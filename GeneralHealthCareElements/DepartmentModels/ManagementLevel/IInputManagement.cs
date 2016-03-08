using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.Management
{
    /// <summary>
    /// Input required for management controls, at this point contains only moving durations
    /// </summary>
    public interface IInputManagement
    {
        /// <summary>
        /// Duration of moves between control units
        /// </summary>
        /// <param name="entity">Moving entity</param>
        /// <param name="origin">Origin control unit of move</param>
        /// <param name="destination">Destination control unit of move</param>
        /// <returns>Duration of move</returns>
        TimeSpan DurationMove(Entity entity, ControlUnit origin, ControlUnit destination);

    } // end of IInputManagement
}

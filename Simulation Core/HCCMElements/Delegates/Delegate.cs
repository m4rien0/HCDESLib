using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Interface for all messages (delegates) to send between control units. The only propoerty required for
    /// such as message is the origin-control unit, i.e. the sender.
    /// </summary>
    public interface IDelegate
    {
        ControlUnit OriginControlUnit { get; } 

    } // end of Delegate
}

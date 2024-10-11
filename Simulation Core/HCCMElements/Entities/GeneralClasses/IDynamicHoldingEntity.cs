using System.Collections.Generic;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Interface for holding entities, e.g. waiting areas
    /// </summary>
    public interface IDynamicHoldingEntity
    {
        /// <summary>
        /// List of entities that are currently held
        /// </summary>
        List<Entity> HoldedEntities { get; }
    } // end of
}
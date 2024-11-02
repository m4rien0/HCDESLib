using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.ProductionAssets
{
    public class LineInstalledEvent(ControlUnit parentControlUnit)
        : Event(EventType.Standalone, parentControlUnit)
    {
        public override Entity[] AffectedEntities => [];

        public override Event Clone() => new LineInstalledEvent(ParentControlUnit);

        public override string ToString() => nameof(EquipmentNeededEvent);

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
        }
    }
}
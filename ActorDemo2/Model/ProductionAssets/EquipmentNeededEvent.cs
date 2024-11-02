using ActorDemo2.Model.FactoryOwners;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.ProductionAssets
{
    public class EquipmentNeededEvent(ControlUnit parentControlUnit, FactoryOwner buyer)
        : Event(EventType.Standalone, parentControlUnit)
    {
        public override Entity[] AffectedEntities => [Buyer];

        public FactoryOwner Buyer { get; set; } = buyer;

        public override Event Clone() => new EquipmentNeededEvent(ParentControlUnit, Buyer);

        public override string ToString() => nameof(EquipmentNeededEvent);

        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            ParentControlUnit.AddRequest(
                new ProductionLineRequest(
                    ProductionLineRequest.ACTIVITY_NAME,
                    Buyer,
                    time
                )
            );
        }
    }
}
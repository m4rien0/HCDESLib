using ActorDemo.Model.Entities;
using ActorDemo.Model.Requests;
using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.Events
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
                new SearchRequest(
                    SearchRequest.SEARCH_ACTIVITY,
                    Buyer,
                    time,
                    EquipmentType.RobotArm
                )
            );
        }
    }
}
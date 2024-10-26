using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;

namespace ActorDemo.Model.Entities
{
    public class FactoryOwner : Entity
    {
        public const string ENTITY_NAME = nameof(FactoryOwner);
        private static int _runningActorId;

        public FactoryOwner()
            : base(_runningActorId++)
        {
            MachineInventory = [];
        }

        public IList<ProductionEquipment> MachineInventory { get; set; }

        public override Entity Clone()
        {
            return new FactoryOwner()
            {
                MachineInventory = MachineInventory
            };
        }

        public override string ToString()
        {
            return $"{ENTITY_NAME} {Identifier}";
        }
    }
}
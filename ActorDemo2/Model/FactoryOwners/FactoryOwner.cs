using ActorDemo2.Model.Base;
using ActorDemo2.Model.ProductionAssets;
using SimulationCore.HCCMElements;
using System.Drawing;

namespace ActorDemo2.Model.FactoryOwners
{
    public class FactoryOwner : Actor
    {
        private static int _runningActorId;

        public FactoryOwner()
            : base(_runningActorId++)
        {
            MachineInventory = [];
            Requirements = [];

            Name = ToString();
            Location = new Point(0, 0);
        }

        public IList<ProductionEquipment> MachineInventory { get; set; }

        public IList<FactoryOwnerRequirement> Requirements { get; set; }

        public static void ResetId()
        {
            _runningActorId = 0;
        }

        public override Entity Clone()
        {
            return new FactoryOwner()
            {
                Location = Location,
                MachineInventory = MachineInventory,
                Name = Name
            };
        }

        public void EnterRequirements()
        {
            // temporary method that creates a production line requirement for this scenario

            IDictionary<EquipmentType, double> productionLineRequirement = new Dictionary<EquipmentType, double>()
            {
                { EquipmentType.ConveyerBelt, 22.2 },
                { EquipmentType.Robot, 5 },
                { EquipmentType.CncMachine, 1 },
                { EquipmentType.MillingMachine, 3 },
                { EquipmentType.AuxilaryEquipment,  6}
            };
            Requirements.Add(new FactoryOwnerRequirement(productionLineRequirement));
        }

        public override string ToString()
        {
            return $"{nameof(FactoryOwner)} {Identifier}";
        }
    }
}
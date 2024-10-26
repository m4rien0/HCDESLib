using ActorDemo.Model.Entities;

namespace ActorDemo.Model.Resources
{
    public enum EquipmentType
    {
        Robot,

        RobotArm,

        DrillingMachine,

        TurningMachine,

        MillingMachine,

        CncMachine,

        ConveyerBelt,

        AuxilaryEquipment
    }

    public class ProductionEquipment(FactoryOwner owner, EquipmentType type, double bookPrice = 0.0)
    {
        public double? AskingPrice { get; set; } = null;

        public double BookPrice { get; set; } = bookPrice;

        public EquipmentType EquipmentType { get; set; } = type;

        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsForSale { get; set; } = false;

        public FactoryOwner Owner { get; set; } = owner;
    }
}
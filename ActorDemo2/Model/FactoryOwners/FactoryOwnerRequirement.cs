using ActorDemo2.Model.ProductionAssets;

namespace ActorDemo2.Model.FactoryOwners
{
    public class FactoryOwnerRequirement(IDictionary<EquipmentType, double> equipmentRequirements)
    {
        public IDictionary<EquipmentType, double> EquipmentRequirements { get; } = equipmentRequirements;

        public Guid Id { get; } = Guid.NewGuid();

        public double TotalBudget { get; set; } = 0.0;
    }
}
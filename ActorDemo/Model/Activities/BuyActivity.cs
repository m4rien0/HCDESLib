using ActorDemo.Model.Entities;
using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.Activities
{
    public class BuyActivity(
            ControlUnit parentControlUnit,
            FactoryOwner buyer,
            FactoryOwner seller,
            List<ProductionEquipment> tradeAssets)
        : Activity(parentControlUnit, nameof(BuyActivity), false)
    {
        public override Entity[] AffectedEntities => [Buyer, Seller];

        public FactoryOwner Buyer { get; set; } = buyer;

        public FactoryOwner Seller { get; set; } = seller;

        public List<ProductionEquipment> TradeAssets { get; set; } = tradeAssets;

        public override Activity Clone() => new BuyActivity(ParentControlUnit, Buyer, Seller, TradeAssets);

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            TradeAssets.ForEach(Buyer.MachineInventory.Add);
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            TradeAssets.ForEach(asset =>
            {
                Seller.MachineInventory.Remove(asset);
                asset.Owner = Buyer;
                asset.IsForSale = false;
            });

            TransportActivity transportActivity = new(ParentControlUnit, Buyer, Seller, this);
            simEngine.AddScheduledEvent(transportActivity.StartEvent, time.AddSeconds(30));
        }

        public override string ToString() => $"{Buyer} bought {TradeAssets.Count} assets from {Seller}.";
    }
}
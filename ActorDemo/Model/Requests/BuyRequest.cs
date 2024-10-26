using ActorDemo.Model.Activities;
using ActorDemo.Model.Entities;
using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;

namespace ActorDemo.Model.Requests
{
    public class BuyRequest(string activity, FactoryOwner buyer, DateTime time, FactoryOwner seller, IList<ProductionEquipment> equipment)
        : ActivityRequest(activity, [buyer], time)
    {
        public const string BUY_ACTIVITY = nameof(BuyActivity);

        public FactoryOwner Buyer { get; } = buyer;

        public IList<ProductionEquipment> Equipment { get; } = equipment;

        public FactoryOwner Seller { get; } = seller;
    }
}
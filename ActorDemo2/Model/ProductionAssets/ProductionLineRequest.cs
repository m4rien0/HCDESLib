using ActorDemo2.Model.FactoryOwners;
using SimulationCore.HCCMElements;

namespace ActorDemo2.Model.ProductionAssets
{
    public class ProductionLineRequest(string activity, FactoryOwner buyer, DateTime time)
        : ActivityRequest(activity, [buyer], time)
    {
        public const string ACTIVITY_NAME = "ProductionLineRequested";

        public FactoryOwner Buyer { get; } = buyer;
    }
}
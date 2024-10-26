using ActorDemo.Model.Activities;
using ActorDemo.Model.Entities;
using ActorDemo.Model.Events;
using ActorDemo.Model.Requests;
using ActorDemo.Model.Resources;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.ControlUnits
{
    public class ActorDemoPlatformControlUnit : ControlUnit
    {
        public ActorDemoPlatformControlUnit(string name,
            ControlUnit? parentControlUnit,
            SimulationModel parentSimulationModel,
            FactoryOwner? buyer = null,
            FactoryOwner? seller = null,
            List<ProductionEquipment>? equipment = null)
        : base(name, parentControlUnit, parentSimulationModel)
        {
            PlatformProductionEquipment = equipment ?? [];

            Buyer = buyer ?? new();
            AddEntity(Buyer);

            Seller = seller ?? new();
            AddEntity(Seller);
        }

        public FactoryOwner Buyer { get; set; }

        public IEnumerable<ProductionEquipment> EquipmentForSale => PlatformProductionEquipment.Where(x => x.IsForSale);

        public List<ProductionEquipment> PlatformProductionEquipment { get; }

        public FactoryOwner Seller { get; set; }

        public override Event EntityEnterControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }

        public override void EntityLeaveControlUnit(DateTime time, ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        }

        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            // init inventory
            PlatformProductionEquipment.AddRange(Buyer.MachineInventory);
            PlatformProductionEquipment.AddRange(Seller.MachineInventory);

            Event initEvent = new EquipmentNeededEvent(this, Buyer);
            simEngine.AddScheduledEvent(initEvent, startTime);
        }

        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            IEnumerable<SearchRequest> searchRequests
                = RAEL.Where(x => x.Activity.Equals(SearchRequest.SEARCH_ACTIVITY)).Cast<SearchRequest>();
            HandleSearchRequests(time, simEngine, [.. searchRequests]);

            IEnumerable<BuyRequest> buyRequests
                = RAEL.Where(x => x.Activity.Equals(BuyRequest.BUY_ACTIVITY)).Cast<BuyRequest>();
            HandleBuyRequests(time, simEngine, [.. buyRequests]);

            return false;
        }

        private void HandleBuyRequests(DateTime time, ISimulationEngine simEngine, IList<BuyRequest> buyRequests)
        {
            foreach (BuyRequest buyRequest in buyRequests)
            {
                BuyActivity purchaseActivity = new(this, Buyer, Seller, [.. buyRequest.Equipment]);
                purchaseActivity.StartEvent.Trigger(time, simEngine);

                RemoveRequest(buyRequest);
            }
        }

        private void HandleSearchRequests(DateTime time, ISimulationEngine simEngine, IList<SearchRequest> searchRequests)
        {
            foreach (SearchRequest searchRequest in searchRequests)
            {
                List<ProductionEquipment> searchMatches = EquipmentForSale.Where(x => x.EquipmentType == searchRequest.Search).ToList();

                simEngine.AddScheduledEvent(new SearchCompletedEvent(this, Buyer, searchMatches), time.AddMinutes(1));
                RemoveRequest(searchRequest);
            }
        }
    }
}
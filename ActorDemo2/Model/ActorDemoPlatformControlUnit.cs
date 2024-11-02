using ActorDemo2.Model.FactoryOwners;
using ActorDemo2.Model.Platform;
using ActorDemo2.Model.ProductionAssets;
using ActorDemo2.Model.Services;
using ActorDemo2.Model.SystemIntegration;
using ActorDemo2.Model.Transactions;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model
{
    public class ActorDemoPlatformControlUnit : ControlUnit
    {
        public ActorDemoPlatformControlUnit(string name,
            ControlUnit? parentControlUnit,
            SimulationModel parentSimulationModel,
            FactoryOwner? buyer = null,
            ServiceProvider? platformOperator = null,
            SystemIntegrator? systemIntegrator = null,
            List<ProductionEquipment>? equipment = null)
        : base(name, parentControlUnit, parentSimulationModel)
        {
            PlatformProductionEquipment = equipment ?? [];

            Buyer = buyer ?? new();
            AddEntity(Buyer);

            PlatformOperator = platformOperator ?? new(3);
            AddEntity(PlatformOperator);

            SystemIntegrator = systemIntegrator ?? new();
            AddEntity(SystemIntegrator);

            PlatformStaffMembers = [];
            PlatformStaff staff = new();
            PlatformStaffMembers.Add(staff);
            AddEntity(staff);
        }

        public FactoryOwner Buyer { get; set; }

        public IEnumerable<ProductionEquipment> EquipmentForSale => PlatformProductionEquipment.Where(x => x.IsForSale);

        public ServiceProvider PlatformOperator { get; set; }

        public List<ProductionEquipment> PlatformProductionEquipment { get; }

        public IList<PlatformStaff> PlatformStaffMembers { get; set; }

        public SystemIntegrator SystemIntegrator { get; set; }

        private Dictionary<ServiceType, IList<ServiceOffer>> ServiceOffers { get; } = [];

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

            ServiceOffers.Add(ServiceType.Platform, [
                new ServiceOffer(ServiceConstants.SERVICE_PLATFORM_FACTORY_OWNER_REQUIREMENTS_KEY, ServiceConstants.SERVICE_PLATFORM_FACTORY_OWNER_REQUIREMENTS_LABEL, true),
                new ServiceOffer(ServiceConstants.SERVICE_PLATFORM_AI_MATCHMAKING_KEY, ServiceConstants.SERVICE_PLATFORM_AI_MATCHMAKING_LABEL, false)
            ]);

            PlatformOperator.ServicePortfolio.Add(
                ServiceConstants.SERVICE_PLATFORM_FACTORY_OWNER_REQUIREMENTS_KEY,
                new Service(ServiceOffers[ServiceType.Platform].Single(x => x.ServiceOfferKey.Equals(ServiceConstants.SERVICE_PLATFORM_FACTORY_OWNER_REQUIREMENTS_KEY)), 4.0)
            );
            PlatformOperator.ServicePortfolio.Add(
                ServiceConstants.SERVICE_PLATFORM_AI_MATCHMAKING_KEY,
                new Service(ServiceOffers[ServiceType.Platform].Single(x => x.ServiceOfferKey.Equals(ServiceConstants.SERVICE_PLATFORM_AI_MATCHMAKING_KEY)), 0.02)
            );

            // in a later version, a system integrator will have a project combosed from multiple services
            // here, for simplicity reason, this will only be one service with a long duration
            // duration: 15 work days (8 hours = work day)
            SystemIntegrator.SystemIntegrationProject.InitializeServices([SystemIntegrator.SystemIntegrationProject], projectDuration: 15 * 8);

            Event initEvent = new EquipmentNeededEvent(this, Buyer);
            simEngine.AddScheduledEvent(initEvent, startTime);
        }

        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            IEnumerable<ProductionLineRequest> lineRequests
                = RAEL.Where(x => x.Activity.Equals(ProductionLineRequest.ACTIVITY_NAME)).Cast<ProductionLineRequest>();
            HandleProductionLineRequests(time, simEngine, [.. lineRequests]);

            return false;
        }

        private void HandleProductionLineRequests(DateTime time, ISimulationEngine simEngine, IList<ProductionLineRequest> lineRequests)
        {
            foreach (ProductionLineRequest lineRequest in lineRequests)
            {
                Order lineOrder = new(Buyer);

                if (Buyer.Requirements.Count == 0)
                {
                    // Single here, since factory owner requirement service is hardcoded
                    ServiceOffer forService = ServiceOffers[ServiceType.Platform]
                        .Single(x => x.ServiceOfferKey.Equals(ServiceConstants.SERVICE_PLATFORM_FACTORY_OWNER_REQUIREMENTS_KEY));

                    Service forProviderService = PlatformOperator.ServicePortfolio[forService.ServiceOfferKey];
                    Transaction forTransaction = new(Buyer, PlatformOperator, forProviderService)
                    {
                        PreTransaction = (Activity _, ControlUnit _, ISimulationEngine _, DateTime _) =>
                        {
                            Buyer.EnterRequirements();
                        },
                    };
                    lineOrder.Transactions.Add(forTransaction);
                }

                // Single here, since the AI matchmaking service is hardcoded
                ServiceOffer aiMatchmakingService = ServiceOffers[ServiceType.Platform]
                    .Single(x => x.ServiceOfferKey.Equals(ServiceConstants.SERVICE_PLATFORM_AI_MATCHMAKING_KEY));

                Service aiProviderService = PlatformOperator.ServicePortfolio[aiMatchmakingService.ServiceOfferKey];
                Transaction aiTransaction = new(Buyer, PlatformOperator, aiProviderService);
                lineOrder.Transactions.Add(aiTransaction);

                Service integrationService = SystemIntegrator.SystemIntegrationProject;
                Transaction integrationTransaction = new(Buyer, PlatformOperator, integrationService);
                lineOrder.Transactions.Add(integrationTransaction);

                RemoveRequest(lineRequest);

                OrderActivity orderActivity = new(this, lineOrder);
                simEngine.AddScheduledEvent(orderActivity.StartEvent, time);
            }
        }
    }
}
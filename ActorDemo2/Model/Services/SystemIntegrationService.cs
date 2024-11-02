namespace ActorDemo2.Model.Services
{
    public class SystemIntegrationService : Service
    {
        public SystemIntegrationService()
            : base(new ServiceOffer(ServiceConstants.SYSTEM_INTEGRATION, "System Integration Service Bundle", false), 0.0)
        {
            BundledServices = [];
        }

        public IList<Service> BundledServices { get; set; }

        public void InitializeServices(IList<Service> services, double? lumpSum = null, double? projectDuration = null)
        {
            BundledServices = new List<Service>(services);

            MeanDuration = projectDuration ?? BundledServices.Sum(x => x.MeanDuration);
            Cost = lumpSum ?? BundledServices.Sum(x => x.Cost);
        }
    }
}
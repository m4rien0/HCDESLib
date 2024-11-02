namespace ActorDemo2.Model.Services
{
    public class ServiceOffer(string key, string label, bool needsStaffCheck)
    {
        public Guid Id { get; set; }

        public string Label { get; set; } = label;

        public bool NeedsStaffCheck { get; set; } = needsStaffCheck;

        public string ServiceOfferKey { get; set; } = key;

        public ServiceOfferType ServiceOfferType { get; set; }

        public ServiceType ServiceType { get; set; }
    }
}
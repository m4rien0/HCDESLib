namespace ActorDemo2.Model.Services
{
    public class Service(ServiceOffer service, double meanDuration)
    {
        public double Cost { get; set; } = 0.0;

        public Guid Id { get; set; } = Guid.NewGuid();

        public double MeanDuration { get; set; } = meanDuration;

        public ServiceOffer ServiceOffer { get; set; } = service;

        public override string ToString() => $"Service {ServiceOffer.Label} ({Id})";
    }
}
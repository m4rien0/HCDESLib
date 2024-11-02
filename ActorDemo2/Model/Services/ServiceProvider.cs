using ActorDemo2.Model.Base;
using SimulationCore.HCCMElements;

namespace ActorDemo2.Model.Services
{
    public class ServiceProvider(int totalCapacity) : Actor(_runningActorId++)
    {
        private static int _runningActorId;

        public int FreeCapacity => TotalCapacity - Occupied;

        public int Occupied { get; set; } = 0;

        public IDictionary<string, Service> ServicePortfolio { get; set; } = new Dictionary<string, Service>();

        public int TotalCapacity { get; set; } = totalCapacity;

        public static void ResetId()
        {
            _runningActorId = 0;
        }

        public override Entity Clone() =>
            new ServiceProvider(TotalCapacity)
            {
                ServicePortfolio = ServicePortfolio
            };

        public override string ToString() => $"{nameof(ServiceProvider)} {Identifier}";
    }
}
using ActorDemo2.Model.Base;
using ActorDemo2.Model.Services;
using SimulationCore.HCCMElements;

namespace ActorDemo2.Model.SystemIntegration
{
    public class SystemIntegrator() : Actor(_runningActorId++)
    {
        private static int _runningActorId;

        public Dictionary<string, Service> ServicePortfolio { get; set; } = [];

        public SystemIntegrationService SystemIntegrationProject { get; set; } = new();

        public static void ResetId()
        {
            _runningActorId = 0;
        }

        public override Entity Clone() => new SystemIntegrator() { SystemIntegrationProject = SystemIntegrationProject, ServicePortfolio = ServicePortfolio };

        public override string ToString() => $"{nameof(SystemIntegrator)} {Identifier}";
    }
}
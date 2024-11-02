using ActorDemo2.Model.Base;
using SimulationCore.HCCMElements;

namespace ActorDemo2.Model.Services
{
    public class SystemIntegrator() : Actor(_runningId++)
    {
        private static int _runningId;

        public Dictionary<string, Service> ServicePortfolio { get; set; } = [];

        public SystemIntegrationService SystemIntegrationProject { get; set; } = new();

        public override Entity Clone() => new SystemIntegrator() { SystemIntegrationProject = SystemIntegrationProject, ServicePortfolio = ServicePortfolio };

        public override string ToString() => $"{nameof(SystemIntegrator)} {Identifier}";
    }
}
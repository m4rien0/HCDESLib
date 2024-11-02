using ActorDemo2.Model.Base;
using SimulationCore.HCCMElements;

namespace ActorDemo2.Model.SystemIntegration
{
    public class SystemIntegrator : Actor
    {
        private static int _runningActorId;

        public SystemIntegrator() : base(_runningActorId++)
        {
        }

        public static void ResetId()
        {
            _runningActorId = 0;
        }

        public override Entity Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return $"{nameof(SystemIntegrator)} {Identifier}";
        }
    }
}
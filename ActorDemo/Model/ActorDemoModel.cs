using SimulationCore.SimulationClasses;

namespace ActorDemo.Model
{
    public class ActorDemoModel : SimulationModel
    {
        public const string MODEL_NAME = nameof(ActorDemoModel);
        public const string CONTROL_UNIT_NAME = "ActorDemoControl";

        public ActorDemoModel(DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            Factories = [];
            ServiceProviders = [];

            //_rootControlUnit = new();
        }

        public List<FactoryOwner> Factories { get; }
        public List<ServiceProvider> ServiceProviders { get; }

        public override void CustomInitializeModel()
        {
            // no custom init needed
        }

        public override string GetModelString()
        {
            return MODEL_NAME;
        }
    }
}
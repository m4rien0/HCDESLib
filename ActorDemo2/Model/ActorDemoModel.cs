using ActorDemo2.Model.FactoryOwners;
using ActorDemo2.Model.Platform;
using ActorDemo2.Model.Services;
using ActorDemo2.Model.SystemIntegration;
using ActorDemo2.Visualization;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using WPFVisualizationBase;

namespace ActorDemo2.Model
{
    public class ActorDemoModel : SimulationModel
    {
        public const string CONTROL_UNIT_NAME = "ActorDemoControl";
        public const string MODEL_NAME = nameof(ActorDemoModel);

        public ActorDemoModel(DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            _rootControlUnit = new ActorDemoPlatformControlUnit(CONTROL_UNIT_NAME, null, this);
        }

        public override void CustomInitializeModel()
        {
            // no custom init needed
        }

        public override string GetModelString()
        {
            return MODEL_NAME;
        }

        public override void InitializeVisualization(object args)
        {
            BaseWPFModelVisualization visualEngine = new(this, (DrawingOnCoordinateSystem)args);

            visualEngine.VisualizationPerControlUnit.Add(RootControlUnit, new ActorDemoVisualizationEngine((DrawingOnCoordinateSystem)args));

            _simulationDrawingEngine = visualEngine;
        }

        public override void ResetModel()
        {
            FactoryOwner.ResetId();
            PlatformStaff.ResetId();
            SystemIntegrator.ResetId();
            ServiceProvider.ResetId();
        }
    }
}
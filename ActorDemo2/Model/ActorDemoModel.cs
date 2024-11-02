using ActorDemo2.Model.FactoryOwners;
using ActorDemo2.Model.Platform;
using ActorDemo2.Model.SystemIntegration;
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
            //FactoryOwner demoSeller = new();
            //demoSeller.MachineInventory = [
            //    new ProductionEquipment(demoSeller, EquipmentType.Robot) { IsForSale = true },
            //    new ProductionEquipment(demoSeller, EquipmentType.RobotArm) { IsForSale = true },
            //    new ProductionEquipment(demoSeller, EquipmentType.RobotArm) { IsForSale = true },
            //    new ProductionEquipment(demoSeller, EquipmentType.MillingMachine) { IsForSale = true },
            //];

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
            _simulationDrawingEngine = new BaseWPFModelVisualization(this, (DrawingOnCoordinateSystem)args);

            // TODO
            //((BaseWPFModelVisualization)SimulationDrawingEngine).VisualizationPerControlUnit.Add(RootControlUnit, new ActorDemoVisualizationEngine((DrawingOnCoordinateSystem)args));
        }

        public override void ResetModel()
        {
            FactoryOwner.ResetId();
            PlatformStaff.ResetId();
            SystemIntegrator.ResetId();
        }
    }
}
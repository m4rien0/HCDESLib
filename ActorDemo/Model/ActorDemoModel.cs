using ActorDemo.Model.ControlUnits;
using ActorDemo.Model.Entities;
using ActorDemo.Model.Resources;
using ActorDemo.Visualization;
using SimulationCore.SimulationClasses;
using SimulationWPFVisualizationTools;
using WPFVisualizationBase;

namespace ActorDemo.Model
{
    public class ActorDemoModel : SimulationModel
    {
        public const string CONTROL_UNIT_NAME = "ActorDemoControl";
        public const string MODEL_NAME = nameof(ActorDemoModel);

        public ActorDemoModel(DateTime startTime, DateTime endTime)
            : base(startTime, endTime)
        {
            FactoryOwner demoSeller = new();
            demoSeller.MachineInventory = [
                new ProductionEquipment(demoSeller, EquipmentType.Robot) { IsForSale = true },
                new ProductionEquipment(demoSeller, EquipmentType.RobotArm) { IsForSale = true },
                new ProductionEquipment(demoSeller, EquipmentType.RobotArm) { IsForSale = true },
                new ProductionEquipment(demoSeller, EquipmentType.MillingMachine) { IsForSale = true },
            ];

            _rootControlUnit = new ActorDemoPlatformControlUnit(CONTROL_UNIT_NAME, null, this, seller: demoSeller);
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

            ((BaseWPFModelVisualization)SimulationDrawingEngine).VisualizationPerControlUnit.Add(RootControlUnit, new ActorDemoVisualizationEngine((DrawingOnCoordinateSystem)args));
        }
    }
}
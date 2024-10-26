using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.Activities
{
    public class TransportActivity(
            ControlUnit parentControlUnit,
            Entity source,
            Entity destination,
            Activity startingActivity)
        : Activity(parentControlUnit, nameof(BuyActivity), false)
    {
        private readonly Activity _startingActivitiy = startingActivity;

        public override Entity[] AffectedEntities => [];

        public Entity Destination { get; set; } = destination;

        public Entity Source { get; set; } = source;

        public override Activity Clone() => new TransportActivity(ParentControlUnit, Source, Destination, _startingActivitiy);

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            // not much to do hear for now ...
            simEngine.AddScheduledEvent(_startingActivitiy.EndEvent, time.AddSeconds(10));

            // future: set asset location, calculate CO2 emission, ...
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            double transportTime = new Random().NextDouble() * 5;
            DateTime endEventTime = time.AddMinutes(Distributions.Instance.Exponential(transportTime));

            // for now: random time for transport
            simEngine.AddScheduledEvent(EndEvent, endEventTime);

            // future: determine route, calculate route distance and duration, set transport method, ...
        }

        public override string ToString() => $"Transport from {Source} to {Destination}.";
    }
}
using ActorDemo2.Model.ProductionAssets;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.Transactions
{
    public class OrderActivity(ControlUnit parentControlUnit, Order order)
        : Activity(parentControlUnit, nameof(TransactionActivity), false)
    {
        public override Entity[] AffectedEntities => [];

        public Order Order { get; set; } = order;

        public override Activity Clone() => new OrderActivity(ParentControlUnit, Order);

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            Order.State = OrderState.Successful;

            Event initEvent = new LineInstalledEvent(ParentControlUnit);
            simEngine.AddScheduledEvent(initEvent, time.AddSeconds(10));
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            Order.State = OrderState.InProcess;

            TransactionActivity activity = new(ParentControlUnit, Order.Transactions.First(), Order.Transactions.Skip(1), EndEvent);
            simEngine.AddScheduledEvent(activity.StartEvent, time);
        }

        public override string ToString() => $"Executing Order: {Order}";
    }
}
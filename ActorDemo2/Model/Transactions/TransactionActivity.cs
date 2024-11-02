using ActorDemo2.Model.Platform;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.Transactions
{
    public class TransactionActivity(ControlUnit parentControlUnit, Transaction transaction, IEnumerable<Transaction> followUpTransactions, EventActivity orderEndEvent)
        : Activity(parentControlUnit, nameof(TransactionActivity), false)
    {
        public override Entity[] AffectedEntities => [];

        public IEnumerable<Transaction> FollowUpTransactions { get; set; } = followUpTransactions;

        public EventActivity OrderEndEvent { get; set; } = orderEndEvent;

        public Transaction Transaction { get; set; } = transaction;

        public override Activity Clone() => new TransactionActivity(ParentControlUnit, Transaction, FollowUpTransactions, OrderEndEvent);

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            Transaction.PostTransaction?.Invoke(this, ParentControlUnit, simEngine, time);
            Transaction.State = TransactionState.Done;

            if (Transaction.Service.ServiceOffer.NeedsStaffCheck)
            {
                // only a single staff member in this demo
                PlatformStaff staff = ((ActorDemoPlatformControlUnit)ParentControlUnit).PlatformStaffMembers.Single();
                CheckActivity check = new(ParentControlUnit, staff, Transaction, FollowUpTransactions, OrderEndEvent);
                simEngine.AddScheduledEvent(check.StartEvent, time);
                return;
            }

            if (FollowUpTransactions.Any())
            {
                TransactionActivity next = new(ParentControlUnit, FollowUpTransactions.First(), FollowUpTransactions.Skip(1), OrderEndEvent);
                simEngine.AddScheduledEvent(next.StartEvent, time);
            }
            else
            {
                simEngine.AddScheduledEvent(OrderEndEvent, time);
            }
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            Transaction.State = TransactionState.Waiting;
            Transaction.PreTransaction?.Invoke(this, ParentControlUnit, simEngine, time);

            double meanDurationInHours = Transaction.Service.MeanDuration;
            Transaction.State = TransactionState.InProcess;
            simEngine.AddScheduledEvent(EndEvent, time + TimeSpan.FromHours(Distributions.Instance.Exponential(meanDurationInHours)));
        }

        public override string ToString() => $"Executing Transaction: {Transaction}";
    }
}
using ActorDemo2.Model.Transactions;
using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.Platform
{
    public class CheckActivity(ControlUnit parentControlUnit, PlatformStaff staff, Transaction transaction, IEnumerable<Transaction> followUpTransactions, EventActivity orderEndEvent)
        : Activity(parentControlUnit, nameof(CheckActivity), false)
    {
        public override Entity[] AffectedEntities => [PlatformStaff];

        public IEnumerable<Transaction> FollowUpTransactions { get; set; } = followUpTransactions;

        public EventActivity OrderEndEvent { get; set; } = orderEndEvent;

        public PlatformStaff PlatformStaff { get; set; } = staff;

        public Transaction Transaction { get; set; } = transaction;

        public override Activity Clone() => new CheckActivity(ParentControlUnit, PlatformStaff, Transaction, FollowUpTransactions, OrderEndEvent);

        public override void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            // Demo only shows positive checks, later version will include a configurable amount
            // of "failed checks", e.g. 90% successful, 10% fail
            double successRate = 1.0;

            if (new Random().Next(100) < (successRate * 100))
            {
                Transaction.IsPlatformCheckSuccessful = true;
                Transaction.State = TransactionState.Checked;

                TransactionActivity next = new(ParentControlUnit, FollowUpTransactions.First(), FollowUpTransactions.Skip(1), OrderEndEvent);
                next.StartEvent.Trigger(time, simEngine);
            }
            else
            {
                Transaction correction = new(Transaction);
                TransactionActivity correctionActivity = new(ParentControlUnit, correction, FollowUpTransactions, OrderEndEvent);
                correctionActivity.StartEvent.Trigger(time, simEngine);
            }
        }

        public override void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            // Assumption: check on average needs half of the average time of the service being checked
            // e.g. if a factory owner mean time to enter FORs is 4h, the check will take 2h on average
            double checkTime = Transaction.Service.MeanDuration / 2;
            DateTime checkDoneTime = time + TimeSpan.FromHours(Distributions.Instance.Exponential(checkTime));
            simEngine.AddScheduledEvent(EndEvent, checkDoneTime);
        }

        public override string ToString() => $"Staff member {PlatformStaff} is checking {Transaction}";
    }
}
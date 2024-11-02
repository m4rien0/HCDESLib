using ActorDemo2.Model.Base;
using ActorDemo2.Model.Services;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.Transactions
{
    public class Transaction(Actor destination, Actor? source, Service service)
    {
        public Transaction(Transaction transaction)
            : this(transaction.Destination, transaction.Source, transaction.Service)
        {
            Id = Guid.NewGuid();
            IsPlatformCheckSuccessful = false;
            PostTransaction = transaction.PostTransaction;
            PreTransaction = transaction.PreTransaction;
            State = TransactionState.Waiting;
            Type = transaction.Type;
        }

        public Actor Destination { get; set; } = destination;

        public Guid Id { get; set; } = Guid.NewGuid();

        public bool IsPlatformCheckSuccessful { get; set; } = false;

        public Action<Activity, ControlUnit, ISimulationEngine, DateTime>? PostTransaction { get; set; }

        public Action<Activity, ControlUnit, ISimulationEngine, DateTime>? PreTransaction { get; set; }

        public Service Service { get; set; } = service;

        public Actor? Source { get; set; } = source;

        public TransactionState State { get; set; } = TransactionState.Queued;

        public TransactionType Type { get; set; }
    }
}
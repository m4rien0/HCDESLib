namespace ActorDemo2.Model.Transactions
{
    public enum TransactionState
    {
        Queued,
        Waiting,
        InProcess,
        Done,
        Checked,
        Canceled
    }
}
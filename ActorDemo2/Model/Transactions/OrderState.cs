namespace ActorDemo2.Model.Transactions
{
    public enum OrderState
    {
        Waiting,
        InProcess,
        Successful,
        SuccessfulWithCancellations,
        SuccessfulAfterFailedCheck,
        Failed
    }
}
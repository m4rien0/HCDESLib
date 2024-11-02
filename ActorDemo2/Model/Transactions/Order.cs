namespace ActorDemo2.Model.Transactions
{
    public class Order
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public OrderState State { get; set; } = OrderState.Waiting;

        public IList<Transaction> Transactions { get; set; } = [];
    }
}
using ActorDemo2.Model.Base;

namespace ActorDemo2.Model.Transactions
{
    public class Order(Actor creator)
    {
        public Actor Creator { get; set; } = creator;

        public Guid Id { get; set; } = Guid.NewGuid();

        public OrderState State { get; set; } = OrderState.Waiting;

        public IList<Transaction> Transactions { get; set; } = [];
    }
}
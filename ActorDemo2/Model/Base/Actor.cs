using ActorDemo2.Model.Transactions;
using SimulationCore.HCCMElements;
using System.Drawing;

namespace ActorDemo2.Model.Base
{
    public abstract class Actor(int identifier) : Entity(identifier)
    {
        public Point Location { get; set; }

        public string Name { get; set; }

        // Note: this will be replace with another structure later (e.g. real world coordinates)

        public IList<Order> Orders { get; set; } = [];
    }
}
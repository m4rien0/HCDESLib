using SimulationCore.HCCMElements;

namespace ActorDemo.Model
{
    public class FactoryOwner : Entity
    {
        public FactoryOwner(int identifier)
            : base(identifier)
        {
        }

        public override Entity Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
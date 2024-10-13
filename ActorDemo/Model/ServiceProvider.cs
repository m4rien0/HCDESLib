using SimulationCore.HCCMElements;

namespace ActorDemo.Model
{
    public class ServiceProvider : Entity
    {
        public ServiceProvider(int identifier)
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
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo.Model.Entities
{
    public class PlatformStaff : Entity, IActiveEntity
    {
        public PlatformStaff(int identifier)
            : base(identifier)
        {
        }

        public void AddActivity(Activity activity)
        {
            throw new NotImplementedException();
        }

        public override Entity Clone()
        {
            throw new NotImplementedException();
        }

        public List<Activity> GetCurrentActivities()
        {
            throw new NotImplementedException();
        }

        public bool IsInOnlyActivity(string activity)
        {
            throw new NotImplementedException();
        }

        public bool IsWaiting()
        {
            throw new NotImplementedException();
        }

        public bool IsWaitingOrPreEmptable()
        {
            throw new NotImplementedException();
        }

        public void RemoveActivity(Activity activity)
        {
            throw new NotImplementedException();
        }

        public Event StartWaitingActivity(IDynamicHoldingEntity? waitingArea = null)
        {
            throw new NotImplementedException();
        }

        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public void StopWaitingActivity(DateTime time, ISimulationEngine simEngine)
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
}
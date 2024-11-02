using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;

namespace ActorDemo2.Model.Platform
{
    public class PlatformStaff : Entity, IActiveEntity, IDynamicHoldingEntity
    {
        private static int _runningId;

        private readonly IList<Activity> _activities;

        public PlatformStaff()
            : base(_runningId++)
        {
            _activities = [];

            HoldedEntities = [];
        }

        public List<Entity> HoldedEntities { get; set; }

        public bool IsBusy { get; set; }

        public static void ResetId()
        {
            _runningId = 0;
        }

        public void AddActivity(Activity activity)
        {
            _activities.Add(activity);
        }

        public override Entity Clone() => new PlatformStaff();

        public List<Activity> GetCurrentActivities()
        {
            return [.. _activities];
        }

        public bool IsInOnlyActivity(string activity) => _activities.Count == 1 && _activities.First().ActivityName == activity;

        public bool IsWaiting() => _activities.Count == 1 && _activities.First() is ActivityWait;

        public bool IsWaitingOrPreEmptable() => IsWaiting() || _activities.All(x => x.PreEmptable());

        public void RemoveActivity(Activity activity)
        {
            _activities.Remove(activity);
        }

        public Event StartWaitingActivity(IDynamicHoldingEntity? waitingArea = null) => new ActivityWait(ParentControlUnit, this, waitingArea).StartEvent;

        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            foreach (Activity activity in _activities)
            {
                activity.EndEvent.Trigger(time, simEngine);
            }
        }

        public void StopWaitingActivity(DateTime time, ISimulationEngine simEngine)
        {
            if (IsWaiting())
            {
                StopCurrentActivities(time, simEngine);
            }
        }

        public override string ToString()
        {
            return $"{nameof(PlatformStaff)} {Identifier}";
        }
    }
}
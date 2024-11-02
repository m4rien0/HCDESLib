using SimulationCore.HCCMElements;
using System.Text;

namespace ActorDemo2.Logging
{
    /// <summary>
    /// Represents a full control unit state, consisting of triggered events, and a logging state, needs revision
    /// </summary>
    public class SimulationState
    {
        public SimulationState(LogControlUnitState rootControlUnitState, IEnumerable<Event> triggeredEvents, DateTime time)
        {
            RootControlUnitState = rootControlUnitState;
            TriggeredEvents = [.. triggeredEvents];
            TimeStamp = time;

            StringBuilder state = new();

            state.Append(RootControlUnitState.GetDescription());
            foreach (Event ev in triggeredEvents)
            {
                state.Append(TimeStamp.ToString("MM.dd:HH:mm:ss:fff") + ": ");
                state.Append(ev.GetDescription());
                state.AppendLine();
            }

            state.AppendLine("----------------------------------------------------------------------------------------------------------");

            LogState = state.ToString();
        }

        public string EventLog { get; } = "";

        public string LogState { get; } = "";

        public LogControlUnitState RootControlUnitState { get; }

        public DateTime TimeStamp { get; }

        public List<Event> TriggeredEvents { get; }

        public string GetDescription()
        {
            return LogState;
        }
    }
}
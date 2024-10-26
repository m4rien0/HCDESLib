using SimulationCore.HCCMElements;
using System.Text;

namespace ActorDemo.Logging
{
    /// <summary>
    /// Creates a state of a control unit for logging
    /// </summary>
    public class LogControlUnitState
    {
        public LogControlUnitState(DateTime timeStamp,
            BaseLoggingEngine loggingEngine,
            ControlUnit controlUnit)
        {
            StringBuilder description = new();

            description.Append(timeStamp.ToString("MM.dd:HH:mm:ss:fff")).Append(": ").Append(controlUnit.Name).Append(": RAEL length: ").Append(controlUnit.RAEL.Count).Append(", ");

            foreach (KeyValuePair<Type, HashSet<Entity>> entityType in controlUnit.ControlledEntities)
            {
                description.Append(loggingEngine.GetStringRepDefaultEntityType(entityType.Key)).Append(": ,").Append(entityType.Value.Count).Append(", ");
            }

            foreach (KeyValuePair<string, List<Activity>> activityType in controlUnit.CurrentActivitiesPerType)
            {
                description.Append(activityType.Key).Append(": ,").Append(activityType.Value.Count).Append(", ");
            }

            description.AppendLine();

            foreach (ControlUnit child in controlUnit.ChildControlUnits)
            {
                LogControlUnitState childState = new LogControlUnitState(timeStamp, loggingEngine, child);
                description.Append(childState.GetDescription());
            }

            LogState = description.ToString();
        }

        /// <summary>
        /// String state of the control unit
        /// </summary>
        public string LogState { get; }

        /// <summary>
        /// Returns the state description
        /// </summary>
        /// <returns></returns>
        public string GetDescription()
        {
            return LogState;
        }
    }
}
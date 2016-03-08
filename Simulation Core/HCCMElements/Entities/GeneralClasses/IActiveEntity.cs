using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Interface that defines an active entity
    /// </summary>
    public interface IActiveEntity
    {
        /// <summary>
        /// Gets the activites the entity is currently engaged in.
        /// </summary>
        /// <returns>List of current activities</returns>
        List<Activity> GetCurrentActivities();

        /// <summary>
        /// Stops all current activities.
        /// </summary>
        /// <param name="time">Time where activities are stopped</param>
        /// <param name="simEngine">SimEngine that handles simulation execution</param>
        void StopCurrentActivities(DateTime time, ISimulationEngine simEngine);

        /// <summary>
        /// Stops a possible waiting actitity
        /// </summary>
        /// <param name="time"></param>
        /// <param name="simEngine"></param>
        void StopWaitingActivity(DateTime time, ISimulationEngine simEngine);

        /// <summary>
        /// Gets the start event of a waiting activity for the entity.
        /// </summary>
        /// <param name="waitingArea">A waiting area for the activity, per default equal to null.</param>
        /// <returns>Start event of a waiting activity</returns>
        Event StartWaitingActivity(IDynamicHoldingEntity waitingArea = null);

        /// <summary>
        /// Adds an activity to the current activities of entities.
        /// </summary>
        /// <param name="activity">Activity to add</param>
        void AddActivity(Activity activity);

        /// <summary>
        /// Removes an activity to the current activities of entities.
        /// </summary>
        /// <param name="activity">Activity to remove</param>
        void RemoveActivity(Activity activity);

        /// <summary>
        /// Checks if the entity is in a waiting activity, and no other activity
        /// </summary>
        /// <returns>Returns true if the entity is waiting and in no óther activity</returns>
        bool IsWaiting();

        /// <summary>
        /// Checks if the entity is in a waiting activity or in a pre-emptable activity(s).
        /// </summary>
        /// <returns>Returns true if the entity is only in a waiting activity or pre-emptable activities.</returns>
        bool IsWaitingOrPreEmptable();

        /// <summary>
        /// Checks if the entity is only in one activity of the type specified by the string identifier.
        /// </summary>
        /// <param name="activity">String identifier of activity</param>
        /// <returns>Returns true if entity is only in an activity of the passed type</returns>
        bool IsInOnlyActivity(string activity);
    }
}

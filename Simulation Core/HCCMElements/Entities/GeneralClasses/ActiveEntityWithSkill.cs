using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimulationCore.HCCMElements
{
    public abstract class ActiveEntityWithSkill : EntityWithSkill, IActiveEntity
    {
        #region Constructor

        public ActiveEntityWithSkill(int identifier, SkillSet skillSet = null)
            : base(identifier, skillSet)
        {
            _currentActivities = new List<Activity>();
        } // end of ActiveEntity

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region CurrentActivities

        private List<Activity> _currentActivities;

        private List<Activity> CurrentActivities
        {
            get
            {
                return _currentActivities;
            }
            set
            {
                _currentActivities = value;
            }
        } // end of CurrentActivities

        #endregion CurrentActivities

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region GetCurrentActivities

        public List<Activity> GetCurrentActivities()
        {
            return _currentActivities;
        } // end of GetCurrentActivities

        #endregion GetCurrentActivities

        #region AddActivity

        public void AddActivity(Activity activity)
        {
            _currentActivities.Add(activity);
        } // end of AddActivity

        #endregion AddActivity

        #region RemoveActivity

        public void RemoveActivity(Activity activity)
        {
            _currentActivities.Remove(activity);
        } // end of RemoveActivity

        #endregion RemoveActivity

        #region StopCurrentActivities

        public void StopCurrentActivities(DateTime time, ISimulationEngine simEngine)
        {
            while (CurrentActivities.Count > 0)
            {
                CurrentActivities.First().EndEvent.Trigger(time, simEngine);
            } // end while
        } // end of StopCurrentActivites

        #endregion StopCurrentActivities

        #region StopWaitingActivity

        public void StopWaitingActivity(DateTime time, ISimulationEngine simEngine)
        {
            if (CurrentActivities.Count == 1
                    && CurrentActivities.First().GetType() == typeof(ActivityWait))
                StopCurrentActivities(time, simEngine);
        } // end of StopCurrentActivites

        #endregion StopWaitingActivity

        #region StartWaitingActivity

        public Event StartWaitingActivity(IDynamicHoldingEntity waitingArea = null)
        {
            ActivityWait wait = new ActivityWait(this.ParentControlUnit, this, waitingArea);
            return wait.StartEvent;
        } // end of

        #endregion StartWaitingActivity

        #region IsWaiting

        public bool IsWaiting()
        {
            return CurrentActivities.Count == 1 && CurrentActivities.First().GetType() == typeof(ActivityWait);
        } // end of IsWaiting

        #endregion IsWaiting

        #region IsInOnlyActivity

        public bool IsInOnlyActivity(Activity activity)
        {
            return CurrentActivities.Count == 1 && (CurrentActivities.First()) == activity;
        } // end of DoesOnlyActivity

        public bool IsInOnlyActivity(string activity)
        {
            return CurrentActivities.Count == 1 && ((CurrentActivities.First()).ActivityName) == activity;
        } // end of DoesOnlyActivity

        #endregion IsInOnlyActivity

        #region IsWaitingOrPreEmptable

        public bool IsWaitingOrPreEmptable()
        {
            if (IsInOnlyActivity("ActivityWait"))
            {
                return true;
            }
            else
            {
                foreach (Activity act in CurrentActivities)
                {
                    if (!act.PreEmptable())
                        return false;
                } // end foreach

                return true;
            } // end if
        } // end of IsWaitingOrPreEmptable

        #endregion IsWaitingOrPreEmptable
    } // end of ActiveEntity
}
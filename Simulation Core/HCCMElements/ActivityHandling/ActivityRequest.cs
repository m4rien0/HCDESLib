using SimulationCore.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationCore.HCCMElements
{
    /// <summary>
    /// Base class for activity requests
    /// </summary>
    public class ActivityRequest
    {
        #region Constructor

        /// <summary>
        /// Base constructor, takes an array of entities that request an activity,
        /// the type of activity requested and time
        /// </summary>
        /// <param name="activity">What activity is requested</param>
        /// <param name="origin">Who requests it</param>
        /// <param name="time">At what time the request is made</param>
        public ActivityRequest(string activity, Entity[] origin, DateTime time)
        {
            _activity = activity;
            _timeRequested = time;
            _origin = origin;
        } // end of ActivityRequest

        /// <summary>
        /// Base constructor, takes an array of entities that request an activity,
        /// the type of activity requested and time
        /// </summary>
        /// <param name="activity">What activity is requested</param>
        /// <param name="origin">Who requests it</param>
        /// <param name="time">At what time the request is made</param>
        public ActivityRequest(string activity, Entity origin, DateTime time)
        {
            _activity = activity;
            _timeRequested = time;
            _origin = Helpers<Entity>.ToArray(origin);;
        } // end of ActivityRequest

        #endregion

        #region Activity

        private string _activity;

        public string Activity
        {
            get
            {
                return _activity;
            }
            set
            {
                _activity = value;
            }
        } // end of Activity

        #endregion
        
        #region TimeRequested

        private DateTime _timeRequested;

        public DateTime TimeRequested
        {
            get
            {
                return _timeRequested;
            }
        } // end of TimeRequested

        #endregion

        #region Origin

        private Entity[] _origin;

        public Entity[] Origin
        {
            get
            {
                return _origin;
            }
        } // end of Origin

        #endregion

    }
}
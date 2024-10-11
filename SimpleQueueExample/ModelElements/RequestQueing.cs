using SimulationCore.HCCMElements;
using System;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Request object for being served, has flag if a queue has already been assigned, so is used for queue assignment
    /// and for service request
    /// </summary>
    public class RequestQueing : ActivityRequest
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="type">Type of request, eiter "WaitInQueue" or "GetServed"</param>
        /// <param name="client">Client filing request</param>
        /// <param name="time">Time the request is filed</param>
        public RequestQueing(string type, EntityClient client, DateTime time)
            : base(type, client.ToArray(), time)
        {
            _client = client;
        } // end of RequestBeAbsent

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Member
        //--------------------------------------------------------------------------------------------------

        #region Client

        private EntityClient _client;

        /// <summary>
        /// Client filing request
        /// </summary>
        public EntityClient Client
        {
            get
            {
                return _client;
            }
        } // end of Client

        #endregion Client

        #region QueueAssigned

        private EntityQueue _queueAssigned;

        /// <summary>
        /// True if queue has been assigned
        /// </summary>
        public EntityQueue QueueAssigned
        {
            get
            {
                return _queueAssigned;
            }
            set
            {
                _queueAssigned = value;
            }
        } // end of QueueAssigned

        #endregion QueueAssigned
    } // end of RequestQueing
}
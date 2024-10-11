using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Sample queuing control, basically incoming clients are assinged to queues with minimum length
    /// and clients are selected from front of queues by FIFO (so FIFO within a single queue and FIFO
    /// of queue fronts)
    /// </summary>
    public class ControlUnitQueuingModel : ControlUnit
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, entities are added to model
        /// </summary>
        /// <param name="name">Name of control</param>
        /// <param name="parentControlUnit">Root control unit, null in this example</param>
        /// <param name="parentSimulationModel">Simulation model control belongs to</param>
        /// <param name="numberQueues">Number queues to be modeled</param>
        /// <param name="numberServers">Number servers to be modeled</param>
        public ControlUnitQueuingModel(string name,
                           ControlUnit parentControlUnit,
                           SimulationModel parentSimulationModel,
                           int numberQueues,
                           int numberServers)
            : base(name, parentControlUnit, parentSimulationModel)
        {
            _queues = new List<EntityQueue>();
            _servers = new List<EntityServer>();

            for (int i = 0; i < numberQueues; i++)
            {
                EntityQueue newQueue = new EntityQueue();
                AddEntity(newQueue);
                Queues.Add(newQueue);
            } // end for

            for (int i = 0; i < numberServers; i++)
            {
                EntityServer newServer = new EntityServer();
                AddEntity(newServer);
                Servers.Add(newServer);
            } // end for
        } // end of

        #endregion Constructor

        #region Initialize

        /// <summary>
        /// Arrival stream of clients is initialized
        /// </summary>
        /// <param name="startTime">Start time of simulation</param>
        /// <param name="simEngine">End time of simulation</param>
        protected override void CustomInitialize(DateTime startTime, ISimulationEngine simEngine)
        {
            EntityClient nextClient = new EntityClient();

            EventClientArrival nextClientArrival = new EventClientArrival(this, nextClient);

            double arrivalTimeMinutes = ((SimulationModelQueuing)ParentSimulationModel).ArrivalTime;

            simEngine.AddScheduledEvent(nextClientArrival, startTime
                + TimeSpan.FromMinutes(Distributions.Instance.Exponential(arrivalTimeMinutes)));
        } // end of CustomInitialize

        #endregion Initialize

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region Queues

        private List<EntityQueue> _queues;

        /// <summary>
        /// Number queues to be modeled
        /// </summary>
        public List<EntityQueue> Queues
        {
            get
            {
                return _queues;
            }
        } // end of Queues

        #endregion Queues

        #region Servers

        private List<EntityServer> _servers;

        /// <summary>
        /// Number servers to be modeled
        /// </summary>
        public List<EntityServer> Servers
        {
            get
            {
                return _servers;
            }
        } // end of Servers

        #endregion Servers

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region PerformCustomRules

        /// <summary>
        /// Custom rule set, basically incoming clients are assinged to queues with minimum length
        /// and clients are selected from front of queues by FIFO (so FIFO within a single queue and FIFO
        /// of queue fronts)
        /// </summary>
        /// <param name="time">Time rules are executed</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <returns></returns>
        protected override bool PerformCustomRules(DateTime time, ISimulationEngine simEngine)
        {
            #region PlacedInQueue

            List<RequestQueing> waitInQueueRequests = RAEL.Where(p => p.Activity == "WaitInQueue").Cast<RequestQueing>().ToList();

            foreach (RequestQueing request in waitInQueueRequests)
            {
                // determine the smallest QueueLength
                int minQueueLength = Queues.Select(p => p.HoldedEntities.Count).Aggregate((curmin, x) => ((x) < curmin ? x : curmin));

                // select first queue with minimum length
                EntityQueue queue = Queues.Where(p => p.HoldedEntities.Count == minQueueLength).First();
                queue.HoldedEntities.Add(request.Client);

                RemoveRequest(request);
                RequestQueing newReq = new RequestQueing("GetServed", request.Client, time);
                newReq.QueueAssigned = queue;
                AddRequest(newReq);
            } // end foreach

            #endregion PlacedInQueue

            #region GetServerd

            List<RequestQueing> getServedRequests = RAEL.Where(p => p.Activity == "GetServed").Cast<RequestQueing>().ToList();

            while (getServedRequests.Count > 0 && Servers.Where(p => p.IsIdle).Count() > 0)
            {
                RequestQueing earliestRequest = getServedRequests.Aggregate((curmin, x) => (curmin == null || (x.TimeRequested) < curmin.TimeRequested ? x : curmin));

                ActivityGetServed newService = new ActivityGetServed(this, earliestRequest.Client, Servers.Where(p => p.IsIdle).First());
                newService.StartEvent.Trigger(time, simEngine);

                RemoveRequest(earliestRequest);
                earliestRequest.QueueAssigned.HoldedEntities.Remove(earliestRequest.Client);

                getServedRequests.Remove(earliestRequest);
            } // end while

            #endregion GetServerd

            return false;
        } // end of PerformCustomRules

        #endregion PerformCustomRules

        #region EntityEnterControlUnit

        public override Event EntityEnterControlUnit(DateTime time, SimulationCore.SimulationClasses.ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityEnterControlUnit

        #endregion EntityEnterControlUnit

        #region EntityLeaveControlUnit

        public override void EntityLeaveControlUnit(DateTime time, SimulationCore.SimulationClasses.ISimulationEngine simEngine, Entity entity, IDelegate originDelegate)
        {
            throw new NotImplementedException();
        } // end of EntityLeaveControlUnit

        #endregion EntityLeaveControlUnit
    } // end of ControlUnitQueuingModel
}
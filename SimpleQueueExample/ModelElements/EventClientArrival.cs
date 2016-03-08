using SimulationCore.HCCMElements;
using SimulationCore.MathTool.Distributions;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Event that is triggered for a client arrival in the model
    /// </summary>
    public class EventClientArrival : Event
    {
        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="parentControlUnit">Parent control</param>
        /// <param name="patient">Client that arrives</param>
        public EventClientArrival(ControlUnit parentControlUnit, EntityClient client)
            : base(EventType.Standalone, parentControlUnit)
        {
            _client = client;
        } // end of Event

        #endregion

        //--------------------------------------------------------------------------------------------------
        // State Change
        //--------------------------------------------------------------------------------------------------

        #region StateChange

        /// <summary>
        /// Overriden state change of the event. Request for service is made, next client arrival is scheduled
        /// </summary>
        /// <param name="time">Time the client arrives</param>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        protected override void StateChange(DateTime time, ISimulationEngine simEngine)
        {
            // next arrival is scheduled

            EntityClient nextClient = new EntityClient();

            EventClientArrival nextClientArrival = new EventClientArrival(ParentControlUnit, nextClient);

            double arrivalTimeMinutes = ((SimulationModelQueuing)ParentControlUnit.ParentSimulationModel).ArrivalTime;

            simEngine.AddScheduledEvent(nextClientArrival, time
                + TimeSpan.FromMinutes(Distributions.Instance.Exponential(arrivalTimeMinutes)));

            ParentControlUnit.AddRequest(new RequestQueing("WaitInQueue", Client, time));

        } // end of Trigger

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Client

        private EntityClient _client;

        /// <summary>
        /// Arriving client
        /// </summary>
        public EntityClient Client
        {
            get
            {
                return _client;
            }
        } // end of Client

        #endregion

        #region AffectedEntites

        /// <summary>
        /// Affected entities only consist of the patient
        /// </summary>
        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] { Client };
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "EventClientArrival";
        } // end of ToString

        #endregion

        #region Clone

        override public Event Clone()
        {
            return new EventClientArrival(ParentControlUnit, Client);
        } // end of Clone

        #endregion

    } // end of EventClientArrival
}

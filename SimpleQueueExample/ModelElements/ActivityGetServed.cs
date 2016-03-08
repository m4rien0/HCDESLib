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
    /// Sample activity to represent a service of a client at the server
    /// </summary>
    public class ActivityGetServed : Activity
    {
        #region Constructor

        /// <summary>
        /// Basic Construcor
        /// </summary>
        /// <param name="parentControlUnit">Parent control</param>
        /// <param name="client">Client to be served</param>
        /// <param name="server">Server performing service</param>
        public ActivityGetServed(ControlUnit parentControlUnit,
            EntityClient client,
            EntityServer server)
            : base(parentControlUnit, "ActivityGetServed", true)
        {
            _client = client;
            _server = server;
        } // end of Activity

        #endregion

        #region Name

        public static string Name = "ActivityGetServed";

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Affected Entities
        //--------------------------------------------------------------------------------------------------

        #region Client

        private EntityClient _client;

        /// <summary>
        /// Client to be served
        /// </summary>
        public EntityClient Client
        {
            get
            {
                return _client;
            }
        } // end of Client

        #endregion

        #region Server

        private EntityServer _server;

        /// <summary>
        /// Server performing service
        /// </summary>
        public EntityServer Server
        {
            get
            {
                return _server;
            }
        } // end of Server

        #endregion

        #region AffectedEntites

        public override Entity[] AffectedEntities
        {
            get
            {
                return new Entity[] {Client, Server};
            }
        } // end of AffectedEntities

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Events
        //--------------------------------------------------------------------------------------------------

        #region TriggerStartEvent
        /// <summary>
        /// Overrides the state change at start. Server is not idle, and end event is triggered.
        /// </summary>
        override public void StateChangeStartEvent(DateTime time, ISimulationEngine simEngine)
        {
            double serviceTimeMinutes = ((SimulationModelQueuing)ParentControlUnit.ParentSimulationModel).ServiceTime;

            Server.IsIdle = false;
            simEngine.AddScheduledEvent(EndEvent, time + TimeSpan.FromMinutes(Distributions.Instance.Exponential(serviceTimeMinutes)));
        } // end of TriggerStartEvent

        #endregion

        #region TriggerEndEvent

        /// <summary>
        /// Overrides the state change at end. Server is set idle again
        /// </summary>
        override public void StateChangeEndEvent(DateTime time, ISimulationEngine simEngine)
        {
            Server.IsIdle = true;
        } // end of TriggerEndEvent

        #endregion

        #region ToString

        /// <summary>
        /// Creates string of activity name and representation of the server and client
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}: {1},{2}", Name, Client.ToString(), Server.ToString());
        } // end of ToString

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region Clone

        public override Activity Clone()
        {
            return new ActivityGetServed(ParentControlUnit, Client, Server);
        } // end of Clone

        #endregion

    } // end of ActivityGetServed
}

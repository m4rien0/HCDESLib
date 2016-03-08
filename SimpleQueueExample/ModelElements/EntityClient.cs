using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Entity representing a client to be served in queuing model
    /// </summary>
    public class EntityClient : Entity
    {
        public static int RunningID = 0;

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        public EntityClient()
            : base(RunningID++)
        {
        } // end of EntityClient
        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Client: " + Identifier.ToString();
        } // end of

        #endregion

        #region Clone

        public override Entity Clone()
        {
            return new EntityQueue();

        } // end of Clone

        #endregion

    } // end of EntityClient
}

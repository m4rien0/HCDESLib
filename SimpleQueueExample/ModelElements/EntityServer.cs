using SimulationCore.HCCMElements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleQueueExample.ModelElements
{
    /// <summary>
    /// Server entity, has an idle flag to signalize posssible new clients to be served
    /// </summary>
    public class EntityServer : Entity
    {
        public static int RunningID = 0;

        #region Constructor

        public EntityServer()
            : base(RunningID++)
        {
            _isIdle = true;
        } // end of EntityClient
        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region IsIdle

        private bool _isIdle;

        /// <summary>
        /// False if currently performing a service, true else
        /// </summary>
        public bool IsIdle
        {
            get
            {
                return _isIdle;
            }
            set
            {
                _isIdle = value;
            }
        } // end of IsIdle

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Server: " + Identifier.ToString();
        } // end of

        #endregion

        #region Clone

        public override Entity Clone()
        {
            return new EntityQueue();

        } // end of Clone

        #endregion

    } // end of EntityServer
}

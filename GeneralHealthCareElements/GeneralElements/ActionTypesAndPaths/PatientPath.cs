using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneralHealthCareElements.GeneralClasses.ActionTypesAndPaths
{
    /// <summary>
    /// Abstract base class of patient paths representing a list of actions and
    /// enabling update and action initializing mehtods
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PatientPath<T> where T : ActionTypeClass 
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="actions">List of actions that have to be performed along path</param>
        /// <param name="parentPatient">Patient for whom the path is generated</param>
        public PatientPath(List<T> actions, EntityPatient parentPatient)
        {
            _actions = actions;
            _currentActionType = actions.First();
            _currentPosition = 0;
            _parentPatient = parentPatient;
        } // end of PatientPath
    

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region ParentPatient

        private EntityPatient _parentPatient;

        /// <summary>
        /// Patient that is moving along the path
        /// </summary>
        public EntityPatient ParentPatient
        {
            get
            {
                return _parentPatient;
            }
            set
            {
                _parentPatient = value;
            }
        } // end of ParentPatient

        #endregion

        #region Actions

        private List<T> _actions;

        /// <summary>
        /// List of actions to be performed along the path
        /// </summary>
        public List<T> Actions
        {
            get
            {
                return _actions;
            }
            set
            {
                _actions = value;
            }
        } // end of Actions

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Position in path 
        //--------------------------------------------------------------------------------------------------

        #region CurrentActionType

        private T _currentActionType;

        /// <summary>
        /// Action type to be performed next
        /// </summary>
        public T CurrentActionType
        {
            get
            {
                return _currentActionType;
            }
        } // end of CurrentActionType

        #endregion

        #region CurrentPosition

        private int _currentPosition;

        /// <summary>
        /// Current position in the path
        /// </summary>
        private int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
            }
        } // end of CurrentPosition

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region GetCurrentActionType

        public T GetCurrentActionType()
        {
            return _currentActionType;
        } // end of GetNextActionType

        #endregion

        #region UpdateNextAction

        public void UpdateNextAction()
        {
            _currentPosition++;

            _currentActionType = Actions[CurrentPosition];
        } // end of UpdateNextAction

        #endregion

        #region TakeNextAction

        /// <summary>
        /// Abstract method that defines how next actions are taken
        /// </summary>
        /// <param name="simEngine">SimEngine responsible for simulation execution</param>
        /// <param name="currentEvent">Event after which next action is taken</param>
        /// <param name="time">Time next action is taken</param>
        /// <param name="parentControlUnit">Control unit that hosts the next action</param>
        /// <returns>Returns true if Patient goes in an waiting activitiy before next action, else false </returns>
        public abstract bool TakeNextAction(
            ISimulationEngine simEngine,
            Event currentEvent,
            DateTime time,
            ControlUnit parentControlUnit);

        #endregion

    } // end of PatientPath
}

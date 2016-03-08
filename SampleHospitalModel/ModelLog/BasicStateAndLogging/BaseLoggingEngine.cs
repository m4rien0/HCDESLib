using GeneralHealthCareElements.BookingModels;
using GeneralHealthCareElements.Entities;
using SimulationCore.HCCMElements;
using SimulationCore.Interfaces;
using SimulationCore.SimulationClasses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SampleHospitalModel.ModelLog
{
    /// <summary>
    /// Example of a logging engine
    /// </summary>
    public class BaseLoggingEngine : IModelLog
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor 
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Constructor that sets string representations for entities for logging purposes
        /// </summary>
        /// <param name="parentSimulationModel">Simulation model that is executed</param>
        public BaseLoggingEngine(SimulationModel parentSimulationModel)
        {
            _parentSimulationModel = parentSimulationModel;
            _simulationResult = new List<SimulationState>();

            _defaultEntityStringReps = new Dictionary<Type, string>();

            _defaultEntityStringReps.Add(typeof(EntityDoctor), "EntityDoctor");
            _defaultEntityStringReps.Add(typeof(EntityPatient), "EntityPatient");
            _defaultEntityStringReps.Add(typeof(EntityNurse), "EntityNurse");
            _defaultEntityStringReps.Add(typeof(EntityTreatmentFacility), "EntitiyTreatmentFacility");
            _defaultEntityStringReps.Add(typeof(EntityWaitingArea), "EntityWaitingArea");
            _defaultEntityStringReps.Add(typeof(EntityMultiplePatientTreatmentFacility), "EntityMultiplePatientTreatmentFacility");
            _defaultEntityStringReps.Add(typeof(EntityWaitingListSchedule), "EntityWaitingListSchedule");
            _defaultEntityStringReps.Add(typeof(EntityPath), "EntityPath");
            //_defaultEntityStringReps.Add(typeof(EntityBreastCancerPatient), "EntityBreastCancerPatient");
            _defaultEntityStringReps.Add(typeof(EntitySingleBookingModelWaitingList), "EntitySingleBookingModelWaitingList");

        } // end of BaseLoggingEngine

        #endregion

        #region ParentSimulationModel

        private SimulationModel _parentSimulationModel;

        /// <summary>
        /// Simulation model that is executed
        /// </summary>
        public SimulationModel ParentSimulationModel
        {
            get
            {
                return _parentSimulationModel;
            }
            set
            {
                _parentSimulationModel = value;
            }
        } // end of ParentSimulationModel

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region CreateCurrentState

        /// <summary>
        /// Creates a LogControlUnitState and stores it in the simulation result
        /// </summary>
        /// <param name="currentEvents">Events triggered at current time</param>
        /// <param name="time">Current time of simulation execution</param>
        public void CreateCurrentState(List<Event> eventsTriggered, DateTime time)
        {
            LogControlUnitState currentState = new LogControlUnitState(time, this, ParentSimulationModel.RootControlUnit);

            SimulationResult.Add(new SimulationState(currentState, eventsTriggered, time));
        } // end of StoreCurrentState

        #endregion

        #region GetStringRepDefaultEntityType

        private Dictionary<Type, string> _defaultEntityStringReps = new Dictionary<Type, string>();

        /// <summary>
        /// Returns the string representation of an entity type
        /// </summary>
        /// <param name="defaultEntitiyType">The type of the entity</param>
        /// <returns></returns>
        public string GetStringRepDefaultEntityType(Type defaultEntitiyType)
        {
            return _defaultEntityStringReps[defaultEntitiyType];
        } // end of

        #endregion

        #region GetCurrentStateRepresentation

        /// <summary>
        /// Currently not implemented
        /// </summary>
        /// <returns>String that represents the current model state</returns>
        public string GetCurrentStateRepresentation()
        {
            throw new NotImplementedException();
        } // end of GetCurrentStateRepresentation

        #endregion

        #region GetSimulationResult

        /// <summary>
        /// Creates the simulation result by creating a txt file with the event log
        /// </summary>
        public void CreateSimulationResult()
        {
            StreamWriter outfile = new StreamWriter("Output.txt");

            for (int i = 0; i < SimulationResult.Count; i++)
            {
                outfile.Write(SimulationResult[i].GetDescription());
            } // end for

            outfile.Close();
        }

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region SimulationResult

        private List<SimulationState> _simulationResult;

        /// <summary>
        /// List of states that represent the simulation result
        /// </summary>
        public List<SimulationState> SimulationResult
        {
            get
            {
                return _simulationResult;
            }
            set
            {
                _simulationResult = value;
            }
        } // end of SimulationResult

        #endregion





       
    } // end of BaseLoggingEngine
}

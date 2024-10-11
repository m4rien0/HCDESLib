using SimulationCore.HCCMElements;
using SimulationCore.MathTool.GeometricClasses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneralHealthCareElements.Entities
{
    /// <summary>
    /// Class for an entity that represents a travel path between two control units
    /// </summary>
    public class EntityPath : Entity
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="identifier">Entity identifier</param>
        /// <param name="startControl">Start control unit of move</param>
        /// <param name="endControl">Destination control unit of move</param>
        /// <param name="points">Geometric points of move</param>
        public EntityPath(
            int identifier,
            ControlUnit startControl,
            ControlUnit endControl,
            MyPoint[] points)
            : base(identifier)
        {
            _startControlUnit = startControl;
            _endControlUnit = endControl;
            _hostedEntities = new List<Entity>();

            _pathLine = new PolygonialLine(points.ToArray());
        } // end of identifier

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region StartControlUnit

        private ControlUnit _startControlUnit;

        /// <summary>
        /// Start control unit of move
        /// </summary>
        public ControlUnit StartControlUnit
        {
            get
            {
                return _startControlUnit;
            }
        } // end of StartControlUnit

        #endregion StartControlUnit

        #region EndControlUnit

        private ControlUnit _endControlUnit;

        /// <summary>
        /// Destination control unit of move
        /// </summary>
        public ControlUnit EndControlUnit
        {
            get
            {
                return _endControlUnit;
            }
        } // end of EndControlUnit

        #endregion EndControlUnit

        #region HostedEntities

        private List<Entity> _hostedEntities;

        /// <summary>
        /// Entities currently moving on the path
        /// </summary>
        public List<Entity> HostedEntities
        {
            get
            {
                return _hostedEntities;
            }
        } // end of HostedEntities

        #endregion HostedEntities

        #region PathLine

        private PolygonialLine _pathLine;

        /// <summary>
        /// Geometric representation of path
        /// </summary>
        public PolygonialLine PathLine
        {
            get
            {
                return _pathLine;
            }
        } // end of PathLine

        #endregion PathLine

        //--------------------------------------------------------------------------------------------------
        // Methods
        //--------------------------------------------------------------------------------------------------

        #region ToString

        public override string ToString()
        {
            return "Path: (" + StartControlUnit.ToString() + "," + EndControlUnit.ToString() + ")";
        } // end of ToString

        #endregion ToString

        #region Clone

        public override Entity Clone()
        {
            throw new NotImplementedException();
        } // end of Clone

        #endregion Clone
    } // end of Entity
}
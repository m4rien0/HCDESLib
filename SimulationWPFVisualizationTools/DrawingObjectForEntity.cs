using SimulationCore.HCCMElements;
using System.Windows;
using WPFVisualizationBase;

namespace SimulationWPFVisualizationTools
{
    /// <summary>
    /// Class to link simulation entities with drawing objects
    /// </summary>
    public abstract class DrawingObjectForEntity : DrawingObject
    {
        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor
        /// </summary>
        /// <param name="startPosition">Start position of object</param>
        /// <param name="correspondingEntity">Entity that is visualized with object</param>
        public DrawingObjectForEntity(Point startPosition,
                                      Entity correspondingEntity) : base(startPosition)
        {
            _correspondingEntity = correspondingEntity;

            DrawingShape.ToolTip = correspondingEntity.ToString();
        } // end of DrawingObject

        #endregion Constructor

        //--------------------------------------------------------------------------------------------------
        // Members
        //--------------------------------------------------------------------------------------------------

        #region CorrespondingEntity

        private Entity _correspondingEntity;

        /// <summary>
        /// Entity that is visualized with object
        /// </summary>
        public Entity CorrespondingEntity
        {
            get
            {
                return _correspondingEntity;
            }
        } // end of CorrespondingEntity

        #endregion CorrespondingEntity
    } // end of DrawingObjectForEntity
}
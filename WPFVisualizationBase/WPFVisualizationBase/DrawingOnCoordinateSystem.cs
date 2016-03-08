using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace WPFVisualizationBase
{
    /// <summary>
    /// A basic drawing system to place drawing objects on a coordinate system nested in a canvas.
    /// Drawing coordinates are handled in the usual mathematical sense with (0,0) origin. The WPF 
    /// canvas feature that y-coordinates start at the top of a canvas is avoided by a default
    /// transform that translates user specific transforms in a canvas transform
    /// </summary>
    public class DrawingOnCoordinateSystem : Canvas
    {

        //--------------------------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------------------------

        #region Constructor

        /// <summary>
        /// Basic constructor, adds mouse events for wheel and move, initializes default transform
        /// for coordinate system drawing
        /// </summary>
        /// <param name="startXShift">Defines at which x-coordinate the origin should be first visualized
        /// on the canvas</param>
        /// <param name="startYShift">Defines at which y-coordinate the origin should be first visualized
        /// on the canvas</param>
        /// <param name="fixedOutline">True if no mouse events (scaling and transform) are added</param>
        public DrawingOnCoordinateSystem(double startXShift, double startYShift, bool fixedOutline = false)
        {

            Background = Brushes.Transparent;

            ClipToBounds = true;

            _drawingObjects = new List<DrawingObject>();

            _currentScaleFactor = 1;

            _currentShift = new Vector(0, 0);

            _coordScaleTransform = new ScaleTransform(CurrentScaleFactor, CurrentScaleFactor);
            _coordMatrixTransform = new MatrixTransform(1, 0, 0, -1, startXShift, startYShift);

            _currentTransform = new TransformGroup();

            _currentTransform.Children.Add(_coordMatrixTransform);
            _currentTransform.Children.Add(_coordScaleTransform);

            if (!fixedOutline)
            {
                MouseWheel += new System.Windows.Input.MouseWheelEventHandler(DrawingCanvas_MouseWheel);
                MouseMove += CoordinateSystemMouseMove;
            } // end if

            UpdateObjectsRendering();
        } // end of DrawingObject

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Members 
        //--------------------------------------------------------------------------------------------------

        #region DrawingObjects

        protected List<DrawingObject> _drawingObjects;

        /// <summary>
        /// Currently visualized drawing objects
        /// </summary>
        public IReadOnlyList<DrawingObject> DrawingObjects
        {
            get
            {
                return _drawingObjects;
            }
        } // end of DrawingObjects

        #endregion

        #region CurrentScaleFactor

        protected double _currentScaleFactor;

        /// <summary>
        /// Current zoom factor (changable via mouse wheel)
        /// </summary>
        public double CurrentScaleFactor
        {
            get
            {
                return _currentScaleFactor;
            }
        } // end of CurrentScaleFactor

        #endregion

        #region CurrentShift

        protected Vector _currentShift;

        /// <summary>
        /// Current shift vector of coordinate system origin on canvas
        /// </summary>
        public Vector CurrentShift
        {
            get
            {
                return _currentShift;
            }
        } // end of CurrentShift

        #endregion

        #region CoordScaleTransform

        protected ScaleTransform _coordScaleTransform;

        /// <summary>
        /// Transform object of current zoom state
        /// </summary>
        protected ScaleTransform CoordScaleTransform
        {
            get
            {
                return _coordScaleTransform;
            }
        } // end of CoordScaleTransform

        #endregion

        #region CoordMatrixTransform

        protected MatrixTransform _coordMatrixTransform;

        /// <summary>
        /// Transform of current shift state (with respect to canvas position and coordinate origin)
        /// </summary>
        protected MatrixTransform CoordMatrixTransform
        {
            get
            {
                return _coordMatrixTransform;
            }
        } // end of CoordMatrixTransform

        #endregion

        #region CurrentTransform

        protected TransformGroup _currentTransform;

        /// <summary>
        /// Aggregated current transform (scale and shift)
        /// </summary>
        public TransformGroup CurrentTransform
        {
            get
            {
                return _currentTransform;
            }
        } // end of CurrentTransform

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Methods 
        //--------------------------------------------------------------------------------------------------

        #region AddObject

        /// <summary>
        /// Adds a drawing object to the drawing system
        /// </summary>
        /// <param name="objectToAdd">Object to be added</param>
        public void AddObject(DrawingObject objectToAdd)
        {
            objectToAdd.ParentCoordinateSystem = this;
            _drawingObjects.Add(objectToAdd);
            Children.Add(objectToAdd.DrawingShape);
            UpdateObjectsRendering();
        } // end of AddObject

        #endregion

        #region RemoveObject

        /// <summary>
        /// Removes a drawing object from the drawing system
        /// </summary>
        /// <param name="objectToAdd">Object to be removed</param>
        public void RemoveObject(DrawingObject objectToRemove)
        {
            objectToRemove.ParentCoordinateSystem = null;
            _drawingObjects.Remove(objectToRemove);
            Children.Remove(objectToRemove.DrawingShape);
            UpdateObjectsRendering();
        } // end of RemoveObject

        #endregion

        #region ClearSystem

        /// <summary>
        /// Removes all drawing objects from the system
        /// </summary>
        public void ClearSystem()
        {
            while (DrawingObjects.Count > 0)
            {
                DrawingObject objectToRemove = DrawingObjects.First();

                objectToRemove.ParentCoordinateSystem = null;
                _drawingObjects.Remove(objectToRemove);
                Children.Remove(objectToRemove.DrawingShape);
            } // end while

            UpdateObjectsRendering();

        } // end of ClearSystem

        #endregion

        //--------------------------------------------------------------------------------------------------
        // Event Handling
        //--------------------------------------------------------------------------------------------------

        #region CoordinateSystemMouseWheel

        /// <summary>
        /// Event handler for mouse wheel events, updates the scale transform, scaling is done
        /// with respect to the center of the canvas (not the coordinate origin)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawingCanvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            float factor = (float)Math.Pow(1.01d, e.Delta / 10d);
            _currentScaleFactor *= factor;

            Point p = Mouse.GetPosition(this);

            _coordScaleTransform = new ScaleTransform(_currentScaleFactor, _currentScaleFactor, ActualWidth / 2, ActualHeight / 2);

            UpdateObjectsRendering();
        } // end of CoordinateSystemMouseWheel

        #endregion

        #region CoordinateSystemMouseMove

        DateTime lastMouseMove;
        private double deltaX, deltaY;
        private double mouseX, mouseY;

        /// <summary>
        /// Event handler for mouse move events, updates the shift transform with respect to the last position
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CoordinateSystemMouseMove(object sender, MouseEventArgs e)
        {
            if (!(DateTime.Now.Ticks - lastMouseMove.Ticks > 400000))
                return;

            lastMouseMove = DateTime.Now;

            Point p = Mouse.GetPosition(this);

            if (mouseX == -1)
            {
                mouseX = p.X;
                mouseY = p.Y;
            }

            // Check how far mouse has been moved
            deltaX = (p.X - mouseX) / CurrentScaleFactor;
            deltaY = (p.Y - mouseY) / CurrentScaleFactor;

            mouseX = p.X;
            mouseY = p.Y;

            if (e.LeftButton == MouseButtonState.Pressed)
            {

                _coordMatrixTransform = new MatrixTransform(CoordMatrixTransform.Matrix.M11,
                    CoordMatrixTransform.Matrix.M12,
                    CoordMatrixTransform.Matrix.M21,
                    CoordMatrixTransform.Matrix.M22,
                    CoordMatrixTransform.Matrix.OffsetX + deltaX,
                    CoordMatrixTransform.Matrix.OffsetY + deltaY);

                UpdateObjectsRendering();

            } // end if
        } // end of CoordinateSystemMouseMove

        #endregion

        #region UpdateObjectsRendering

        /// <summary>
        /// Updates the visualization of objects after transform changes or adding/removing obhects
        /// </summary>
        public void UpdateObjectsRendering()
        {

            _currentTransform = new TransformGroup();
            _currentTransform.Children.Add(_coordMatrixTransform);
            _currentTransform.Children.Add(_coordScaleTransform);

            foreach (DrawingObject objectToDraw in DrawingObjects)
            {
                objectToDraw.UpdateRendering();
            } // end foreach

        } // end of UpdateObjectsRendering

        #endregion

    } // end of DrawingOnCoordinateSystem
}

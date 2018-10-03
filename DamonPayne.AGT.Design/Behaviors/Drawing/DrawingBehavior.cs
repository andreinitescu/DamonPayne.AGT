using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DamonPayne.AGT.Design.Behaviors.Drawing
{
    public abstract class DrawingBehavior
    {
        public DrawingBehavior(Canvas surface)
        {
            Surface = surface;
        }

        /// <summary>
        /// The surface we're drawing on
        /// </summary>
        public Canvas Surface { get; private set; }

        /// <summary>
        /// Mouse click point relative to Surface    
        /// </summary>    
        public Point StartPoint { get; set; }

        public abstract Shape StartDrawing(Point s);

        /// <summary>    
        /// Give derrived classes a chance to determine if they can or should return a valid     
        /// Shape in their current state    
        /// </summary>    
        /// <param name="g"></param>    
        /// <returns></returns>    
        public virtual bool ShouldStopDrawing(DrawingGestures g)
        {
            return (g == DrawingGestures.MouseLeftButtonUp);
        }

        /// <summary>    
        /// Stop drawing and return the final Shape    
        /// </summary>    
        /// <returns></returns> 
        public abstract Shape StopDrawing();


        /// <summary>    
        /// Pass Mouse events through to the DrawingBehavior    
        /// </summary>    
        /// <param name="e"></param>    
        public abstract void MouseMove(MouseEventArgs e);
    }

}

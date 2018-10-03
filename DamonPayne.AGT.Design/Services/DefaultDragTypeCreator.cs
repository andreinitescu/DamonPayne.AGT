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
using DamonPayne.AGT.Design.Controls;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AG.Core;

namespace DamonPayne.AGT.Design.Services
{
    public class DefaultDragTypeCreator : IDragTypeCreator
    {
        //TODO: get these from a config service?
        private static double MAX_DIMENSION = 75;

        private static double DRAG_OPACITY = .75;

        public Control CreateDragRepresentation(Type t)
        {
            //Default behavior:
            DragContainer dc = new DragContainer();
            Control c = (Control)Activator.CreateInstance(t);
            //Thanks to http://www.jeffblankenburg.com/2008/04/how-about-some-code-simple-resizing-in.html for scaling code

            double uniformScaleAmount = c.UniformScaleAmount(MAX_DIMENSION);

            if (double.IsNaN(uniformScaleAmount))
            {
                dc.LayoutRoot.Children.Add(c);
                c.Measure(new Size(MAX_DIMENSION, MAX_DIMENSION));
            }


            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = uniformScaleAmount;
            st.ScaleY = uniformScaleAmount;
            tg.Children.Add(st);
            c.RenderTransform = tg;
            
            
            c.InvalidateMeasure();            
            c.UpdateLayout();
                        
            double estimatedNewWidth = c.Width * uniformScaleAmount;
            double estimatedNewHeight = c.Height * uniformScaleAmount;

            double left = (dc.Width / 2.0D) - (estimatedNewWidth / 2.0D);
            double top = (dc.Height / 2.0D) - (estimatedNewHeight / 2.0D);
            c.SetValue(Canvas.LeftProperty, left);
            c.SetValue(Canvas.TopProperty, top);
            dc.LayoutRoot.Children.Add(c);
            dc.Opacity = DRAG_OPACITY;
            dc.LayoutRoot.Opacity = DRAG_OPACITY;
            
            return dc;
        }

    }
}

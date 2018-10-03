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
    public class RectangleDrawingBehavior : DrawingBehavior
    {
        public static readonly double SELECT_RECT_STROKE_WIDTH = 3.0;
        public static readonly double SELECT_RECT_RADIUS_X = 0.0;
        public static readonly double SELECT_RECT_RADIUS_Y = 0.0;

        public RectangleDrawingBehavior(Canvas surface) : base(surface) { }

        public Rectangle DrawingRect { get; set; }

        public override Shape StartDrawing(Point s)
        {
            StartPoint = s; 
            DrawingRect = new Rectangle { Width = 5.0, Height = 5.0, };
            ApplyShapeStyle(DrawingRect);
            DrawingRect.SetValue(Canvas.LeftProperty, s.X);
            DrawingRect.SetValue(Canvas.TopProperty, s.Y);
            Surface.Children.Add(DrawingRect);
            return DrawingRect;
        }

        public override Shape StopDrawing()
        {
            var rVal = DrawingRect;
            DrawingRect = null;
            return rVal;
        }

        public override void MouseMove(MouseEventArgs e)
        {
            Point localMousePos = e.GetPosition(Surface);
            if (StartPoint.X < localMousePos.X)
            {
                double width = localMousePos.X - StartPoint.X;
                double height = localMousePos.Y - StartPoint.Y;
                if (width > 0 && height > 0)
                {
                    DrawingRect.Width = width;
                    DrawingRect.Height = height;
                }
            }        
            else // northwest drag        
            {            
                double width = StartPoint.X - localMousePos.X;
                double height = StartPoint.Y - localMousePos.Y;
                if (width > 0 && height > 0)//need this safety here in case a resize rectangle "crosses" itself.           
                {
                    DrawingRect.Width = width;
                    DrawingRect.Height = height;
                    DrawingRect.SetValue(Canvas.LeftProperty, localMousePos.X);
                    DrawingRect.SetValue(Canvas.TopProperty, localMousePos.Y);   
                }       
            }         
        }

        public virtual void ApplyShapeStyle(Shape s)
        {
            DrawingRect.StrokeThickness = SELECT_RECT_STROKE_WIDTH;
            DrawingRect.RadiusX = SELECT_RECT_RADIUS_X;
            DrawingRect.RadiusY = SELECT_RECT_RADIUS_Y;
            DrawingRect.Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x15, 0x05, 0xFF));
            DrawingRect.Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x49, 0x59, 0xFF));
            DrawingRect.Opacity = .50;
            DrawingRect.SetValue(Canvas.ZIndexProperty, 1000);
        }

    }
}

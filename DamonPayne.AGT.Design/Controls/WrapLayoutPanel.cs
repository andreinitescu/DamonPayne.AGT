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

namespace DamonPayne.AGT.Design.Controls
{
    /// <summary>
    /// A WrapPanel that respects RenderTransform for it's children.  This is outside the usual Silverlight rendering pipeline...
    /// </summary>
    public class WrapLayoutPanel : Panel
    {
        public WrapLayoutPanel()
        {
        }


        protected override Size ArrangeOverride(Size finalSize)
        {
            double xPos, yPos;
            xPos = yPos = 0.0;

            foreach (var child in Children)
            {
                var desired = child.DesiredSize;
                if (xPos + desired.Width > finalSize.Width)
                {
                    xPos = 0;
                    yPos += desired.Height;
                    Rect r = new Rect(xPos, yPos, desired.Width, desired.Height);
                    child.Arrange(r);
                }
                else
                {
                    Rect r = new Rect(xPos, yPos, desired.Width, desired.Height);
                    child.Arrange(r);
                    xPos += desired.Width;
                }

            }
            return finalSize;
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            Size avail = base.MeasureOverride(availableSize);
            double desiredHeight = 0.0;
            double desiredWidth = availableSize.Width;
            double xPos = 0.0;
            foreach (var child in Children)
            {
                child.Measure(availableSize);
                Size desired = child.DesiredSize;
                double totalWidth = desired.Width;
                if (desired.Height > desiredHeight)
                {
                    desiredHeight = desired.Height;
                }
                if (xPos + totalWidth > desiredWidth)
                {
                    xPos = 0.0;
                    desiredHeight += desired.Height;
                }
                else
                {
                    xPos += totalWidth;
                }                
            }

            return new Size(desiredWidth, desiredHeight);
        }

        
    }
}

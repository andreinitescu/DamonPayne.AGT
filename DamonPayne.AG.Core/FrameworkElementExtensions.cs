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

namespace DamonPayne.AG.Core
{
    public static class FrameworkElementExtensions
    {
        /// <summary>
        /// Returns four corner Points for an element placed on a Canvas
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Point[] GetCanvasCorners(this FrameworkElement u)
        {
            if (!(u.Parent is Canvas))
            {
                throw new ArgumentException("This only works if FrameworkElement is on a Canvas!");
            }

            Point[] corners = new Point[4];

            double left = u.GetValue<double>(Canvas.LeftProperty);
            double top = u.GetValue<double>(Canvas.TopProperty);
            double width = u.Width;
            double height = u.Height;
            if (double.IsNaN(width) || double.IsNaN(height))
            {
                width = u.RenderSize.Width;
                height = u.RenderSize.Height;
            }

            corners[0] = new Point(left, top);
            corners[1] = new Point(left + width, top);
            corners[2] = new Point(left, top + height);
            corners[3] = new Point(left + width, top + height);

            return corners;
        }
    }
}

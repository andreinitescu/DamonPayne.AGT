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
    public static class ControlExtensions
    {
        public static double UniformScaleAmount(this Control c, double maxDimension)
        {
            double uniformScaleAmount = double.NaN;
            if (!double.IsNaN(c.Width))
            {
                double originaltWidth = c.Width;
                double originalHeight = c.Height;
                uniformScaleAmount = Math.Min(maxDimension / originaltWidth, maxDimension / originalHeight);
            }
            else
            {
            }
            return uniformScaleAmount;
        }

        /// <summary>
        /// Remove <paramref name="c"/> from its parent in a manner specific to the Type of its parent
        /// </summary>
        /// <param name="c"></param>
        public static void RemoveFromParent(this Control c)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(c);
            if (parent is ContentPresenter)
            {
                var presenter = (ContentPresenter)parent;
                presenter.Content = null;
            }
            else if (parent is Panel)
            {
                var canvas = (Panel)parent;
                canvas.Children.Remove(c);
            }
        }


    }
}

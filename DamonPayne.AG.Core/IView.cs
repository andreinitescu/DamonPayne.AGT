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
    public interface IView
    {
        /// <summary>
        /// Grid, etc. extends canvas
        /// </summary>
        UserControl VisualRoot { get; }
    }
}

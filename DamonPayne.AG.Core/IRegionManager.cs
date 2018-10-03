using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DamonPayne.AG.Core
{

    public interface IRegionManager 
    {
        /// <summary>
        /// The root visual of our current universe
        /// </summary>
        UIElement ApplicationRootVisual { get; }

        /// <summary>
        /// The topmost visual container that Services and such may interact with.
        /// </summary>
        Panel TopLevelContainer { get; }

        List<string> RegionNames { get; }
        void AddView(IView v);
        void AddView(IView v, string regionName);
        void RemoveView(string regionName);

        IView GetView(string regionName);
        List<IView> Views { get; }
    }
}

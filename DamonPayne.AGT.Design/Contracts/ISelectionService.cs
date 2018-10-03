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
using System.Collections.Generic;

namespace DamonPayne.AGT.Design.Contracts
{
    /// <summary>
    /// Keep track of what is selected
    /// </summary>
    public interface ISelectionService
    {
        void Select(IList<IDesignableControl> selection);
        IList<IDesignableControl> GetSelection();
        int SelectionCount { get; }
        IDesignableControl PrimarySelection { get; }
        event EventHandler SelectionChanged;
    }
}

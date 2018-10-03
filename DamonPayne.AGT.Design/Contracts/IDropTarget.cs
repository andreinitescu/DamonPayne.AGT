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
    public interface IDropTarget
    {
        Size RenderSize { get; }
        Point Location { get; }
        bool IsHitTestVisible { get; }
        void OnDrop(IDesignableControl dc);
        IEnumerable<UIElement> HitTest(Point p);
    }
}

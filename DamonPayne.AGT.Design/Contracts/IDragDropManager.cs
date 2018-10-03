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
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design.Contracts
{
    public interface IDragDropManager
    {
        void RegisterDropTarget(IDropTarget target);
        void BeginDrag(ToolboxItem item);
        void RemoveDropTarget(IDropTarget target);
    }
}

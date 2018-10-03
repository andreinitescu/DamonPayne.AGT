using System;
using System.Net;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Events;
using Microsoft.Practices.Unity;
using DamonPayne.AG.Core;
using DamonPayne.AG.Core.Events;

namespace DamonPayne.AGT.Design.Services
{
    public class DefaultDragDropManager : IDragDropManager
    {
        public DefaultDragDropManager()
        {         
            _dropTargets = new List<IDropTarget>();
            Startup();
        }

        private List<IDropTarget> _dropTargets;

        private GlobalMousePostionChangedEvent _globalMouse;
        private Point _mousePos;
        private MouseLeftButtonUpEvent _mouseUp;

        private Control _dragRepresentation;
        private Type _draggingType;

        [Dependency]
        public IDragTypeCreator DragTypeCreator { get; set; }

        [Dependency]
        public IRegionManager RegionManager { get; set; }

        [Dependency]
        public IDesignTypeCreator DesignCreator { get; set; }

        public virtual void RegisterDropTarget(IDropTarget target)
        {
            _dropTargets.Add(target);   
        }

        public virtual void RemoveDropTarget(IDropTarget target)
        {
            _dropTargets.Remove(target);
        }

        public virtual void BeginDrag(DamonPayne.AGT.Design.Types.ToolboxItem item)
        {
            _draggingType = item.Type;
            _dragRepresentation = DragTypeCreator.CreateDragRepresentation(item.Type);
            _dragRepresentation.SetValue(Canvas.ZIndexProperty, 5785);
            _dragRepresentation.SetValue(Canvas.LeftProperty, _mousePos.X);
            _dragRepresentation.SetValue(Canvas.TopProperty, _mousePos.Y);

            RegionManager.TopLevelContainer.Children.Add(_dragRepresentation);            
        }

        public virtual void EndDrag(MouseButtonEventArgs e)
        {
            RegionManager.TopLevelContainer.Children.Remove(_dragRepresentation);
            _dragRepresentation = null;
            
            foreach (IDropTarget target in _dropTargets)
            {
                if (target.IsHitTestVisible)
                {
                    IEnumerable<UIElement> elements = target.HitTest(_mousePos);
                    if (elements.GetEnumerator().MoveNext())//hit test succeed
                    {
                        IDesignableControl ctrl = DesignCreator.CreateInstance(_draggingType);
                        target.OnDrop(ctrl);
                    }
                }
            }
            _draggingType = null;
        }

        public virtual void MouseMove(Point pos)
        {
            _mousePos = pos;
            if (null != _dragRepresentation)
            {
                _dragRepresentation.SetValue(Canvas.LeftProperty, pos.X);
                _dragRepresentation.SetValue(Canvas.TopProperty, pos.Y);
                //TODO: Maybe change the visual look if we're over a valid IDropTarget?  This would be similar to 
                //the "DragEnter" phase in WinForms
            }
        }

        public virtual void MouseUp(MouseButtonEventArgs e)
        {
            if (null != _dragRepresentation)
            {
                EndDrag(e);
            }
        }

        public void Startup()
        {
            _globalMouse = EventAggregator.Get<GlobalMousePostionChangedEvent, Point>();
            _globalMouse.Subscribe(MouseMove);

            _mouseUp = EventAggregator.Get<MouseLeftButtonUpEvent, MouseButtonEventArgs>();
            _mouseUp.Subscribe(MouseUp);
        }        
    }
}

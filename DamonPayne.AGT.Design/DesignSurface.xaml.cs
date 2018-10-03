using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Events;
using DamonPayne.AGT.Design.Controls;
using Microsoft.Practices.Unity;
using DamonPayne.AG.Core;
using DamonPayne.AGT.Design.Behaviors.Drawing;

namespace DamonPayne.AGT.Design
{
    public partial class DesignSurface : UserControl, IDropTarget, IView, IDesigner, INamingContainer
    {
        public DesignSurface()
        {
            InitializeComponent();
            _isSelecting = false;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DragDropManager.RegisterDropTarget(this);
        }

        private Point _localMousePos;

        public UserControl VisualRoot
        {
            get
            {
                return this;
            }
        }

        public Point Location
        {
            get
            {
                Point p = new Point();
                p.X = (double)GetValue(Canvas.LeftProperty);
                p.Y = (double)GetValue(Canvas.TopProperty);
                return p;
            }
        }

        public Canvas Surface
        {
            get
            {
                return LayoutRoot;
            }
        }

        [Dependency]
        public IDragDropManager DragDropManager { get; set; }

        [Dependency]
        public IDesignTypeCreator DesignTypeCreator { get; set; }

        [Dependency]
        public ILogService Log { get; set; }

        [Dependency]
        public INameProvider NameProvider { get; set; }

        private ISelectionService _selectionSvc;

        [Dependency]
        public ISelectionService SelectionSvc 
        {
            get
            {
                return _selectionSvc;
            }
            set
            {
                if (null != _selectionSvc)
                {
                    _selectionSvc.SelectionChanged -= new EventHandler(SelectionSvc_SelectionChanged);
                }
                _selectionSvc = value;
                _selectionSvc.SelectionChanged += new EventHandler(SelectionSvc_SelectionChanged);
            }
        }

        void SelectionSvc_SelectionChanged(object sender, EventArgs e)
        {
            List<IDesignableControl> idtList = new List<IDesignableControl>();
            foreach (var idt in SelectionSvc.GetSelection())
            {
                if (idt is DesignSite)
                {
                    idtList.Add(((DesignSite)idt).HostedContent);
                }
                else
                {
                    idtList.Add(idt);
                }
            }
            FloatEditService(idtList);
        }                

        [Dependency]
        public IDesignEditorService EditSvc { get; set; }

        /// <summary>
        /// Used when the user clicks & drags to draw a selection shape
        /// </summary>
        public DrawingBehavior SelectDrawingBehavior { get; set; }


        public IEnumerable<UIElement> HitTest(Point p)
        {
            return VisualTreeHelper.FindElementsInHostCoordinates(p, LayoutRoot);
        }

        /// <summary>
        /// Position edit service
        /// </summary>
        /// <param name="idtList"></param>
        protected virtual void FloatEditService(List<IDesignableControl> idtList)
        {
            if (null != idtList && idtList.Count > 0)
            {
                Rect r = GetSelectionBounds();
                Control editVisual = EditSvc.Visual;
                editVisual.RemoveFromParent();

                editVisual.SetValue(Canvas.LeftProperty, r.Left + r.Width + 20.0);
                editVisual.SetValue(Canvas.TopProperty, r.Top);
                editVisual.SetValue(Canvas.ZIndexProperty, Surface.Children.Count);
                Surface.Children.Add(editVisual);

                EditSvc.Edit(idtList);
            }
            else if (Surface.Children.Contains(EditSvc.Visual))
            {
                Surface.Children.Remove(EditSvc.Visual);
            }
        }


        public void OnDrop(IDesignableControl dc)
        {
            EnsureName(dc);
            dc.Visual.SetValue(Canvas.LeftProperty, _localMousePos.X);
            dc.Visual.SetValue(Canvas.TopProperty, _localMousePos.Y);
            LayoutRoot.Children.Add(dc.Visual);
            if (dc is DamonPayne.AGT.Design.Controls.DesignSite)
            {
                ((DamonPayne.AGT.Design.Controls.DesignSite)dc).DesignParent = this;
            }
        }

        private void EnsureName(IDesignableControl dc)
        {
            if (string.IsNullOrEmpty(dc.DesignTimeName)) 
            {
                string name = NameProvider.GetUniqueName(this, ((DesignSite)dc).HostedContent);
                dc.DesignTimeName = name;
            }  
        }

        /// <summary>
        /// Get left,top, width,height of the entirety of all selected components
        /// </summary>
        /// <returns></returns>
        public virtual Rect GetSelectionBounds()
        {
            double left, top, width, height;
            left = int.MaxValue;
            top = int.MaxValue;
            width = 0.0;
            height = 0.0;

            IList<IDesignableControl> selection = SelectionSvc.GetSelection();

            foreach (var idt in selection)
            {
                double l = idt.Visual.GetValue<double>(Canvas.LeftProperty);
                double t = idt.Visual.GetValue<double>(Canvas.TopProperty);

                if (l < left) { left = l; }
                if (t < top) { top = t; }
            }

            foreach (var idt in selection)
            {
                double l = idt.Visual.GetValue<double>(Canvas.LeftProperty);
                double t = idt.Visual.GetValue<double>(Canvas.TopProperty);
                //Using RenderSize takes transforms into account
                double w = idt.Visual.RenderSize.Width;
                double h = idt.Visual.RenderSize.Height;

                if (l + w > width) { width = l + w - left; }
                if (t + h > height) { height = t + h - top; }
            }

            Rect r = new Rect(left, top, width, height);
            return r;
        }




        //Rectangular lasso vars
        private bool _isSelecting;
        private Rectangle _selectingRect;

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            _localMousePos = e.GetPosition(this);
            if (_isSelecting && null != SelectDrawingBehavior)
            {
                SelectDrawingBehavior.MouseMove(e);
                SelectLassoComponents(_selectingRect);
            }                        
        }


        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SelectionSvc.Select(null);
            _isSelecting = true;
            SelectDrawingBehavior = new RectangleDrawingBehavior(LayoutRoot);
            LayoutRoot.CaptureMouse();
            _localMousePos = e.GetPosition(this);
            _selectingRect = (Rectangle)SelectDrawingBehavior.StartDrawing(_localMousePos);                    
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {            
            if (_isSelecting && null != SelectDrawingBehavior)
            {
                if (SelectDrawingBehavior.ShouldStopDrawing(DrawingGestures.MouseLeftButtonUp))
                {
                    var shape = 
                        SelectDrawingBehavior.StopDrawing();
                    _selectingRect = null;
                    LayoutRoot.Children.Remove(shape);
                }
            }
            _isSelecting = false;            
            LayoutRoot.ReleaseMouseCapture();
        }

        protected virtual void SelectLassoComponents(Rectangle lasso)//TODO: make hit test strategy pluggable
        {            
            double left = lasso.GetValue<double>(Canvas.LeftProperty);
            double top = lasso.GetValue<double>(Canvas.TopProperty);
            double width = lasso.Width;
            double height = lasso.Height;
            Rect r = new Rect(left,top,width,height);            
            List<IDesignableControl> selection = new List<IDesignableControl>();

            foreach (var u in LayoutRoot.Children)
            {
                if (u is IDesignableControl)
                {
                    IDesignableControl test = (IDesignableControl)u;
                    Point[] corners = test.Visual.GetCanvasCorners();
                    for (int i = 0; i < corners.Length; ++i)
                    {
                        if (r.Contains(corners[i]))
                        {
                            selection.Add(test);
                            break;
                        }
                    }
                }
            }

            SelectionSvc.Select(selection);
        }

        public IList<IDesignableControl> Children 
        {
            get
            {
                var idcType = typeof(IDesignableControl);
                return (from c in LayoutRoot.Children where c.GetType().ImplementsInterface(idcType) select ((DesignSite)c).HostedContent).ToList();
            }
        }

    }
}

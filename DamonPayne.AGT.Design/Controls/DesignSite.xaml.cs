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
using DamonPayne.AG.Core;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AG.Core.DataTypes;
using Microsoft.Practices.Unity;

namespace DamonPayne.AGT.Design.Controls
{
    public partial class DesignSite : UserControl, IDesignableControl
    {
        //TODO: make me a dependency property
        public double UNIFORM_BORDER_THICKNESS = 5.0d;

        public Brush NORMAL_BORDER;
        public Brush HIDDEN_BORDER = new SolidColorBrush(Colors.Transparent);

        public DesignSite()
        {
            InitializeComponent();
         
            //I don't want the container hanging on to this, so 
            //resolve manually
            _selSvc = ContainerHelper.Resolve<ISelectionService>();
            _selSvc.SelectionChanged += new EventHandler(_selSvc_SelectionChanged);
            _resizing = false;

            _designer = ContainerHelper.Resolve<IDesigner>();
            Log = ContainerHelper.Resolve<ILogService>();
            NORMAL_BORDER = (LinearGradientBrush)Resources["SelectionBorderBrush"];
            SiteBorder.BorderThickness = new Thickness(UNIFORM_BORDER_THICKNESS);
            SiteBorder.BorderBrush = HIDDEN_BORDER;
        }

        void _selSvc_SelectionChanged(object sender, EventArgs e)
        {
            SetSelected(_selSvc.GetSelection().Contains(this));   
        }

        /// <summary>
        /// Remove callbacks so we're not being held onto
        /// </summary>
        ~DesignSite()
        {
            if (null != _selSvc)
            {
                _selSvc.SelectionChanged -= new EventHandler(_selSvc_SelectionChanged);
            }
        }

        [Dependency]
        public ILogService Log { get; set; }

        private ISelectionService _selSvc;

        private IDesigner _designer;

        private IDesignableControl _content;

        //TODO: Unhack
        public IDesigner DesignParent { get; set; }

        private bool _isMoving;
        private Point _localMovePoint;
        private Point _surfaceMousePoint;


        public List<DesignablePropertyDescriptor> GetDesignProperties()
        {
            if (null != HostedContent)
            {
                return HostedContent.GetDesignProperties();
            }
            return null;
        }

        /// <summary>
        /// The Real content
        /// </summary>
        public IDesignableControl HostedContent 
        {
            get
            {
                return _content;
            }
            set
            {
                if (null != _content)
                {
                    _content.Visual.MouseLeftButtonDown -= Glass_MouseLeftButtonDown;
                }
                _content = value;
                _content.Visual.MouseLeftButtonDown += new MouseButtonEventHandler(Glass_MouseLeftButtonDown);

                //New
                ContentCanvas.Children.Remove(Default);
                _content.Visual.SetValue(Canvas.ZIndexProperty, 1);
                ContentCanvas.Children.Add(_content.Visual);
                _content.Visual.Cursor = Cursors.Arrow;
                LayoutRoot.Width = _content.Visual.Width;
                LayoutRoot.Height = _content.Visual.Height;
                ContentCanvas.Width = _content.Visual.Width;
                ContentCanvas.Height = _content.Visual.Height;
                Glass.Height = _content.Visual.Height;
                Glass.Width = _content.Visual.Width;                
            }
        }

        void Glass_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //if this is already selected, we might want to move.
            if ( (_selSvc.SelectionCount > 0 && _selSvc.GetSelection().Contains(this)))
            {
                StartMove(e);
            }
            else if (!_selSvc.GetSelection().Contains(this))
            {
                _selSvc.Select(new List<IDesignableControl> { this });
                StartMove(e);
            }
            e.Handled = true;
        }

        private void StartMove(MouseButtonEventArgs e)
        {
            _isMoving = true;
            _localMovePoint = e.GetPosition(Glass);
            _surfaceMousePoint = e.GetPosition(DesignParent.Surface);
            Glass.CaptureMouse();
        }

        private void Glass_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                MouseMoveSelection(e);
            }
        }

        protected virtual void MouseMoveSelection(MouseEventArgs e)
        {
            Point currentPoint = e.GetPosition(DesignParent.Surface);
            double leftChange = currentPoint.X - _surfaceMousePoint.X;
            double topChange = currentPoint.Y - _surfaceMousePoint.Y;
            Rect selectionBounds = DesignParent.GetSelectionBounds();
            
            if (!EnsureValidSelectionBounds(selectionBounds))
            {
                _isMoving = false;
                Glass.ReleaseMouseCapture();
                Rect diff = CalculateOverlap(selectionBounds);
                MoveSelection(diff.Left, diff.Top);//Move by amount out of bounds
            }

            //
            MoveSelection(leftChange, topChange);
            _surfaceMousePoint = currentPoint;
        }

        /// <summary>
        /// Use a Rect to determine how much to the left/top that <paramref name="r"/> is out of bounds.
        /// </summary>
        /// <param name="r"></param>
        /// <returns></returns>
        protected virtual Rect CalculateOverlap(Rect r)
        {
            
            double overLeft  = 0.0;
            double overTop = 0.0;
            if (r.Left < 0)
            {
                overLeft = -1 * r.Left;
            }
            else if (r.Width + r.Left > DesignParent.Surface.ActualWidth)
            {
                overLeft = DesignParent.Surface.ActualWidth - (r.Left + r.Width);
            }
            if (r.Top < 0)
            {
                overTop = -1 * r.Top;
            }
            else if (r.Height + r.Top > DesignParent.Surface.ActualHeight)
            {
                overTop = DesignParent.Surface.ActualHeight - (r.Height + r.Top);
            }

            var overlap = new Rect(overLeft, overTop, 0.0, 0.0);
            return overlap;
        }

        /// <summary>
        /// Move the entire selection by the given amount
        /// </summary>
        /// <param name="leftChange"></param>
        /// <param name="topChange"></param>
        protected virtual void MoveSelection(double leftChange, double topChange)
        {
            foreach (var sel in _selSvc.GetSelection())
            {
                double left = sel.Visual.GetValue<double>(Canvas.LeftProperty);
                double top = sel.Visual.GetValue<double>(Canvas.TopProperty);
                left += leftChange;
                top += topChange;
                sel.Visual.SetValue(Canvas.LeftProperty, left);
                sel.Visual.SetValue(Canvas.TopProperty, top);
            }
        }

        protected virtual bool EnsureValidSelectionBounds(Rect r)
        {
            bool valid = true;
            if (r.Left < 0 || r.Top < 0)
            {
                return false;
            }
            else if (r.Width + r.Left > DesignParent.Surface.ActualWidth)
            {
                return false;
            }
            else if (r.Height + r.Top > DesignParent.Surface.ActualHeight)
            {
                return false;
            }

            return valid;
        }


 


        protected void SetSelected(bool sel)
        {
            UIElement tgt = this;

            double left = tgt.GetValue<double>(Canvas.LeftProperty);
            double top = tgt.GetValue<double>(Canvas.TopProperty);

            if (!sel)
            {
                SiteBorder.BorderBrush = HIDDEN_BORDER;
                //SiteBorder.BorderThickness = new Thickness(0.0);
                //left += UNIFORM_BORDER_THICKNESS;
                //top += UNIFORM_BORDER_THICKNESS;
            }
            else if (sel)
            {
                //SiteBorder.BorderThickness = new Thickness(UNIFORM_BORDER_THICKNESS);
                //left -= UNIFORM_BORDER_THICKNESS;
                //top -= UNIFORM_BORDER_THICKNESS;
                SiteBorder.BorderBrush = NORMAL_BORDER;
            }
            //tgt.SetValue(Canvas.LeftProperty, left);
            //tgt.SetValue(Canvas.TopProperty, top);
        }
        
        public Control Visual
        {
            get 
            {
                return this; 
            }
        }

        public bool IsBoundsResizable
        {
            get { return HostedContent.IsBoundsResizable; }
        }

        public bool IsTransformable
        {
            get { return HostedContent.IsTransformable; }
        }

        public string DesignTimeName
        {
            get
            {
                return HostedContent.DesignTimeName;
            }
            set
            {
                HostedContent.DesignTimeName = value;
            }
        }

        private bool _resizing;

        private void SiteBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _resizing = true;
            SiteBorder.CaptureMouse();
            e.Handled = true;
        }

        private void SiteBorder_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _resizing = false;
            SiteBorder.ReleaseMouseCapture();
        }

        private void SiteBorder_MouseMove(object sender, MouseEventArgs e)
        {
            if (_resizing)
            {
                if (HostedContent.IsBoundsResizable)
                {
                    ResizeContent(e);
                }
                else
                {
                    TransformContent(e);
                }                
            }
        }

        private void ResizeContent(MouseEventArgs e)
        {
            Point resizePoint = e.GetPosition(DesignParent.Surface);
            //Determine where the DesignSite is on parent
            double left = this.GetValue<double>(Canvas.LeftProperty);
            double top = this.GetValue<double>(Canvas.TopProperty);
            double newWidth = resizePoint.X - left;
            double newHeight = resizePoint.Y - top;

            //Take border into account
            double innerWidth = newWidth - (2 * UNIFORM_BORDER_THICKNESS);
            double innerHeight = newHeight - (2 * UNIFORM_BORDER_THICKNESS);

            if (newWidth > 5 && newHeight > 5)
            {
                _content.Visual.Width = innerWidth;
                _content.Visual.Height = innerHeight;
                Glass.Width = innerWidth;
                Glass.Height = innerHeight;
                LayoutRoot.Width = newWidth;
                LayoutRoot.Height = newHeight;
                
                ContentCanvas.Width = innerWidth;
                ContentCanvas.Height = innerHeight;

                SiteBorder.Width = newWidth;
                SiteBorder.Height = newHeight;

            }
        }

        private void TransformContent(MouseEventArgs e)
        {
            Point resizePoint = e.GetPosition(DesignParent.Surface);
            //Determine where the DesignSite is on parent
            double left = this.GetValue<double>(Canvas.LeftProperty);
            double top = this.GetValue<double>(Canvas.TopProperty);
            double newWidth = resizePoint.X - left;
            double newHeight = resizePoint.Y - top;
            //We need to attain a render size equal to our new border inner bounds,
            //so we'll calculate what scale transform would get us there
            double innerWidth = newWidth - (2 * UNIFORM_BORDER_THICKNESS);
            double innerHeight = newHeight - (2 * UNIFORM_BORDER_THICKNESS);

            SiteBorder.Width = newWidth;
            SiteBorder.Height = newHeight;
            //
            Glass.Width = innerWidth;
            Glass.Height = innerHeight;
            //
            ContentCanvas.Width = innerWidth;
            ContentCanvas.Height = innerHeight;
            //
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = innerWidth / HostedContent.Visual.ActualWidth;
            st.ScaleY = innerHeight / HostedContent.Visual.ActualHeight;
            HostedContent.Visual.RenderTransform = st;
        }


        private void SiteBorder_LayoutUpdated(object sender, EventArgs e)
        {

        }

        private void SiteBorder_Loaded(object sender, RoutedEventArgs e)
        {
            Log.Log(new LogMessage
            {
                Level = LogLevels.Debug,
                Message = "SiteBorder_Loaded"
            });
        }

        private void SiteBorder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log.Log(new LogMessage
            {
                Level = LogLevels.Debug,
                Message = "Resized to " + e.NewSize
            });
        }

        private void Glass_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _isMoving = false;
            Glass.ReleaseMouseCapture();
        }


    }
}

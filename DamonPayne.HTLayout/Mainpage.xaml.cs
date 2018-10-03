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
using DamonPayne.HTLayout.Controls;
using DamonPayne.HTLayout.ViewContracts;
using DamonPayne.HTLayout.Presenters;
using DamonPayne.AG.Core;
using DamonPayne.AG.Core.Events;
using DamonPayne.AGT.Design.Events;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Controls;
using DamonPayne.AGT.Design.Contracts;
using Microsoft.Practices.Unity;

namespace DamonPayne.HTLayout
{
    public partial class MainPage : UserControl, IRegionManager, IRootView
    {
        public MainPage()
        {
            InitializeComponent();

            _regionNames = new List<string>()
            {
                "ToolboxRegion",
                "DesignSurfaceRegion",
                "PropertyGridRegion",
                "MessageConsoleRegion"
            };
            _views = new Dictionary<string, IView>();
            Presenter = new RootDesignerPresenter(this);
        }

        private List<string> _regionNames;
        private Dictionary<string, IView> _views;

        public RootDesignerPresenter Presenter { get; set; }

        public UIElement ApplicationRootVisual
        {
            get
            {
                return this;
            }
        }

        public Panel TopLevelContainer {
            get
            {
                return LayoutRoot;
            }
        }

        public List<string> RegionNames
        {
            get 
            {
                return _regionNames;
            }
        }

        public void AddView(IView v)
        {
            LayoutRoot.Children.Add(v.VisualRoot);
        }

        public void AddView(IView v, string regionName)
        {
            if (regionName == "ToolboxRegion")
            {
                ToolboxRegion.Content = v.VisualRoot;
            }
            else if (regionName == "DesignSurfaceRegion")
            {
                DesignSurfaceRegion.Content = v.VisualRoot;
            } 
            else if (regionName == "PropertyGridRegion")
            {
                PropertyGridRegion.Content = v.VisualRoot;
            }
            else if (regionName =="MessageConsoleRegion" )
            {
                MessageConsoleRegion.Content = v.VisualRoot;
            }

            _views.Add(regionName, v);
        }

        public void RemoveView(string regionName)
        {
            if (regionName == "ToolboxRegion")
            {
                ToolboxRegion.Content = new Canvas();
            }
            else if (regionName == "DesignSurfaceRegion")
            {
            }
            else if (regionName == "PropertyGridRegion")
            {
            }
            else if (regionName == "MessageConsoleRegion")
            {
            }
            _views.Remove(regionName);
        }

        public IView GetView(string regionName)
        {
            return _views[regionName];
        }

        public List<IView> Views
        {
            get 
            {
                return _views.Values.ToList<IView>();
            }
        }

        private GlobalMousePostionChangedEvent _globalMouse;
        private MouseLeftButtonDownEvent _leftMouseDown;
        private MouseLeftButtonUpEvent _leftMouseUp;

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            var container = ContainerHelper.I;

            IRegionManager mgr = container.Resolve<IRegionManager>();
            MessageConsoleView view = new MessageConsoleView();
            container.BuildUp(view);
            mgr.AddView(view, "MessageConsoleRegion");
            //
            ToolboxView tbView = new ToolboxView();
            container.BuildUp(tbView);
            container.RegisterInstance<IToolboxService>(tbView);
            mgr.AddView(tbView, "ToolboxRegion");

            _propGrid = (PropertyGrid)PropertyGridRegion.Content;
            container.BuildUp(_propGrid);
            container.RegisterInstance<IDesignEditorService>(_propGrid);

            DesignSurface surface = new DesignSurface();
            container.BuildUp(surface);
            container.RegisterInstance<IDesigner>(surface);
            mgr.AddView(surface, "DesignSurfaceRegion");

            _globalMouse = EventAggregator.Get<GlobalMousePostionChangedEvent, Point>();
            _leftMouseDown = EventAggregator.Get<MouseLeftButtonDownEvent, MouseButtonEventArgs>();
            _leftMouseUp = EventAggregator.Get<MouseLeftButtonUpEvent, MouseButtonEventArgs>();

            container.BuildUp(Presenter);
            Presenter.InitializeDesignableControls();
        }

        private void LayoutRoot_MouseMove(object sender, MouseEventArgs e)
        {
            if (null != _globalMouse)
            {
                _globalMouse.Raise(e.GetPosition(this));
            }
        }

        private void LayoutRoot_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (null != _leftMouseDown)
            {
                _leftMouseDown.Raise(e);
            }
        }

        private void LayoutRoot_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (null != _leftMouseUp)
            {
                _leftMouseUp.Raise(e);
            }
        }

    }
}

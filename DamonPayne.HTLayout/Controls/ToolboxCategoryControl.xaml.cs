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
using DamonPayne.HTLayout.ViewModels;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AG.Core;

namespace DamonPayne.HTLayout.Controls
{
    public partial class ToolboxCategoryControl : UserControl
    {
        public ToolboxCategoryControl()
        {
            InitializeComponent();
            _model = new ToolboxCategoryModel();
            DataContext = _model;
            _dragging = false;
            _itemMappings = new Dictionary<ToolboxItem, ToolboxItemPreviewContainer>();
        }

        private ToolboxCategoryModel _model;
        private Dictionary<ToolboxItem, ToolboxItemPreviewContainer> _itemMappings;        

        public event EventHandler DragStart;


        public string CategoryName
        {
            get
            {
                return _model.CategoryName;
            }
            set
            {
                _model.CategoryName = value;
            }
        }
      

        public void Add(ToolboxItem item)
        {
            _model.ToolboxItems.Add(item);
            var preview = BuildPreview(item);
            WrapPanel.Children.Add(preview);

            preview.MouseLeftButtonDown += Preview_MouseLeftButtonDown;
            preview.MouseLeftButtonUp += Preview_MouseLeftButtonUp;
            preview.MouseMove += Preview_MouseMove;

            _itemMappings.Add(item, preview);
        }

        public void Remove(ToolboxItem item)
        {
            _model.ToolboxItems.Remove(item);
            _model.OnPropertyChanged("ToolboxItems");
            _itemMappings[item].MouseLeftButtonDown -= Preview_MouseLeftButtonDown;
            _itemMappings[item].MouseLeftButtonUp -= Preview_MouseLeftButtonUp;
            _itemMappings[item].MouseMove -= Preview_MouseMove;
            _itemMappings.Remove(item);
        }

        public void Deselect()
        {
        }

        private bool _dragging;


        private void Preview_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var preview = (ToolboxItemPreviewContainer)sender;
            int index = WrapPanel.Children.IndexOf(preview);
            _model.SelectedToolboxItem = _model.ToolboxItems[index];
            _dragging = true;
        }

        private void Preview_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _dragging = false;
        }

        private void Preview_MouseMove(object sender, MouseEventArgs e)
        {
            if (_dragging)
            {                
                if (null != DragStart)
                {                    
                    DragStart(this, EventArgs.Empty);
                }
                _dragging = false;
            }
        }

        protected virtual ToolboxItemPreviewContainer BuildPreview(ToolboxItem item)
        {
            var preview = new ToolboxItemPreviewContainer();
            preview.PreviewText = item.Name;
            preview.Margin = new Thickness(5.0);
            Control c = GetMiniControl(item.Type, 55.0);
            preview.PreviewVisual = c;   
            return preview;
        }

        protected virtual Control GetMiniControl(Type t, double maxDimension)
        {
            Control c = (Control)Activator.CreateInstance(t);
            double uniformScaleAmount = c.UniformScaleAmount(maxDimension);

            if (double.IsNaN(uniformScaleAmount))
            {
                c.Measure(new Size(maxDimension, maxDimension));
            }

            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = uniformScaleAmount;
            st.ScaleY = uniformScaleAmount;
            tg.Children.Add(st);
            c.RenderTransform = tg;

            double estimatedNewWidth = c.Width * uniformScaleAmount;
            double estimatedNewHeight = c.Height * uniformScaleAmount;

            double left = (85 / 2.0D) - (estimatedNewWidth / 2.0D);
            double top = (55.0 / 2.0D) - (estimatedNewHeight / 2.0D);
            c.SetValue(Canvas.LeftProperty, left);
            c.SetValue(Canvas.TopProperty, top);

            return c;
        }
        
    }
}

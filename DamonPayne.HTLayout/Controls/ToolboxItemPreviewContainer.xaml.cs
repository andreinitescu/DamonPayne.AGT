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

namespace DamonPayne.HTLayout.Controls
{
    public partial class ToolboxItemPreviewContainer : UserControl
    {
        public ToolboxItemPreviewContainer()
        {
            InitializeComponent();
        }

        public string PreviewText 
        {
            get { return NameTxt.Text; }
            set
            {
                NameTxt.Text = value;
            }
        }

        private Control _pv;

        public Control PreviewVisual
        {
            get
            {
                return _pv;
            }
            set
            {
                if (null != _pv)
                {
                    PreviewLayout.Children.Remove(_pv);
                }
                _pv = value;
                PreviewLayout.Children.Add(_pv);
            }
        }

    }
}

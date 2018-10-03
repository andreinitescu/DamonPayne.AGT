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
using DamonPayne.AGT.Design.Types;
using DamonPayne.AGT.Design.Services;

namespace DamonPayne.HTLayout.Controls
{
    public partial class DummyButton : UserControl, DamonPayne.AGT.Design.IDesignableControl
    {
        public DummyButton()
        {
            InitializeComponent();
        }

        public Control Visual
        {
            get { return this; }
        }

        public bool IsBoundsResizable
        {
            get { return true; }
        }

        public bool IsTransformable
        {
            get { return false; }
        }

        public string DesignTimeName
        {
            get;
            set;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            DB.Width = e.NewSize.Width;
            DB.Height = e.NewSize.Height;
        }

        public List<DesignablePropertyDescriptor> GetDesignProperties()
        {
            var props = EditServiceHelper.GetDefaultDescriptors();
            return props;
        }

    }
}

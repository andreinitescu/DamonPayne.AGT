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
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Services;

namespace DamonPayne.HTLayout.Controls
{
    public partial class AreaRug : UserControl, IDesignableControl
    {
        public AreaRug()
        {
            InitializeComponent();
            DesignTimeName = "Rugzor";
        }


        public Control Visual
        {
            get { return this; }
        }

        public bool IsBoundsResizable
        {
            get { return false; }
        }

        public bool IsTransformable
        {
            get { return true; }
        }

        private string _dtName;

        public string DesignTimeName
        {
            get
            {
                return _dtName;
            }
            set
            {
                _dtName = value;
            }
        }

        public List<DamonPayne.AGT.Design.Types.DesignablePropertyDescriptor> GetDesignProperties()
        {
            return EditServiceHelper.GetDefaultDescriptors();
        }

    }
}

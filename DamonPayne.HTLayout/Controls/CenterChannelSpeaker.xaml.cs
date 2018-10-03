using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Services;

namespace DamonPayne.HTLayout
{
	public partial class CenterChannelSpeaker : UserControl, IDesignableControl
	{
		public CenterChannelSpeaker()
		{
			// Required to initialize variables
			InitializeComponent();
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

        public string DesignTimeName{get;set;}

        public List<DesignablePropertyDescriptor> GetDesignProperties()
        {
            var props = EditServiceHelper.GetDefaultDescriptors();
            return props;
        }
    }
}
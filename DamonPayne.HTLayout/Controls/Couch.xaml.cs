using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Collections.Generic;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AGT.Design.Services;

using DamonPayne.HTLayout.Editors;
using DamonPayne.HTLayout.Visualizers;

namespace DamonPayne.HTLayout
{
	public partial class Couch : UserControl, IDesignableControl
	{
		public Couch()
		{
			// Required to initialize variables
			InitializeComponent();
            FillColor = Colors.Purple;
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

        public string DesignTimeName { get; set; }


        private Color _fillColor;

        public Color FillColor 
        {
            get { return _fillColor; }
            set
            {
                _fillColor = value;
                LayoutRoot.Background = new SolidColorBrush(value);
            }
        }

        public double Softness { get; set; }

        public List<DesignablePropertyDescriptor> GetDesignProperties()
        {
            List<DesignablePropertyDescriptor> props = EditServiceHelper.GetDefaultDescriptors();
                        
            DesignablePropertyDescriptor fillColor = new DesignablePropertyDescriptor();
            fillColor.PropertyInfo = GetType().GetProperty("FillColor");
            fillColor.DisplayName = "Color";
            fillColor.DisplayType = typeof(TextBlock);
            fillColor.Editable = true;
            fillColor.EditorType = typeof(ComboBox); 
            fillColor.SupportsStandardValues = true;
            fillColor.StandardValues = new List<object>( new object[] {Colors.Purple, Colors.Red, Colors.Gray, Colors.Brown,Colors.Blue,Colors.Green,Colors.Orange});
            fillColor.Converter = new DamonPayne.AGT.Design.Converters.ColorConverter();
            props.Add(fillColor);

            DesignablePropertyDescriptor softness = new DesignablePropertyDescriptor();
            softness.DisplayName = "Softness";
            softness.Converter = null;
            softness.DisplayType = typeof(SoftnessVisualizer);
            softness.Editable = true;
            softness.EditorType = typeof(SoftnessEditor);
            softness.PropertyInfo = GetType().GetProperty("Softness");
            
            props.Add(softness);


            return props;            
        }

    }
}
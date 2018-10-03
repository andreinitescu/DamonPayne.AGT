using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Contracts;


namespace DamonPayne.HTLayout.Editors
{
    public partial class SoftnessEditor : UserControl, IDesignablePropertyEditor
    {
        public SoftnessEditor()
        {
            InitializeComponent();
        }

        public Control Visual
        {
            get { return this; }
        }

        /// <summary>
        /// Setup bindings etc.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="desc"></param>
        public void Initialize(IDesignableControl instance, DamonPayne.AGT.Design.Types.DesignablePropertyDescriptor desc)
        {
            Binding b = new Binding("Softness");
            b.Converter = desc.Converter;
            b.Mode = BindingMode.TwoWay;
            b.Source = instance;
            Slider.SetBinding(Slider.ValueProperty, b);
        }

    }
}

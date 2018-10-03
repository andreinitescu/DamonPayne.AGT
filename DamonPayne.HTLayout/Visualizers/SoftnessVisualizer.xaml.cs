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
using System.Windows.Data;
using DamonPayne.AGT.Design;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.HTLayout.Visualizers
{
    public partial class SoftnessVisualizer : UserControl, IDesignablePropertyVisualizer
    {
        public SoftnessVisualizer()
        {
            InitializeComponent();
        }

        public Control Visual
        {
            get
            {
                return this;
            }
        }


        public double Softness
        {
            get { return (double)GetValue(SoftnessProperty); }
            set { SetValue(SoftnessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Softness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SoftnessProperty =
            DependencyProperty.Register("Softness", typeof(double), typeof(SoftnessVisualizer), new PropertyMetadata(SoftnessChanged));

        public static void SoftnessChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            if(obj is SoftnessVisualizer)
            {
                ((SoftnessVisualizer)obj).ScaleSoftness();
            }
        }

        protected void ScaleSoftness()
        {
            if (Softness > 0.0)
            {
                VisualScale.ScaleX = Softness;
                VisualTranslate.X = 10 * Softness;
            }
            else
            {
                VisualScale.ScaleX = 1.0;
                VisualTranslate.X = 0.0;
            }
        }

        /// <summary>
        /// We know we're just for softness!
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="desc"></param>
        public void Initialize(DamonPayne.AGT.Design.IDesignableControl instance, DamonPayne.AGT.Design.Types.DesignablePropertyDescriptor desc)
        {
            if (null != instance)
            {
                Binding b = new Binding("Softness");
                b.Converter = desc.Converter;
                b.Mode = BindingMode.TwoWay;
                b.Source = instance;
                SetBinding(SoftnessVisualizer.SoftnessProperty, b);
            }
        }
        
    }
}

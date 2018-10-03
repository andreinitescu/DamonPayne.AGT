using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;
using DamonPayne.AG.Core;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design.Services
{
    /// <summary>
    /// Methods to assist an IDesignEditorService implementation with property functions, sort of like TypeDescriptor
    /// in the full framework, contains various design time defaults for various Types, Converters, Edit Representations
    /// </summary>
    public class EditServiceHelper
    {
        static EditServiceHelper()
        {
            _editors = new Dictionary<Type, Type>();
            _displayors = new Dictionary<Type, Type>();
            _basicBindableDisplayTypes = new Dictionary<Type, DependencyProperty>();
            _basicBindableEditTypes = new Dictionary<Type, DependencyProperty>();

            AddDefaultDisplayor(typeof(string), typeof(TextBlock));
            AddDefaultDisplayor(typeof(Color), typeof(TextBlock));
            AddDefaultEditor(typeof(string), typeof(TextBox));
            AddDefaultEditor(typeof(Color), typeof(ComboBox));
            AddBasicBindableDisplayType(typeof(TextBlock), TextBlock.TextProperty);
            AddBasicBindableEditType(typeof(TextBox), TextBox.TextProperty);
            AddBasicBindableEditType(typeof(ComboBox), ComboBox.SelectedItemProperty);
        }

        private static Dictionary<Type, Type> _editors;
        private static Dictionary<Type, Type> _displayors;
        private static Dictionary<Type, DependencyProperty> _basicBindableDisplayTypes;
        private static Dictionary<Type, DependencyProperty> _basicBindableEditTypes;

        /// <summary>
        /// Set editorType as the default editor for srcType, such as "string", "TextBox"
        /// </summary>
        /// <param name="srcType"></param>
        /// <param name="editorType"></param>
        public static void AddDefaultEditor(Type srcType, Type editorType)
        {
            _editors.Add(srcType, editorType);
        }

        /// <summary>
        /// Set displayType as the default display UI for srcType, such as "string", "TextBlock"
        /// </summary>
        /// <param name="srcType"></param>
        /// <param name="displayType"></param>
        public static void AddDefaultDisplayor(Type srcType, Type displayType)
        {
            _displayors.Add(srcType, displayType);
        }

        /// <summary>
        /// Basic bindings mappings, such as TextBox, TextBox.TextProperty
        /// </summary>
        /// <param name="t"></param>
        /// <param name="targetProp"></param>
        public static void AddBasicBindableDisplayType(Type t, DependencyProperty targetProp)
        {
            _basicBindableDisplayTypes.Add(t, targetProp);
        }

        public static void AddBasicBindableEditType(Type t, DependencyProperty targetProp)
        {
            _basicBindableEditTypes.Add(t, targetProp);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        public static FrameworkElement GetDisplayInstance(IDesignableControl instance, DesignablePropertyDescriptor desc)
        {
            //if null we try to find a default, otherwise see if we can do a binding anyway using the override
            if (null == desc.DisplayType || _basicBindableDisplayTypes.ContainsKey(desc.DisplayType))
            {
                Type displayType = _displayors[desc.PropertyInfo.PropertyType];
                FrameworkElement displayInstance = null;
                if (null != displayType)
                {
                    displayInstance = (FrameworkElement)Activator.CreateInstance(displayType);
                    SetupDisplayInstanceBinding(instance, desc, displayInstance);
                }
                return displayInstance;
            }
            else if (desc.DisplayType.ImplementsInterface(typeof(IDesignablePropertyVisualizer)))
            {
                var visualizer = (IDesignablePropertyVisualizer)Activator.CreateInstance(desc.DisplayType);
                visualizer.Initialize(instance, desc);
                return visualizer.Visual;
            }
            return null;
        }

        protected static void SetupDisplayInstanceBinding(IDesignableControl instance, 
            DesignablePropertyDescriptor desc, FrameworkElement display)
        {
            if (_basicBindableDisplayTypes.ContainsKey(display.GetType()))
            {                
                var dProp = _basicBindableDisplayTypes[display.GetType()];
                Binding b = new Binding(desc.PropertyInfo.Name);
                b.Converter = desc.Converter;
                b.Mode = BindingMode.TwoWay;
                b.Source = instance;
                display.SetBinding(dProp, b);
            }
        }

        public static FrameworkElement GetEditInstance(IDesignableControl instance, DesignablePropertyDescriptor desc)
        {
            if (!desc.Editable)
            {
                return null;
            }

            if (null == desc.EditorType || _basicBindableEditTypes.ContainsKey(desc.EditorType))
            {
                Type editType = _editors[desc.PropertyInfo.PropertyType];
                FrameworkElement editInstance = null;
                editInstance = (FrameworkElement)Activator.CreateInstance(editType);

                if (editInstance is ItemsControl && desc.SupportsStandardValues)
                {
                    SetupItemsValues(desc, (ItemsControl)editInstance);
                }

                SetupEditInstanceBinding(instance, desc, editInstance);
                
                return editInstance;
            } 
            else if (desc.EditorType.ImplementsInterface(typeof(IDesignablePropertyEditor)))
            {
                var editor = (IDesignablePropertyEditor)Activator.CreateInstance(desc.EditorType);
                editor.Initialize(instance, desc);
                return editor.Visual;
            }

            return null;
        }

        /// <summary>
        /// Populate List Item with appropriate .... ?
        /// </summary>
        /// <param name="desc"></param>
        /// <param name="editor"></param>
        protected static void SetupItemsValues(DesignablePropertyDescriptor desc, ItemsControl editor)
        {
            if (null != desc.Converter)
            {
                List<object> vals = new List<object>();//TODO, is copy & run converter really the best option here?
                Type t = desc.PropertyInfo.PropertyType;
                foreach (object o in desc.StandardValues)
                {
                    var converted = desc.Converter.Convert(o, typeof(string), null, null);
                    vals.Add(converted);
                }
                editor.ItemsSource = vals;
            }
            else
            {
                editor.ItemsSource = desc.StandardValues;
            }
        }

        protected static void SetupEditInstanceBinding(IDesignableControl instance, DesignablePropertyDescriptor desc, FrameworkElement editor)
        {
            if (_basicBindableEditTypes.ContainsKey(editor.GetType()))
            {
                var dProp = _basicBindableEditTypes[editor.GetType()];
                Binding b = new Binding(desc.PropertyInfo.Name);
                b.Converter = desc.Converter;
                b.Mode = BindingMode.TwoWay;
                b.Source = instance;
                editor.SetBinding(dProp, b);
            }
        }



        /// <summary>
        /// Returns common props from IDesignableControl : DesignTimeName, 
        /// </summary>
        /// <returns></returns>
        public static List<DesignablePropertyDescriptor> GetDefaultDescriptors()
        {
            List<DesignablePropertyDescriptor> common = new List<DesignablePropertyDescriptor>();

            DesignablePropertyDescriptor name = new DesignablePropertyDescriptor();
            name.PropertyInfo = typeof(IDesignableControl).GetProperty("DesignTimeName");
            name.DisplayName = "Name";
            name.Editable = true;

            common.Add(name);
            return common;
        }

        /// <summary>
        /// Return default <code>DesignablePropertyDescriptor</code> objects for each Property name
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public static List<DesignablePropertyDescriptor> GetDescriptors(IEnumerable<string> names, IDesignableControl idc)
        {
            List<DesignablePropertyDescriptor> props = new List<DesignablePropertyDescriptor>();
            Type t = idc.GetType();

            foreach (string s in names)
            {
                var dp = new DesignablePropertyDescriptor();
                dp.DisplayName = s;
                dp.Editable = true;
                dp.PropertyInfo = t.GetProperty(s);
                props.Add(dp);
            }

            return props;
        }

    }
}

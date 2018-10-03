using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace DamonPayne.AGT.Design.Types
{
    /// <summary>
    /// 
    /// </summary>
    public class PropertyGridModel
    {
        public PropertyGridModel()
        {
            Selection = new List<IDesignableControl>();
            _displayElements = new Dictionary<DesignablePropertyDescriptor, FrameworkElement>();
            _displayElementsReverseLookup = new Dictionary<FrameworkElement, DesignablePropertyDescriptor>();
            _editElements = new Dictionary<DesignablePropertyDescriptor, FrameworkElement>();
            _editElementsReverseLookup = new Dictionary<FrameworkElement, DesignablePropertyDescriptor>();
        }

        /// <summary>
        /// What are we editing?
        /// </summary>
        public virtual List<IDesignableControl> Selection { get; set; }

        protected List<DesignablePropertyDescriptor> _props;

        protected Dictionary<DesignablePropertyDescriptor, FrameworkElement> _displayElements;
        protected Dictionary<FrameworkElement, DesignablePropertyDescriptor> _displayElementsReverseLookup;

        protected Dictionary<DesignablePropertyDescriptor, FrameworkElement> _editElements;
        protected Dictionary<FrameworkElement, DesignablePropertyDescriptor> _editElementsReverseLookup;

        /// <summary>
        /// Properties common across the entire Selection
        /// </summary>
        /// <param name="props"></param>
        public virtual void SetProperties(List<DesignablePropertyDescriptor> props)
        {
            _props = props;
        }

        /// <summary>
        /// Only 1 property is editing at a time
        /// </summary>
        public FrameworkElement CurrentEditElement { get; set; }

        public virtual void SetDisplayElement(DesignablePropertyDescriptor d, FrameworkElement fe)
        {
            _displayElements[d] = fe;
            _displayElementsReverseLookup[fe] = d;
        }

        public virtual FrameworkElement GetDisplayElement(DesignablePropertyDescriptor d)
        {
            if (_displayElements.ContainsKey(d))
            {
                return _displayElements[d];
            }
            return null;
        }
        public virtual List<FrameworkElement> GetAllDisplayElements()
        {
            return _displayElements.Values.ToList<FrameworkElement>();
        }

        public virtual void SetEditElement(DesignablePropertyDescriptor d, FrameworkElement fe) 
        {
            _editElements[d] = fe;
            _editElementsReverseLookup[fe] = d;
        }

        public virtual FrameworkElement GetEditElement(DesignablePropertyDescriptor d)
        {
            if (_editElements.ContainsKey(d))
            {
                return _editElements[d];
            }
            return null;
        }

        public virtual DesignablePropertyDescriptor GetDescriptorForEditElement(FrameworkElement fe)
        {
            if (_editElementsReverseLookup.ContainsKey(fe))
            {
                return _editElementsReverseLookup[fe];
            }
            return null;
        }


        public virtual DesignablePropertyDescriptor GetDescriptorForDisplayElement(FrameworkElement fe)
        {
            if (_displayElementsReverseLookup.ContainsKey(fe))
            {
                return _displayElementsReverseLookup[fe];
            }

            return null;
        }

        /// <summary>
        /// Remove everything from the Model
        /// </summary>
        public virtual void Reset()
        {
            Selection.Clear();
            _displayElements.Clear();
            _displayElementsReverseLookup.Clear();
            _editElements.Clear();
            _editElementsReverseLookup.Clear();
            if (null != _props)
            {
                _props.Clear(); 
            }
            CurrentEditElement = null;
        }
    }
}

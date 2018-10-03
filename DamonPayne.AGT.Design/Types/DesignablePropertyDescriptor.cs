using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Data;

namespace DamonPayne.AGT.Design.Types
{
    public class DesignablePropertyDescriptor
    {
        public DesignablePropertyDescriptor()
        {
            SupportsStandardValues = false;
            EditorType = null;
            DisplayType = null;
        }


        public PropertyInfo PropertyInfo { get; set; }

        /// <summary>
        /// Return false if this property is read-only at design time
        /// </summary>
        public bool Editable { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; set; }
        public bool SupportsStandardValues { get; set; }
        public List<object> StandardValues { get; set; }

        /// <summary>
        /// Type of the control used to edit this property, do not set to use default
        /// </summary>
        public Type EditorType { get; set; }

        /// <summary>
        /// Type of the control used to display this property's current value
        /// </summary>
        public Type DisplayType { get; set; }

        public IValueConverter Converter { get; set; }

        public override bool Equals(object obj)
        {
            return GetHashCode().Equals(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            string hash = PropertyInfo.Name + PropertyInfo.PropertyType+ Editable + DisplayName + EditorType + DisplayType + Converter;
            return hash.GetHashCode();
        }

    }
}

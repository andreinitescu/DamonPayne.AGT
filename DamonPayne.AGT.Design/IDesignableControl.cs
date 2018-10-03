using System.Collections.Generic;
using System.Windows.Controls;
using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design
{
    public interface IDesignableControl
    {
        /// <summary>
        /// Visual representation of this IDesignableControl
        /// </summary>
        Control Visual { get; }

        /// <summary>
        /// Return true if this IDesignableControl should be resized by changing Width & Height
        /// </summary>
        bool IsBoundsResizable { get; }

        /// <summary>
        /// Can a ScaleTransform etc. be used to resize this control?
        /// </summary>
        bool IsTransformable { get; }

        string DesignTimeName { get; set; }

        List<DesignablePropertyDescriptor> GetDesignProperties();        
    }
}

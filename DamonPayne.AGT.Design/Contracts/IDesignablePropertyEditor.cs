﻿using System;
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

using DamonPayne.AGT.Design.Types;

namespace DamonPayne.AGT.Design.Contracts
{
    public interface IDesignablePropertyEditor
    {
        /// <summary>
        /// 
        /// </summary>
        Control Visual { get; }

        /// <summary>
        /// The Editor should set up a binding to the appropriate property
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="desc"></param>
        void Initialize(IDesignableControl instance, DesignablePropertyDescriptor desc);
    }
}

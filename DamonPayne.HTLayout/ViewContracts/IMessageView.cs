﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AG.Core.DataTypes;


namespace DamonPayne.HTLayout.ViewContracts
{
    public interface IMessageView
    {
        void AddMessage(LogMessage lm);
    }
}

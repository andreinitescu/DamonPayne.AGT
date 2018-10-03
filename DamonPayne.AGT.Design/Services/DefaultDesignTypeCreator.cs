using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.AGT.Design.Controls;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AG.Core;

namespace DamonPayne.AGT.Design.Services
{
    public class DefaultDesignTypeCreator : IDesignTypeCreator
    {

        public IDesignableControl CreateInstance(Type t)
        {
            if (!t.ImplementsInterface(typeof(IDesignableControl)))
            {
                throw new ArgumentException(t + " does not implement IDesignableControl");
            }
            IDesignableControl idc = null;
            idc = (IDesignableControl)Activator.CreateInstance(t);

            DesignSite site = new DesignSite();
            site.HostedContent = idc;


            return site;
        }

    }
}

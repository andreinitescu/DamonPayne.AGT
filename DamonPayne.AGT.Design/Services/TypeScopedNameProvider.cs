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
using DamonPayne.AGT.Design.Contracts;
using System.Linq;

namespace DamonPayne.AGT.Design.Services
{
    /// <summary>
    /// A name provider that uses the last part of type name plus 
    /// </summary>
    public class TypeScopedNameProvider : INameProvider
    {
        public string GetUniqueName(INamingContainer container, IDesignableControl newChild)
        {
            var type = newChild.GetType();
            string namePart = type.Name;
            int existingCount = (from c in container.Children where c.GetType() == type select c).Count();
            return namePart + existingCount;
        }
    }
}

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

namespace DamonPayne.AG.IoC
{

    /// <summary>
    /// Properties marked with PresenterAttribute had better be of a type that takes a single View in a constructor!
    /// </summary>
    [global::System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class PresenterAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236


        // This is a positional argument
        public PresenterAttribute()
        {
        }
    }
}

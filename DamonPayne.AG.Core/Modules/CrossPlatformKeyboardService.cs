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


namespace DamonPayne.AG.Core.Modules
{
    public class CrossPlatformKeyboardService : IKeyboardService
    {
        /// <summary>
        /// Control key, or open-apple on the Mac "platform"
        /// </summary>
        public bool ControlKeyDown
        {
            get 
            {
                return ( Test(ModifierKeys.Control) || Test(ModifierKeys.Apple));
            }
        }

        public bool AltKeyDown
        {
            get { return Test(ModifierKeys.Alt); }
        }

        public bool ShiftDown
        {
            get { return Test(ModifierKeys.Shift); }
        }

        protected bool Test(ModifierKeys target)
        {
            return target == (target & Keyboard.Modifiers);
        }

    }
}

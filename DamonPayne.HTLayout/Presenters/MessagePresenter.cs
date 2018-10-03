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
using DamonPayne.HTLayout.ViewContracts;

namespace DamonPayne.HTLayout.Presenters
{
    public class MessagePresenter
    {
        public MessagePresenter(IMessageView view)
        {
            View = view;
        }

        public IMessageView View { get; set; }

    }
}

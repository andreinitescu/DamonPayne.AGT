using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using DamonPayne.HTLayout.ViewContracts;
using DamonPayne.HTLayout.Presenters;
using DamonPayne.AG.Core;
using DamonPayne.AG.Core.Events;
using DamonPayne.AG.Core.DataTypes;
using Microsoft.Practices.Unity;

namespace DamonPayne.HTLayout.Controls
{
    public partial class MessageConsoleView : UserControl, IView, IMessageView
    {
        public MessageConsoleView()
        {
            InitializeComponent();
            _currentMessages = new List<LogMessage>();
            DataContext = _currentMessages;
            EventAggregator.Get<MessageArrivedEvent<LogMessage>, LogMessage>()
                .Subscribe(MessagePosted);
            Presenter = new MessagePresenter(this);
        }

        private void MessagePosted(LogMessage m)
        {
            _currentMessages.Add(m);
            ListBoxItem lbi = new ListBoxItem();
            lbi.Content = new TextBlock().Text = m.Message;
            _messages.Items.Insert(0, lbi);
        }
        

        private List<LogMessage> _currentMessages;

        public UserControl VisualRoot
        {
            get 
            {
                return this;
            }
        }

        public void AddMessage(LogMessage lm)
        {
        }

        [Dependency]
        public ILogService LogService { get; set; }

        
        public MessagePresenter Presenter { get; set; }


        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            LogService.Log( 
                new LogMessage
                {
                    Level = LogLevels.Debug,
                    Message = "Message console started"
                }
            );            
        }

        private void _clearBtn_Click(object sender, RoutedEventArgs e)
        {
            _messages.Items.Clear();
        }


    }
}

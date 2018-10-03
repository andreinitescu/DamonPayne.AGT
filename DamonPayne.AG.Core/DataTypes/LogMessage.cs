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

namespace DamonPayne.AG.Core.DataTypes
{
    public class LogMessage
    {
        public LogMessage()
        {
        }

        public LogMessage(string msg, LogLevels l)
        {
            Message = msg;
            Level = l;
        }


        public string Message { get; set; }
        public LogLevels Level { get; set; }
    }

    public enum LogLevels
    {
        Debug = 0,
        Info,
        Warn,
        Error
    }

}

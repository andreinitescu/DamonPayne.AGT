using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;

namespace DamonPayne.AGT.Design.Converters
{
    public class ColorConverter : IValueConverter
    {

        static ColorConverter()
        {
            _colorNameToColor = new Dictionary<string, Color>();
            _colorToName = new Dictionary<Color, string>();
            Type cType = typeof(System.Windows.Media.Colors);
            PropertyInfo[] colors = cType.GetProperties(BindingFlags.Public | BindingFlags.Static);
            foreach (var propInfo in colors)
            {
                var c = (Color)propInfo.GetValue(null,null);
                _colorNameToColor.Add(propInfo.Name, c);
                _colorToName.Add(c, propInfo.Name);
            }
        }

        private static Dictionary<string, Color> _colorNameToColor;
        private static Dictionary<Color, string> _colorToName;


        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Color)
            {
                Color src = (Color)value;
                if (targetType == typeof(string))
                {
                    if (_colorToName.ContainsKey(src))
                    {
                        return _colorToName[src];
                    }
                    else
                    {
                        return src.ToString();
                    }
                }
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is string)
            {
                string src = value.ToString();
                if (targetType == typeof(Color))
                {
                    if (_colorNameToColor.ContainsKey(src))
                    {
                        return _colorNameToColor[src];
                    }
                }
            }

            return null;
        }
    }
}

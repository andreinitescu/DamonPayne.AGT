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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DamonPayne.AGT.Design.Converters;

namespace TestDesign.Converters
{
    [TestClass]
    public class TestColorConverter
    {
        [TestMethod]
        public void Convert()
        {
            ColorConverter cc = new ColorConverter();
            object val = cc.Convert(Colors.Red, typeof(string), null, null);
            Assert.IsNotNull(val);
            Assert.AreEqual<string>("Red", val.ToString());
        }

        [TestMethod]
        public void ConvertBack()
        {
            ColorConverter cc = new ColorConverter();
            object val = cc.ConvertBack("Red", typeof(Color), null, null);
            Assert.IsNotNull(val);
            Assert.AreEqual<Color>(Colors.Red, (Color)val);
        }

    }
}

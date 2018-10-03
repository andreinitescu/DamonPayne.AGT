/*Creative Commons - attribution-noncommerical-share alike 3.0 Unported
          You are free to copy, distribute, and transmit this work.
          You are free to adapt the work.
        Under the following conditions:
          -You must attribute the author(Damon Payne, http://www.damonpayne.com) for this work but not in a way 
          that suggests the author endorses you or your use of the work.
          -You may not use this work for commercial purposes
          -If you alter, transform, or build upon this work, you are free to distribute the work under the same or similar license to this one.
        */
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

namespace DamonPayne.AGT.Design.Types
{
    public class ToolboxItem
    {
        public ToolboxItem():this("Unknown", "No description",null)
        {
        }

        public ToolboxItem(string name, string desc, Type type)
        {
            Name = name;
            Description = desc;
            Type = type;
        }

        //TODO: Provide support for a preview view of the item

        public string Name { get; set; }
        public string Description { get; set; }

        /// <summary>
        /// A type, which should be an IDesignableControl
        /// </summary>
        public Type Type { get; set; }
    }
}

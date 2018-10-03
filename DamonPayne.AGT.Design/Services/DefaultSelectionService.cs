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
using System.Collections.Generic;
using DamonPayne.AGT.Design.Contracts;
using Microsoft.Practices.Unity;
using DamonPayne.AG.Core;

namespace DamonPayne.AGT.Design.Services
{
    public class DefaultSelectionService : ISelectionService
    {
        public DefaultSelectionService()
        {
            _selection = new List<IDesignableControl>();
        }

        [Dependency]
        public IKeyboardService KBService { get; set; }

        private List<IDesignableControl> _selection;

        public void Select(System.Collections.Generic.IList<IDesignableControl> incoming)
        {
            if (!KBService.ControlKeyDown)
            {
                _selection.Clear();
                if (null != incoming)
                {
                    _selection.AddRange(incoming); 
                }
                OnSelectionChanged();            
            }
            else
            {
                foreach (IDesignableControl idc in incoming)
                {
                    if (!_selection.Contains(idc))
                    {
                        _selection.Add(idc);
                    }
                    OnSelectionChanged();            
                }
            }            
        }

        public System.Collections.Generic.IList<IDesignableControl> GetSelection()
        {
            return _selection;
        }

        public int SelectionCount
        {
            get { return _selection.Count; }
        }

        public IDesignableControl PrimarySelection
        {
            get 
            {
                if (_selection.Count > 0)
                {
                    return _selection[0]; 
                }
                return null;
            }
        }

        public event EventHandler SelectionChanged;

        protected void OnSelectionChanged()
        {
            if (null != SelectionChanged)
            {
                SelectionChanged(this, EventArgs.Empty);
            }
        }
    }
}

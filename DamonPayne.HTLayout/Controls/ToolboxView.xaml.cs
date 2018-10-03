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
using DamonPayne.AG.Core;
using DamonPayne.AGT.Design.Contracts;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AG.Core.Events;
using Microsoft.Practices.Unity;

namespace DamonPayne.HTLayout.Controls
{
    public partial class ToolboxView : UserControl, IView, IToolboxService
    {
        public ToolboxView()
        {
            InitializeComponent();
            _categories = new Dictionary<string, ToolboxCategoryControl>();
            _selectionEvent = EventAggregator.Get<SelectedDataChangedEvent<ToolboxItem>, ToolboxItem>();
            _selectionEvent.Subscribe(SelectionChanged);
        }

        private SelectedDataChangedEvent<ToolboxItem> _selectionEvent;

        public void SelectionChanged(ToolboxItem item)
        {
            _selectedItem = item;
            _descriptionBlock.Text = item.Description;
        }

        public UserControl VisualRoot
        {
            get { return this; }
        }

        private Dictionary<string, ToolboxCategoryControl> _categories;

        private ToolboxItem _selectedItem;

        public void AddItem(DamonPayne.AGT.Design.Types.ToolboxItem item)
        {
            AddItem(item, "Default");
        }

        public void AddItem(DamonPayne.AGT.Design.Types.ToolboxItem item, string category)
        {
            EnsureCategory(category);
            _categories[category].Add(item);
        }

        private void EnsureCategory(string category)
        {
            if (!_categories.ContainsKey(category))
            {
                ToolboxCategoryControl cat = new ToolboxCategoryControl();
                cat.CategoryName = category;
                _categories.Add(category, cat);
                _ToolboxCategoriesLst.Items.Add(cat);
                cat.DragStart += new EventHandler(cat_DragStart);
            }
        }

        [Dependency]
        public IDragDropManager DragDropManager { get; set; }

        void cat_DragStart(object sender, EventArgs e)
        {
            ToolboxItem tbi = SelectedItem;//Will already be populated from event wireup
            DragDropManager.BeginDrag(tbi);
        }

        public DamonPayne.AGT.Design.Types.ToolboxItem SelectedItem
        {
            get 
            {
                return _selectedItem;
            }
        }

        public void RemoveItem(DamonPayne.AGT.Design.Types.ToolboxItem item)
        {
            throw new NotImplementedException();
        }

    }
}

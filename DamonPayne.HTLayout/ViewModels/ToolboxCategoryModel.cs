using System.Collections.ObjectModel;
using System.ComponentModel;
using DamonPayne.AGT.Design.Types;
using DamonPayne.AG.Core;
using DamonPayne.AG.Core.Events;

namespace DamonPayne.HTLayout.ViewModels
{
    public class ToolboxCategoryModel : INotifyPropertyChanged
    {
        public ToolboxCategoryModel()
        {
            ToolboxItems = new ObservableCollection<ToolboxItem>();
            _selectionEvent = EventAggregator.Get<SelectedDataChangedEvent<ToolboxItem>, ToolboxItem>();
            _selectionEvent.Subscribe(item => { SelectedToolboxItem = null; }, item => !ToolboxItems.Contains(item));
        }

        private SelectedDataChangedEvent<ToolboxItem> _selectionEvent;

        
        private string _CategoryName;
        public string CategoryName
        {
            get { return _CategoryName; }
            set
            {
                _CategoryName = value;
                OnPropertyChanged("CategoryName");
            }
        }

        private ObservableCollection<ToolboxItem> _ToolboxItems;
        public ObservableCollection<ToolboxItem> ToolboxItems
        {
            get { return _ToolboxItems; }
            set
            {
                _ToolboxItems = value;
                OnPropertyChanged("ToolboxItems");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string pname)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(pname));
            }
        }

        private ToolboxItem _SelectedToolboxItem;
        public ToolboxItem SelectedToolboxItem
        {
            get { return _SelectedToolboxItem; }
            set
            {
                _SelectedToolboxItem = value;
                OnPropertyChanged("SelectedToolboxItem");
                if (null != value)
                {
                    _selectionEvent.Raise(value); 
                }
            }
        }

    }
}

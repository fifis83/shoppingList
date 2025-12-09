using System.Collections.ObjectModel;
using System.ComponentModel;

namespace shoppingList.Models
{
    public class StoreListPageModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private MainPageModel _mainPageModel;

        public ObservableCollection<string> StoreList { get; set; }
        public ObservableCollection<ItemModel> FilteredItems { get; set; } = new ObservableCollection<ItemModel>();

        private int _selectedStoreIndex = -1;
        public int SelectedStoreIndex
        {
            get => _selectedStoreIndex;
            set
            {
                _selectedStoreIndex = value;
                OnPropertyChanged(nameof(SelectedStoreIndex));
                UpdateFilteredItems();
            }
        }

        public StoreListPageModel(MainPageModel mainPageModel)
        {
            _mainPageModel = mainPageModel;

            var stores = new ObservableCollection<string>();
            var uniqueStores = _mainPageModel.Categories
                .SelectMany(cat => cat.Items)
                .Select(item => item.store)
                .Where(store => !string.IsNullOrWhiteSpace(store))
                .Distinct()
                .OrderBy(store => store);

            foreach (var store in uniqueStores)
            {
                stores.Add(store);
            }
            stores.Add(" ");

            StoreList = stores;

            if (StoreList.Count > 0)
            {
                UpdateFilteredItems();
            }
        }

        private void UpdateFilteredItems()
        {
            FilteredItems.Clear();

            if (SelectedStoreIndex >= StoreList.Count)
                return;
            if(SelectedStoreIndex==-1) SelectedStoreIndex=StoreList.Count-1;

            string selectedStore = StoreList[SelectedStoreIndex];

            var items = _mainPageModel.Categories
                .SelectMany(cat => cat.Items)
                .Where(item => item.store == selectedStore)
                .OrderBy(item => item.bought)
                .ThenBy(item => item.name);

            foreach (var item in items)
            {
                item.PropertyChanged += unboughtItem_PropertyChanged;
                FilteredItems.Add(item);
            }

        }

        private void unboughtItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemModel item && e.PropertyName == nameof(item.deleted))
            {

                item.PropertyChanged -= unboughtItem_PropertyChanged;
                if (!FilteredItems.Contains(item)) return;
                FilteredItems.Remove(item);

            }
            OnPropertyChanged(nameof(FilteredItems));
        }
    }
}
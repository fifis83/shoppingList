using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace shoppingList.Models
{
    public class StoreListModel : INotifyPropertyChanged
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

        public StoreListModel(MainPageModel mainPageModel)
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

            StoreList = stores;

            if (StoreList.Count > 0)
            {
                SelectedStoreIndex = 0;
            }
        }

        private void UpdateFilteredItems()
        {
            FilteredItems.Clear();

            if (SelectedStoreIndex < 0 || SelectedStoreIndex >= StoreList.Count)
                return;

            string selectedStore = StoreList[SelectedStoreIndex];

            var items = _mainPageModel.Categories
                .SelectMany(cat => cat.Items)
                .Where(item => item.store == selectedStore && !item.deleted)
                .OrderBy(item => item.bought)
                .ThenBy(item => item.name);

            foreach (var item in items)
            {
                FilteredItems.Add(item);
            }
        }
    }
}
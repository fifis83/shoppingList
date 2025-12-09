using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace shoppingList.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ObservableCollection<ItemModel> Items { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ItemsVisible { get; set; } = "True";        
        
        public MainPageModel mainPageModel;
        
        public ICommand OpenNewItem { get; set; }
        public ICommand ToggleItemsVisibility { get; set; }
        public ICommand DeleteCategory { get; set; }

        public CategoryModel(string name, MainPageModel mainPageModel, ObservableCollection<ItemModel> items = null)
        {
            Name = name;
            Items = items == null ? new() : items;
            this.mainPageModel = mainPageModel;

            Items.CollectionChanged += ItemsCollectionChanged;
            foreach (var item in Items) item.PropertyChanged += ItemPropertyChanged;

            OpenNewItem = new AsyncRelayCommand(OpenNewItemAsync);
            ToggleItemsVisibility = new AsyncRelayCommand(ToggleItemsVisibilityAsync);
            DeleteCategory = new AsyncRelayCommand(DeleteCategoryAsync);
        }

        private async Task OpenNewItemAsync()
        {
            var newPage = new Views.NewItemPage(this);
            var newWindow = new Window { Page = newPage };
            
            newWindow.Height = 500;
            newWindow.Width = 400;
            Application.Current?.OpenWindow(newWindow);
        }

        private async Task DeleteCategoryAsync()
        {
            Items.CollectionChanged -= ItemsCollectionChanged;
            Items.Clear();
            mainPageModel.DeleteCategory(this);
        }

        private async Task ToggleItemsVisibilityAsync()
        {
            ItemsVisible = ItemsVisible == "True" ? "False" : "True";
            OnPropertyChanged(nameof(ItemsVisible));
        }


        public void InsertSorted(ItemModel item)
        {
            if (Items.Count == 0)
            {
                Items.Add(item);
                return;
            }

            if (!item.bought)
            {
                int idx = 0;
                while (idx < Items.Count && !Items[idx].bought) idx++;
                Items.Insert(idx, item);
            }
            else
            {
                Items.Add(item);
            }
            mainPageModel.Save();
        }

        private void ItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ItemModel it in e.NewItems)
                {
                    it.PropertyChanged -= ItemPropertyChanged;
                    it.PropertyChanged += ItemPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ItemModel it in e.OldItems)
                {
                    it.PropertyChanged -= ItemPropertyChanged;
                }
            }
            mainPageModel.Save();
        }

        private void ItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemModel item)
            {
                if (!Items.Contains(item)) return;

                Items.CollectionChanged -= ItemsCollectionChanged;
                try
                {
                    Items.Remove(item);
                    if (e.PropertyName != nameof(ItemModel.deleted)) InsertSorted(item);
                }
                finally
                {
                    Items.CollectionChanged += ItemsCollectionChanged;
                }
            }
            mainPageModel.Save();
        }


    }
}

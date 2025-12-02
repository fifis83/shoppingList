using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace shoppingList.Models
{
    public class CategoryModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public string Name { get; set; } = string.Empty;
        public ObservableCollection<ItemModel> Items { get; set; }
        public ICommand OpenNewItem { get; set; }
        public string ItemsVisible { get; set; } = "True";
        public ICommand ToggleItemsVisibility { get; set; }
        public ICommand DeleteCategory { get; set; }
        public MainPageModel mainPageModel;
        public CategoryModel(string name, MainPageModel mainPageModel, ObservableCollection<ItemModel> items = null)
        {
            Name = name;
            Items = items == null ? new() : items;
            this.mainPageModel = mainPageModel;

            Items.CollectionChanged += Items_CollectionChanged;
            foreach (var item in Items) item.PropertyChanged += Item_PropertyChanged;

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
            Application.Current.OpenWindow(newWindow);
        }

        private async Task DeleteCategoryAsync()
        {
            Items.CollectionChanged -= Items_CollectionChanged;
            Items.Clear();
            mainPageModel.DeleteCategory(this);
        }

        private async Task ToggleItemsVisibilityAsync()
        {
            ItemsVisible = ItemsVisible == "True" ? "False" : "True";
            OnPropertyChanged(nameof(ItemsVisible));
        }
        public void AddItem(ItemModel item)
        {
            InsertSorted(item);
        }

        private void InsertSorted(ItemModel item)
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

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ItemModel it in e.NewItems)
                {
                    it.PropertyChanged -= Item_PropertyChanged;
                    it.PropertyChanged += Item_PropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (ItemModel it in e.OldItems)
                {
                    it.PropertyChanged -= Item_PropertyChanged;
                }
            }
            mainPageModel.Save();
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemModel item)
            {
                if (!Items.Contains(item)) return;

                Items.CollectionChanged -= Items_CollectionChanged;
                try
                {
                    Items.Remove(item);
                    if (e.PropertyName != nameof(ItemModel.deleted)) InsertSorted(item);
                }
                finally
                {
                    Items.CollectionChanged += Items_CollectionChanged;
                }
            }
            mainPageModel.Save();
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

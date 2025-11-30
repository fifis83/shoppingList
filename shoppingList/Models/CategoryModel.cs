using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.Specialized;
using System.ComponentModel;

namespace shoppingList.Models
{
    internal class CategoryModel
    {
        public string Name { get; set; } = string.Empty;
        public ObservableCollection<ItemModel> Items { get; set; }
        public ICommand OpenNewItem { get; set; }

        public CategoryModel(string name, ObservableCollection<ItemModel> items = null)
        {
            Name = name;
            Items = items == null ? new() : items;

            // subscribe to collection changes to track item property changes
            Items.CollectionChanged += Items_CollectionChanged;
            foreach (var item in Items) item.PropertyChanged += Item_PropertyChanged;

            OpenNewItem = new AsyncRelayCommand(OpenNewItemAsync);
        }

        private async Task OpenNewItemAsync()
        {
            var newPage = new Views.NewItemPage(Name);
            var newWindow = new Window { Page = newPage };
            
            newWindow.Height = 500;
            newWindow.Width = 400;
            Application.Current.OpenWindow(newWindow);
            
        }

        public void AddItem(ItemModel item)
        {
                InsertSorted(item);
            // CollectionChanged handler will subscribe to PropertyChanged for the inserted item
        }

        private void InsertSorted(ItemModel item)
        {
            // keep non-bought items before bought items
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
        }

        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ItemModel it in e.NewItems)
                {
                    // avoid double subscription
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
        }

        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemModel.bought) && sender is ItemModel item)
            {
                // Re-position the item in the collection when bought toggles
                if (!Items.Contains(item)) return;

                // temporarily unsubscribe collection changed to avoid double handling
                Items.CollectionChanged -= Items_CollectionChanged;
                try
                {
                    Items.Remove(item);
                    InsertSorted(item);
                }
                finally
                {
                    Items.CollectionChanged += Items_CollectionChanged;
                }
            }
        }
    }
}

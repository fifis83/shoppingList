using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.Input;
using shoppingList.Models;

namespace shoppingList.ViewModels
{
    public class CategoryViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string name) =>
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private CategoryModel category;

        public MainPageViewModel mainPageViewModel;
        public string ItemsVisible { get; set; } = "True";

        public string Name
        {
            get => category.name;
            set
            {
                category.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ObservableCollection<ItemViewModel> Items => category.items;

        public ICommand OpenNewItem { get; set; }
        public ICommand ToggleItemsVisibility { get; set; }
        public ICommand DeleteCategory { get; set; }
        public ICommand DeleteItemCommand { get; set; }
        public ICommand BuyItemCommand { get; set; }

        public CategoryViewModel(string name, MainPageViewModel mainPageModel, ObservableCollection<ItemViewModel> items = null)
        {
            category = new CategoryModel(name, items == null ? new() : items);
            this.mainPageViewModel = mainPageModel;


            OpenNewItem = new AsyncRelayCommand(OpenNewItemAsync);
            ToggleItemsVisibility = new AsyncRelayCommand(ToggleItemsVisibilityAsync);
            DeleteCategory = new AsyncRelayCommand(DeleteCategoryAsync);
            DeleteItemCommand = new Command((sender) => DeleteItem((ItemViewModel)sender));
            BuyItemCommand = new Command((sender) => BuyItem((ItemViewModel)sender));
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
            Items.Clear();
            mainPageViewModel.DeleteCategory(this);
        }

        private async Task ToggleItemsVisibilityAsync()
        {
            ItemsVisible = ItemsVisible == "True" ? "False" : "True";
            OnPropertyChanged(nameof(ItemsVisible));
        }

        public void InsertSorted(ItemViewModel item)
        {
            if (Items.Count == 0)
            {
                Items.Add(item);
                return;
            }

            if (item.Bought)
            {
                Items.Add(item);
            }
            else
            {
                int idx = 0;
                while (idx < Items.Count && !Items[idx].Bought) idx++;
                Items.Insert(idx, item);
            }
            mainPageViewModel.Save();
        }

        private void DeleteItem(ItemViewModel item)
        {
            Items.Remove(item);
            mainPageViewModel.Save();
        }
        private void BuyItem(ItemViewModel item)
        {
            Application.Current?.Dispatcher.Dispatch(() =>
            {
                Items.Remove(item);
                InsertSorted(item);

            });
        }
    }
}

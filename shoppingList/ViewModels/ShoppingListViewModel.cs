using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shoppingList.ViewModels
{
    public class ShoppingListViewModel : ContentPage
    {

        public ObservableCollection<ItemViewModel> unboughtItems { get; set; } = new ObservableCollection<ItemViewModel>();
        MainPageViewModel mainPage;

        public ShoppingListViewModel(MainPageViewModel mainPage)
        {
            this.mainPage = mainPage;

            List<ItemViewModel> items = new();

            foreach (var category in mainPage.Categories)
            {
                foreach (var item in category.Items)
                {
                    if (item.Bought) continue;
                    item.PropertyChanged += unboughtItem_PropertyChanged;
                    unboughtItems.Add(item);
                }
            }
        }


        private void unboughtItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemViewModel item)
            {
                item.PropertyChanged -= unboughtItem_PropertyChanged;
                if (!unboughtItems.Contains(item)) return;
                unboughtItems.Remove(item);

            }
            OnPropertyChanged(nameof(unboughtItems));
            mainPage.Save();
        }
    }
}

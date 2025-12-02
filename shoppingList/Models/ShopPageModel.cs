using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace shoppingList.Models
{
    public partial class ShopPageModel : ContentPage,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<ItemModel> unboughtItems{ get; set; } = new ObservableCollection<ItemModel>();
        MainPageModel mainPageModel;
        public ShopPageModel(MainPageModel mainPage)
        {
            mainPageModel = mainPage;
            
            List<ItemModel> items = new();

            foreach (var category in mainPage.Categories)
            {
                foreach (var item in category.Items)
                {
                    if (item.bought) continue;
                    item.PropertyChanged += unboughtItem_PropertyChanged;
                    unboughtItems.Add(item);
                }
            }
        }


        private void unboughtItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (sender is ItemModel item && e.PropertyName == nameof(item.bought))
            {
                item.PropertyChanged -= unboughtItem_PropertyChanged;
                if (!unboughtItems.Contains(item)) return;
                unboughtItems.Remove(item);
            }
            mainPageModel.Save();
        }
    }
}

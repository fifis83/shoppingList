using System.Collections.ObjectModel;
using System.ComponentModel;

namespace shoppingList.Models
{
    public partial class ShoppingListPageModel : ContentPage
    {

        public ObservableCollection<ItemModel> unboughtItems{ get; set; } = new ObservableCollection<ItemModel>();
        MainPageModel mainPageModel;

        public ShoppingListPageModel(MainPageModel mainPage)
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
            if (sender is ItemModel item)
            {
                item.PropertyChanged -= unboughtItem_PropertyChanged;
                if (!unboughtItems.Contains(item)) return;
                unboughtItems.Remove(item);

            }
            OnPropertyChanged(nameof(unboughtItems));
            mainPageModel.Save();
        }
    }
}

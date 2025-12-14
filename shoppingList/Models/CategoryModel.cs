using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using shoppingList.ViewModels;

namespace shoppingList.Models
{
    public class CategoryModel
    {
        public ObservableCollection<ItemViewModel> items { get; set; }
        public string name { get; set; } = string.Empty;
        public CategoryModel(string name, ObservableCollection<ItemViewModel> items)
        {
            this.name = name;
            this.items = items;
        }
    }
}

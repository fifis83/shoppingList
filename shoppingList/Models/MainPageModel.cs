using System.Collections.ObjectModel;
using shoppingList.Views;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;
using System.Diagnostics;
using shoppingList.ViewModels;


namespace shoppingList.Models
{
    public partial class MainPageModel
    {
        public ObservableCollection<CategoryViewModel> categories { get; set; } = new ObservableCollection<CategoryViewModel>();
      
    }
}

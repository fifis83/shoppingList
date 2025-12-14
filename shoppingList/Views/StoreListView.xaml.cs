using shoppingList.Models;
using shoppingList.ViewModels;

namespace shoppingList.Views
{
    public partial class StoreListView : ContentPage
    {
        public StoreListView(MainPageViewModel mainPageModel)
        {
            InitializeComponent();
            BindingContext = new StoreListViewModel(mainPageModel);
        }
    }
}
using shoppingList.Models;

namespace shoppingList.Views
{
    public partial class StoreListView : ContentPage
    {
        public StoreListView(MainPageModel mainPageModel)
        {
            InitializeComponent();
            BindingContext = new StoreListModel(mainPageModel);
        }
    }
}
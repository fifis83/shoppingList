using shoppingList.Models;
using shoppingList.ViewModels;

namespace shoppingList;

public partial class ShopPageView : ContentPage
{
	public ShopPageView(MainPageViewModel mainPage)
	{
		InitializeComponent();
		BindingContext = new ShoppingListViewModel(mainPage);
	}
}
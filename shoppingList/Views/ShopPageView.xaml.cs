using shoppingList.Models;

namespace shoppingList;

public partial class ShopPageView : ContentPage
{
	public ShopPageView(MainPageModel mainPage)
	{
		InitializeComponent();
		BindingContext = new ShopPageModel(mainPage);
	}
}
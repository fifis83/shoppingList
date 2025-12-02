namespace shoppingList
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ShopPageView), typeof(ShopPageView));
        }
    }
}

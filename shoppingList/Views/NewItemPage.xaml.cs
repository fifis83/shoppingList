using shoppingList.Models;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;

namespace shoppingList.Views;

public partial class NewItemPage : ContentPage
{
    public NewItemPage(CategoryModel cat)
    {
        InitializeComponent();
        BindingContext = new NewItemPageModel(cat,this);

    }


}
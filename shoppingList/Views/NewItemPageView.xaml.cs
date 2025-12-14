using shoppingList.Models;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;
using shoppingList.ViewModels;

namespace shoppingList.Views;

public partial class NewItemPage : ContentPage
{
    public NewItemPage(CategoryViewModel cat)
    {
        InitializeComponent();
        BindingContext = new NewItemPageViewModel(cat,this);

    }


}
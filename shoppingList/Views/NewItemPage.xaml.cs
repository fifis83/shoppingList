using shoppingList.Models;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Linq;

namespace shoppingList.Views;

public partial class NewItemPage : ContentPage
{
    string catName = string.Empty;
    public NewItemPage(string catName)
    {
        InitializeComponent();
        this.catName = catName;

        if (this.FindByName<Entry>("categoryInput") is Entry catEntry)
        {
            catEntry.Text = catName;
            catEntry.IsEnabled = false;
        }
    }

    async private void Button_Clicked(object sender, EventArgs e)
    {
        int amount = 0;
        if (unitInput.SelectedIndex == -1 || !int.TryParse(amountInput.Text, out amount) || string.IsNullOrWhiteSpace(nameInput.Text))
        {
            await this.DisplayAlert("Uwaga", "Wpisz wszystkie dane poprawnie", "OK daddy");
            return;
        }

        var model = new ItemModel
        {
            name = nameInput.Text.Trim(),
            unit = unitInput.SelectedItem?.ToString() ?? string.Empty,
            Optional = optInput.IsChecked,
            bought = false,
            amount = amount
        };

        var targetName = (catName ?? string.Empty).Trim();

        if (Application.Current?.MainPage?.BindingContext is MainPageModel vm)
        {
            var category = vm.Categories.FirstOrDefault(c => string.Equals(c.Name?.Trim(), targetName, StringComparison.OrdinalIgnoreCase));

            category.AddItem(model);
        }
        try
        {
            if (Window != null)
            {
                Application.Current?.CloseWindow(Window);
                return;
            }
        }
        catch { }

    }
}
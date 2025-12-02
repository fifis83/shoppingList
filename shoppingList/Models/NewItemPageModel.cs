using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace shoppingList.Models;

public class NewItemPageModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private readonly Page _page;
    public CategoryModel catOrigin;

    public ObservableCollection<string> storeList { get; set; } = new ObservableCollection<string>() { "Biedronka", "Lidl", "Dodaj..." };
    private int _storeIndex = -1;
    public bool AddStoreVisible { get; set; } = false;

    public int StoreIndex
    {
        get => _storeIndex;
        set
        {
            _storeIndex = value;
            if (value == storeList.Count - 1)
            {
                AddStoreVisible = true;
            }
            else
            {
                AddStoreVisible = false;
            }
                OnPropertyChanged(nameof(AddStoreVisible));
                OnPropertyChanged(nameof(StoreIndex));
        }
    }

    public string nameInput { get; set; } = string.Empty;
    public string amountInput { get; set; } = string.Empty;
    public string unitInput { get; set; } = null;
    public bool optInput { get; set; } = false;
    public ICommand AddStore { get; set; }
    public ICommand AddItem { get; set; }

    public NewItemPageModel(CategoryModel cat, Page page)
    {
        catOrigin = cat;
        _page = page;
        AddStore = new AsyncRelayCommand(AddStoreAsync);
        AddItem = new AsyncRelayCommand(AddItemAsync);
    }

    private async Task AddItemAsync()
    {
        int amount = 0;

        if (unitInput == null || !int.TryParse(amountInput, out amount) || string.IsNullOrWhiteSpace(nameInput) || _storeIndex == storeList.Count - 1 || _storeIndex == -1)
        {
            await _page.DisplayAlert("Uwaga", "Wpisz wszystkie dane poprawnie", "OK");
            return;
        }

        var model = new ItemModel
        {
            name = nameInput,
            unit = unitInput ?? string.Empty,
            Optional = optInput,
            bought = false,
            amount = amount,
            store = storeList[_storeIndex]
        };

        catOrigin.AddItem(model);

        try
        {
            if (_page.Window != null)
            {
                Application.Current?.CloseWindow(_page.Window);
            }
        }
        catch{}
    }

    private async Task AddStoreAsync()
    {
        string result = await _page.DisplayPromptAsync("Nowy sklep", "Nazwa Sklepu:");
        if (string.IsNullOrWhiteSpace(result))
        {
            await _page.DisplayAlert("Uwaga", "Wpisz nazwe poprawnie", "OK");
            return;
        }
        string storeName = storeList.FirstOrDefault(s => s.ToLower().Trim() == result.Trim().ToLower(), "Dodaj...");
        if (storeName != "Dodaj...")
        {
            _storeIndex = storeList.IndexOf(storeName);
            OnPropertyChanged(nameof(_storeIndex));
            return;
        }

        storeList.Insert(StoreIndex, result);
        AddStoreVisible = false;
        OnPropertyChanged(nameof(AddStoreVisible));

        OnPropertyChanged(nameof(_storeIndex));
        OnPropertyChanged(nameof(storeList));
    }
}
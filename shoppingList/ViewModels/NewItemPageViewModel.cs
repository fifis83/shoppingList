using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using shoppingList.Models;

namespace shoppingList.ViewModels
{
    public class NewItemPageViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly Page _page;
        public CategoryViewModel catOrigin;

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

        public NewItemPageViewModel(CategoryViewModel cat, Page page)
        {
            catOrigin = cat;
            _page = page;
            AddStore = new AsyncRelayCommand(AddStoreAsync);
            AddItem = new AsyncRelayCommand(AddItemAsync);
        }

        private async Task AddItemAsync()
        {
            float amount = 0;

            if (unitInput == null ||
                !float.TryParse(amountInput, out amount) ||
                string.IsNullOrWhiteSpace(nameInput) ||
                _storeIndex == storeList.Count - 1 ||
                amount <= 0)
            {
                await _page.DisplayAlert("Uwaga", "Wpisz wszystkie dane poprawnie", "OK");
                return;
            }

            var model = new ItemModel
            {
                name = nameInput,
                unit = unitInput ?? string.Empty,
                optional = optInput,
                bought = false,
                amount = amount,
                store = _storeIndex == -1 ? " " : storeList[_storeIndex]
            };

            catOrigin.InsertSorted(new ItemViewModel(model, catOrigin.DeleteItemCommand, catOrigin.BuyItemCommand));

            if (_page.Window != null)
            {
                Application.Current?.CloseWindow(_page.Window);
            }
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
}

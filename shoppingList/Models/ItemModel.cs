using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace shoppingList.Models
{
    public class ItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public string bgColor { get; set; } = "#1f1f1f";
        public string textColor { get; set; } = "#ffffff";
        public TextDecorations textDecor{ get; set; } = TextDecorations.None;
        public string name { get; set; } = string.Empty;
        public float amount { get; set; } = 0;
        public string unit { get; set; } = string.Empty;
        public string store { get; set; } = string.Empty;

        private bool _bought = false;
        public bool deleted = false;
        public bool bought
        {
            get => _bought;
            set
            {
                if (_bought == value) return;
                _bought = value;

                    OnPropertyChanged(nameof(bought));
                if (value) BoughtAppearance();
                else NotBoughtAppearance();

            }
        }

        private bool _optional = false;
        public bool Optional
        {
            get => _optional;
            set
            {
                _optional = value;
                optionalText = value ? "opcjonalne" : string.Empty;
                OnPropertyChanged(nameof(Optional));
                OnPropertyChanged(nameof(optionalText));
            }
        }
        public string optionalText { get; set; } = string.Empty;

        public void BoughtAppearance()
        {
            bgColor = "#232323";
            textColor = "#757575";
            textDecor = TextDecorations.Strikethrough;
            OnPropertyChanged(nameof(bgColor));
            OnPropertyChanged(nameof(textDecor));
            OnPropertyChanged(nameof(textColor));
        }
        public void NotBoughtAppearance()
        {
            bgColor = "#1f1f1f";
            textColor = "#ffffff";
            textDecor = TextDecorations.None;
            OnPropertyChanged(nameof(bgColor));
            OnPropertyChanged(nameof(textDecor));
            OnPropertyChanged(nameof(textColor));
        }

        public ICommand DeleteItemButton { get; }

        public ItemModel()
        {
            DeleteItemButton = new AsyncRelayCommand(DeleteItemAsync);
        }

        private async Task DeleteItemAsync()
        {
            deleted = true;
            OnPropertyChanged(nameof(deleted));
        }
    }
}
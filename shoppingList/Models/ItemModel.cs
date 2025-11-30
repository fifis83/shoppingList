using System.ComponentModel;

namespace shoppingList.Models
{
    internal class ItemModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string name { get; set; } = string.Empty;
        public float amount { get; set; } = 0;
        public string unit { get; set; } = string.Empty;

        private bool _bought = false;
        public bool bought
        {
            get => _bought;
            set
            {
                if (_bought == value) return;
                _bought = value;
                OnPropertyChanged(nameof(bought));
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
    }
}
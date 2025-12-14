using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using shoppingList.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace shoppingList.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public ICommand DeleteItemButton { get; }
        public ICommand BuyItemCommand{ get; }

        public string BgColor { get; set; } = "#1f1f1f";
        public string TextColor { get; set; } = "#ffffff";
        public TextDecorations TextDecor { get; set; } = TextDecorations.None;

        public string OptionalText { get; set; } = string.Empty;

        ItemModel item { get; }

        public string Name
        {
            get => item.name;
            set
            {
                item.name = value;
                OnPropertyChanged(nameof(Name));
            }
        }
        public float Amount
        {
            get => item.amount;
            set
            {
                item.amount = value;
                OnPropertyChanged(nameof(Amount));
            }
        }
        public string Unit
        {
            get => item.unit;
            set
            {
                item.unit = value;
                OnPropertyChanged(nameof(Unit));
            }
        }
        public string Store
        {
            get => item.store;
            set
            {
                item.store = value;
                OnPropertyChanged(nameof(Store));
            }
        }
        public bool Deleted
        {
            get => item.deleted;
            set
            {
                if (value == item.deleted) return;
                item.deleted = value;
                OnPropertyChanged(nameof(Deleted));
            }
        }

        public bool Bought
        {
            get => item.bought;
            set
            {
                if (value) BoughtAppearance();
                else NotBoughtAppearance();
                if (item.bought == value) return;
                item.bought = value;
                Debug.WriteLine(Name+" Bought "+Bought);

                OnPropertyChanged(nameof(Bought));
                BuyItemCommand.Execute(this);
            }
        }

        public bool Optional
        {
            get => item.optional;
            set
            {
                item.optional = value;
                OptionalText = value ? "opcjonalne" : string.Empty;
                OnPropertyChanged(nameof(Optional));
            }
        }


        public ItemViewModel(ItemModel model,ICommand itemDeleted,ICommand itemBought)
        {
            item = model;
            DeleteItemButton = new Command(()=> { Deleted = true; itemDeleted.Execute(this); });
            BuyItemCommand = new Command(() => itemBought.Execute(this));
        }

        public void BoughtAppearance()
        {
            BgColor = "#232323";
            TextColor = "#757575";
            TextDecor = TextDecorations.Strikethrough;

            OnPropertyChanged(nameof(BgColor));
            OnPropertyChanged(nameof(TextDecor));
            OnPropertyChanged(nameof(TextColor));
        }

        public void NotBoughtAppearance()
        {
            BgColor = "#1f1f1f";
            TextColor = "#ffffff";
            TextDecor = TextDecorations.None;

            OnPropertyChanged(nameof(BgColor));
            OnPropertyChanged(nameof(TextDecor));
            OnPropertyChanged(nameof(TextColor));
        }
    }
}

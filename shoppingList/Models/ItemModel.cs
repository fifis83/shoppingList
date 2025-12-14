using System.ComponentModel;
using System.Globalization;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace shoppingList.Models
{
    public class ItemModel
    {
        public string name { get; set; } = string.Empty;
        public float amount { get; set; } = 0;
        public string unit { get; set; } = string.Empty;
        public string store { get; set; } = string.Empty;
        public bool optional = false;
        public bool bought = false;
        public bool deleted = false;
    }
}
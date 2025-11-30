using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace shoppingList.Models
{
    internal partial class MainPageModel : ContentPage
    {
        // Make this an instance property and use PascalCase to match binding conventions
        public ObservableCollection<CategoryModel> Categories { get; set; } = new ObservableCollection<CategoryModel>();
        //string path = Path.Combine(FileSystem.AppDataDirectory, "List.xml");
        string path = "C:\\Users\\filip\\Desktop\\galus";

        public int CatIndex { get; set; } = -1;
        public IList<string> CategoryPickerList => new List<string>() { "AGD", "Jedzenie","Costam","Inne"};

        public ICommand NewCategory { get; set; }
        public MainPageModel()
        {
            Categories = Load();
            NewCategory = new AsyncRelayCommand(NewCategoryAsync);
        }

        async Task NewCategoryAsync()
        {
            // If nothing selected, show alert on the app's active main page and return.
            if (CatIndex == -1) return;
            string catName = CategoryPickerList[CatIndex];
            string result;
            if (CatIndex == 3)
            {
                result = await Application.Current?.MainPage.DisplayPromptAsync("Nowa kategoria", "Nazwa kategorii:");
                if (!string.IsNullOrWhiteSpace(result)) catName = result;
                else return;
            }

            // Add own category
            Categories.Add(new CategoryModel(catName));

            // Clear the input and notify the UI
            CatIndex = -1;
            OnPropertyChanged(nameof(CatIndex));
        }

        private ObservableCollection<CategoryModel> Load()
        {
            if (!File.Exists(path)) return new();

            string contents = File.ReadAllText(path);
            if (string.IsNullOrWhiteSpace(contents)) return new();

            XElement root;
            try
            {
                root = XElement.Parse(contents);
            }
            catch
            {
                return new();
            }

            var catList = new List<CategoryModel>();

            foreach (var cat in root.Element("categories").Elements())
            {
                catList.Add(new CategoryModel(cat.Name.LocalName));
            }

            foreach (var itemElem in root.Element("items").Elements())
            {
                var name = itemElem.Name.LocalName ?? string.Empty;

                int amount = 0;
                string unit = string.Empty;
                bool optional = false;
                bool bought = false;
                string catName = string.Empty;

                var amountEl = itemElem.Element("amount");
                if (amountEl != null) int.TryParse(amountEl.Value, out amount);

                var unitEl = itemElem.Element("unit");
                if (unitEl != null) unit = unitEl.Value;

                var catEl = itemElem.Element("category");
                if (catEl != null) catName = catEl.Value;

                var optEl = itemElem.Element("optional");
                if (optEl != null) optional = bool.Parse(optEl.Value);

                var boughtEl = itemElem.Element("bought");
                if (boughtEl != null) bought = bool.Parse(boughtEl.Value);

                var model = new ItemModel
                {
                    name = name,
                    unit = unit,
                    Optional = optional,
                    bought = bought,
                    amount = amount
                };

                if (catList.Where(cat => cat.Name == catName).Count() == 0)
                {
                    catList.Add(new CategoryModel(catName));
                }

                catList.ForEach((cat) =>
                {
                    if (cat.Name == catName)
                    {
                        cat.Items.Add(model);
                    }
                });
            }

            return new ObservableCollection<CategoryModel>(catList);
        }

        public async Task Save()
        {
            XElement catsXML = new XElement("categories");

            XElement itemsXML = new XElement("items");
            foreach (var cat in Categories)
            {
                catsXML.Add(cat.Name);
                foreach (var item in cat.Items)
                {
                    itemsXML.Add(new XElement(item.name,
                        new XElement("amount", item.amount),
                        new XElement("unit", item.unit),
                        new XElement("optional", item.Optional),
                        new XElement("bought", item.bought),
                        new XElement("category", cat.Name)));
                }
            }
            XElement list = new XElement("list", itemsXML, catsXML);
            File.WriteAllText(path, list.ToString());
        }
    }
}

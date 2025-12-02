using System.Collections.ObjectModel;
using shoppingList.Views;
using System.Windows.Input;
using System.Xml.Linq;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.Input;


namespace shoppingList.Models
{
    public partial class MainPageModel : ContentPage
    {

        //TODO: zrób aplikacje, tym razem bez czatu
        public ObservableCollection<string> storeList { get; set; } = new ObservableCollection<string>() { "Biedronka", "Lidl", "Dodaj..." };
        public ObservableCollection<CategoryModel> Categories { get; set; } = new ObservableCollection<CategoryModel>();
        string path = Path.Combine(FileSystem.AppDataDirectory, "List.xml");
        public int CatIndex { get; set; } = -1;
        public IList<string> CategoryPickerList => new List<string>() { "AGD", "Jedzenie", "Costam", "Inne" };
        public ICommand NewCategory { get; set; }
        public ICommand ShowShoppingList { get; set; }
        public ICommand ShowStoreList { get; set; }
        public ICommand ExportList { get; set; }
        public ICommand ImportList { get; set; }
        public MainPageModel()
        {
            Categories = Load();
            NewCategory = new AsyncRelayCommand(NewCategoryAsync);
            ShowShoppingList = new AsyncRelayCommand(ShowShoppingListAsync);
            ShowStoreList = new AsyncRelayCommand(ShowStoreListAsync);
            ExportList = new AsyncRelayCommand(ExportListAsync);
            ImportList = new AsyncRelayCommand(ImportListAsync);
        }

        async Task NewCategoryAsync()
        {
            if (CatIndex == -1) return;
            string catName = CategoryPickerList[CatIndex];
            string result;
            if (CatIndex == 3)
            {
                result = await Shell.Current.CurrentPage.DisplayPromptAsync("Nowa kategoria", "Nazwa kategorii:");
                if (!string.IsNullOrWhiteSpace(result)) catName = result;
                else return;
            }

            Categories.Add(new CategoryModel(catName, this));

            CatIndex = -1;
            Save();
            OnPropertyChanged(nameof(CatIndex));
        }


        private async Task ShowShoppingListAsync()
        {
            await Shell.Current.Navigation.PushAsync(new ShopPageView(this));
        }
        private async Task ShowStoreListAsync()
        {
            await Shell.Current.Navigation.PushAsync(new StoreListView(this));
        }
        public void DeleteCategory(CategoryModel category)
        {
            Categories.Remove(category);
            Save();
        }

        private ObservableCollection<CategoryModel> Load(string importFile = "")
        {
            string contents;

            if (!string.IsNullOrWhiteSpace(importFile))
            {
                contents = importFile;
            }
            else
            {
                if (!File.Exists(path)) return new();
                contents = File.ReadAllText(path);
            }

            if (string.IsNullOrWhiteSpace(contents)) return new();

            XElement root;
            try { root = XElement.Parse(contents); }
            catch { return new(); }

            var catList = new List<CategoryModel>();

            foreach (var catXML in root.Elements())
            {
                CategoryModel cat = new CategoryModel(catXML.Name.LocalName ?? string.Empty, this);

                foreach (var itemXML in catXML.Elements())
                {
                    var name = itemXML.Name.LocalName ?? string.Empty;

                    int amount = 0;
                    string unit = string.Empty;
                    string store = string.Empty;
                    bool optional = false;
                    bool bought = false;

                    var amountEl = itemXML.Element("amount");
                    if (amountEl != null) int.TryParse(amountEl.Value, out amount);

                    var unitEl = itemXML.Element("unit");
                    if (unitEl != null) unit = unitEl.Value;

                    var storeEl = itemXML.Element("store");
                    if (storeEl != null) store = storeEl.Value;

                    var optEl = itemXML.Element("optional");
                    if (optEl != null) optional = bool.Parse(optEl.Value);

                    var boughtEl = itemXML.Element("bought");
                    if (boughtEl != null) bought = bool.Parse(boughtEl.Value);

                    var model = new ItemModel
                    {
                        name = name,
                        unit = unit,
                        Optional = optional,
                        bought = bought,
                        amount = amount,
                        store = store
                    };

                    cat.AddItem(model);
                }
                catList.Add(cat);
            }

            return new ObservableCollection<CategoryModel>(catList);
        }
        public void Save()
        {
            XElement catsXML = new XElement("categories");
            foreach (var cat in Categories)
            {
                XElement categoryXML = new XElement(cat.Name);
                foreach (var item in cat.Items)
                {
                    categoryXML.Add(
                        new XElement(item.name,
                        new XElement("amount", item.amount),
                        new XElement("unit", item.unit),
                        new XElement("optional", item.Optional),
                        new XElement("store", item.store),
                        new XElement("bought", item.bought)));
                }
                catsXML.Add(categoryXML);
            }
            File.WriteAllText(path, catsXML.ToString());
        }
        public async Task ExportListAsync()
        {
            Save();
            Stream str = File.OpenRead(path);
            var fileSaverResult = await FileSaver.SaveAsync("Lista.list.xml", str);

            str.Close();
        }
        public async Task ImportListAsync()
        {
            try
            {
                var result = await FilePicker.Default.PickAsync();
                if (result != null)
                {
                    if (result.FileName.EndsWith(".list.xml", StringComparison.OrdinalIgnoreCase))
                    {
                        string xmlString = await File.ReadAllTextAsync(result.FullPath);

                        bool replace = await Shell.Current.CurrentPage.DisplayAlert(
                            "Import",
                            "Czy chcesz zastąpić obecną listę?\n\nTAK - Zastąp wszystko\nNIE - Dodaj do obecnej listy",
                            "TAK",
                            "NIE");

                        if (replace)
                        {
                            Categories.Clear();
                            var imported = Load(xmlString);
                            foreach (var cat in imported)
                            {
                                Categories.Add(cat);
                            }
                        }
                        else
                        {
                            var imported = Load(xmlString);
                            foreach (var cat in imported)
                            {
                                Categories.Add(cat);
                            }
                        }

                        Save();
                    }
                    else
                    {
                        await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Wybierz plik .list.xml", "OK");
                    }
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie udało się zaimportować: {ex.Message}", "OK");
            }
        }
    }
}

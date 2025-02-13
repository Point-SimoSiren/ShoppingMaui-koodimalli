using Newtonsoft.Json;
using ShoppingBackend.Models;
using System.Collections.ObjectModel;


namespace ShoppingMaui
{
    public partial class MainPage : ContentPage
    {


        public MainPage()
        {
            InitializeComponent();
            LoadDataFromRestAPI();

        }

    
    // LISTAN HAKEMINEN BACKENDISTÄ
    public async Task LoadDataFromRestAPI()
        {
           
            // Latausilmoitus näkyviin
            Loading_label.IsVisible = true;
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendope.azurewebsites.net");
            string json = await client.GetStringAsync("/api/shoplist/");

            // Deserialisoidaan json muodosta C# muotoon
            IEnumerable<Shoplist> ienumShoplist = JsonConvert.DeserializeObject<Shoplist[]>(json);

            ObservableCollection<Shoplist> Shoplist = new ObservableCollection<Shoplist>(ienumShoplist);

            itemList.ItemsSource = Shoplist;

            // Latausilmoitus piiloon
            Loading_label.IsVisible = false;

            // Stop the refresh animation
            refreshView.IsRefreshing = false;
        }


        // Lisäyssivulle navigoiminen
        private async void addPageBtn_Clicked(object sender, EventArgs e)
        {
            var addingPage = new AddingPage(this);
            await Shell.Current.Navigation.PushModalAsync(addingPage);
        }

        // PULL TO REFRESH toiminto 
        private void refreshView_Refreshing(object sender, EventArgs e)
        {
            LoadDataFromRestAPI();
        }


        private void itemList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            Shoplist? selectedItem = itemList.SelectedItem as Shoplist;
            kerätty_nappi.Text = "Poimi " + selectedItem?.Item;
        }

        // Tuotteen poistaminen listalta kun se on kerätty
        async void kerätty_nappi_Clicked(object sender, EventArgs e)
        {
            Shoplist? selected = itemList.SelectedItem as Shoplist;

            if (selected == null)
            {
                await DisplayAlert("Valinta puuttuu", "Valitse ensin poimittava tuote", "ok");
                return;
            }

            bool answer = await DisplayAlert("Menikö oikein?", selected.Item + " kerätty?", "Yes! Kyllä meni!", "Ei, Yritän uusiksi");
            if (answer == false)
            {
                return;
            }

            // Jos kaikki on hyvin tuote poistetaan
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://shoppingbackendope.azurewebsites.net");
            HttpResponseMessage res = await client.DeleteAsync("/api/shoplist/" + selected.Id);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                await LoadDataFromRestAPI();
            }
            else
            {
                await DisplayAlert("Tilapäinen virhe", "Joku muu on saattanut poistaa tuotteen sen jälkeen kun listauksesi on viimeeksi päivittynyt?",
                    "Lataa uudelleen");
               await LoadDataFromRestAPI();
               
            }


        }
    }
}

using Newtonsoft.Json;
using ShoppingBackend.Models;
using System.Linq.Expressions;
using System.Text;

namespace ShoppingMaui;

public partial class AddingPage : ContentPage
{
    // Alustus MainPagelle jotta t��ll� voidaan kutsua sen sis�lt�m�� metodia LoadDataFromRestAPI()"
    private MainPage _mainPage;

    public AddingPage(MainPage mainPage)
	{
		InitializeComponent();

        _mainPage = mainPage; // Jotta voidaan kutsua edelt�v�n kommentin mukaisesti...
	}


    // Kun uusi tuote on sy�tetty ja painetaan "tallennus" nappia:
    private async void AddBtn_Clicked(object sender, EventArgs e)
    {

        if (string.IsNullOrEmpty(ItemField.Text))
        {
            await DisplayAlert("Tieto puuttuu", "Anna tuote", "Ok");
            return;
        }

        else
        {

            Shoplist newItem = new Shoplist(); // Luodaan uusi olio tallennusta varten

            newItem.Item = ItemField.Text; // Asetetaan olion Item ominaisuus

            if (string.IsNullOrEmpty(AmountField.Text)) // Jos lukum��r�� ei annettu se on 1
            {
                newItem.Amount = 1;
            }
            else
            {
                newItem.Amount = int.Parse(AmountField.Text);
            }

            // Muutetaan em. data objekti Jsoniksi
            var input = JsonConvert.SerializeObject(newItem);
            HttpContent content = new StringContent(input, Encoding.UTF8, "application/json");

            // L�hetet��n json objekti backendille http post -pyynn�ll�
            HttpClient client = new();
            client.BaseAddress = new Uri("https://shoppingbackendope.azurewebsites.net");
            HttpResponseMessage res = await client.PostAsync("/api/shoplist/", content);

            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                ItemField.Text = "";
                AmountField.Text = "";


                await _mainPage.LoadDataFromRestAPI();
                // Suljetaan modaali sivu
                await Shell.Current.Navigation.PopModalAsync();
                


            }
            else
            {
                await DisplayAlert("Virhe ohjelmassa", "Ota yhteytt� kehitt�jiin", "ok");
            }
        }
    }

}
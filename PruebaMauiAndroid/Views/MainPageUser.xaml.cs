using LibraryClienteAgenda;

namespace PruebaMauiAndroid.Views;

public partial class MainPageUser : ContentPage
{
    public MainPageUser()
    {
        InitializeComponent();
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {


        var succes = await ServerConnection.UserLogout();

        if (succes)
        {
            await Navigation.PopModalAsync();
            //await App.Current.MainPage.DisplayAlert("info", "Closing session", "OK");

        }
    }
}
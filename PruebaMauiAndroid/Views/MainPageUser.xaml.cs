using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class MainPageUser : ContentPage
{
    public MainPageUser()
    {
        InitializeComponent();
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {


        var succes = await ServerConnection.UserLogout();

        if (succes == ResponseStatus.ACTION_SUCCESS)
        {
            await Navigation.PopModalAsync();
            //await App.Current.MainPage.DisplayAlert("info", "Closing session", "OK");

        }
    }
}
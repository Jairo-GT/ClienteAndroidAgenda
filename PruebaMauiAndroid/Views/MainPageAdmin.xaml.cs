using LibraryClienteAgenda;

namespace PruebaMauiAndroid.Views;

public partial class MainPageAdmin : ContentPage
{
    public List<string> Users { get; set; } = new List<string>() { "User1", "User2", "User3" };


    public MainPageAdmin()
    {
        InitializeComponent();


        userListViewUI.ItemsSource = Users;
    }


    /* Cambio no fusionado mediante combinación del proyecto 'PruebaMauiAndroid (net8.0-android)'
    Antes:
        private void Button_Clicked(object sender, EventArgs e)
        {
    Después:
        private async Task Button_ClickedAsync(object sender, EventArgs e)
        {
    */
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
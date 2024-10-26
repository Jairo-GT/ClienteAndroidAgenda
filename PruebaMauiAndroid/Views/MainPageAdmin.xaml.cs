using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class MainPageAdmin : ContentPage
{
    public List<string> Users { get; set; } = new List<string>() { "User1", "User2", "User3" };


    public MainPageAdmin()
    {
        InitializeComponent();
        userListViewUI.ItemsSource = Users;
    }


    private async void Button_ClickedAsync(object sender, EventArgs e)
    {


        var succes = await ServerConnection.UserLogout();

        if (succes==ResponseStatus.ACTION_SUCCESS)
        {
            await Navigation.PopModalAsync();
          

        }
    }
}
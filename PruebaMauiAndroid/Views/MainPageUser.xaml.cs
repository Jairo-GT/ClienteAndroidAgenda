using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class MainPageUser : ContentPage
{
    private IServerConnection connection;//Hasta que se añada viewModel
    public MainPageUser(IServerConnection connection)
    {
        InitializeComponent();
        this.connection = connection;
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {


        var succes = await connection.UserLogout();

        if (succes == ResponseStatus.ACTION_SUCCESS)
        {
            await Navigation.PopModalAsync();
         
        }
    }

    private async void PerfilButton_Clicked(object sender, EventArgs e)
    {

        await Navigation.PushAsync(new UserProfilePage(connection));

    }
}
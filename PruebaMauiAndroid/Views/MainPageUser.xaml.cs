using ClienteAndroidAgenda.ViewModels;
using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class MainPageUser : ContentPage
{

    public readonly MainPageUserViewModel viewModel;

    public MainPageUser(IServerConnection connection)
    {
        InitializeComponent();

        viewModel = new(connection);
        BindingContext = viewModel;
 
    }

    private async void Button_ClickedAsync(object sender, EventArgs e)
    {

       viewModel.Logout(Navigation);
    }

    private async void PerfilButton_Clicked(object sender, EventArgs e)
    {

        viewModel.GoToProfilePage(Navigation);

    }
}
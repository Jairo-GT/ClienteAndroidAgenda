using ClienteAndroidAgenda.ViewModels;
using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class UserProfilePage : ContentPage
{

    public readonly UserProfilePageViewModel viewModel;
    public UserProfilePage(IServerConnection connection)
	{
		InitializeComponent();
		viewModel = new UserProfilePageViewModel(connection);
		BindingContext = viewModel;

	}



    private async void GuardarDatosPersonalButton_Clicked_Async(object sender, EventArgs e)
    {

        await viewModel.ChangeFullName();
    }
}
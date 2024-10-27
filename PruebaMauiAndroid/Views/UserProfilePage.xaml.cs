using ClienteAndroidAgenda.ViewModels;
using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class UserProfilePage : ContentPage
{

    public readonly UserProfilePageViewModel viewModel;
    public UserProfilePage()
	{
		InitializeComponent();
		viewModel = new UserProfilePageViewModel();
		BindingContext = viewModel;

	}



    private async void GuardarDatosPersonalButton_Clicked_Async(object sender, EventArgs e)
    {

        await viewModel.ChangeFullName();
    }
}
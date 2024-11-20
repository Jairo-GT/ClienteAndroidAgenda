using ClienteAndroidAgenda.ViewModels;
using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class UserRegistrationPage : ContentPage
{


    public readonly UserRegistrationPageViewModel viewModel;
    public UserRegistrationPage(IServerConnection connection)
	{
		InitializeComponent();

        viewModel = new UserRegistrationPageViewModel(connection);
        BindingContext = viewModel;

    }

    private void Button_Clicked(object sender, EventArgs e)
    {
        viewModel.CreateUser();
    }
}
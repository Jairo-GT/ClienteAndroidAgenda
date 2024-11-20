using ClienteAndroidAgenda.ViewModels;
using LibraryClienteAgenda;

namespace ClienteAndroidAgenda.Views;

public partial class MainPageAdmin : ContentPage
{
    


    public readonly MainPageAdminViewModel viewModel;
  

    public MainPageAdmin(IServerConnection connection)
    {
        InitializeComponent();
        viewModel = new(connection);
        BindingContext = viewModel;

        
    }


    private async void Button_ClickedAsync(object sender, EventArgs e)
    {


        viewModel.Logout(Navigation);
    }

    private async void NewUserButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new UserRegistrationPage(viewModel.connection));
    }
}
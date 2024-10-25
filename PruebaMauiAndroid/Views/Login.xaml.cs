namespace PruebaMauiAndroid.Views;
using LibraryClienteAgenda;
using PruebaMauiAndroid.ViewModels;

public partial class Login : ContentView
{

    readonly LoginViewModel viewModel;
    public Login()
    {
        InitializeComponent();
        viewModel = new LoginViewModel();
    }


    //TODO: extraer la logica de datos al ViewModel
    private async void LoginButton_Clicked(object sender, EventArgs e)
    {

        if (String.IsNullOrWhiteSpace(usernameEntry.Text) || String.IsNullOrWhiteSpace(passwordEntry.Text)) return;

        string user = viewModel.sanetizeAndValidateUsername(usernameEntry.Text.Trim());
        string password = viewModel.sanetizeAndValidatePassword(passwordEntry.Text.Trim());

        var success = await ServerConnection.UserLogin(user, password);

        usernameEntry.Text = "";
        passwordEntry.Text = "";

        if (success)
        {
            if (ServerConnection.ConnectedUser != null && ServerConnection.ConnectedUser.IsAdmin == true)
            {

                await Navigation.PushModalAsync(new MainPageAdmin());
            }


            else { await Navigation.PushModalAsync(new MainPageUser()); }

        }

    }
}

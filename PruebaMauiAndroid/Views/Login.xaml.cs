namespace PruebaMauiAndroid.Views;
using System.Threading.Tasks;
using System.Net.Http;
using PruebaMauiAndroid.Models;
using PruebaMauiAndroid.ViewModels;

public partial class Login : ContentView
{

    //IP para conectar al server local en emulador android (no android studio)
    private ServerConnection serverConnection = new ServerConnection("10.0.2.2", 12522);

    LoginViewModel viewModel;
    public Login()
    {
        InitializeComponent();
        viewModel = new LoginViewModel();
    }


    //TODO: extraer la logica de datos al ViewModel
    private async void LoginButton_Clicked(object sender, EventArgs e)
    {
        if (usernameEntry.Text == null || passwordEntry.Text == null)
        {
            Console.WriteLine("Invalid data");
            return;
        }

        string user = viewModel.sanetizeAndValidateUsername(usernameEntry.Text.Trim());
        string password = viewModel.sanetizeAndValidatePassword(passwordEntry.Text.Trim());

        var success = await ServerConnection.UserLogin(user, password);

        usernameEntry.Text = "";
        passwordEntry.Text = "";

        if (success)
        {
            if (ServerConnection.ConnectedUser.isAdmin == true)
            {

                await Navigation.PushModalAsync(new MainPageAdmin());
            }


            else { await Navigation.PushModalAsync(new MainPageUser()); }

        }

    }
}

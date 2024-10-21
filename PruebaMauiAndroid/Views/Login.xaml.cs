namespace PruebaMauiAndroid.Views;
using System.Threading.Tasks;
using System.Net.Http;
using PruebaMauiAndroid.Models;

public partial class Login : ContentView
{


    private ServerConnection serverConnection  = new ServerConnection("10.0.2.2", 12522);

	public Login()
	{
		InitializeComponent();
	}



    private string sanetizeAndValidateUsername(string username)
    {




        return username;

    }

    private string sanetizeAndValidatePassword(string password)
    {
        if (password.Length > 24)
        {
            password = password.Substring(0, 24); 
        }

        return password;

    }

    //TODO: extraer la logica de datos al ViewModel
    private async void LoginButton_Clicked(object sender, EventArgs e)
    {

        var userCredentials = new Tuple<string, string>("user", "user");
        var adminCredentials = new Tuple<string, string>("admin", "admin");
       


        if (usernameEntry.Text == null || passwordEntry.Text == null)
        {
            Console.WriteLine("Invalid data");

            //TODO:
            return;
        }

        string user = sanetizeAndValidateUsername(usernameEntry.Text.Trim());
        string password = sanetizeAndValidatePassword(passwordEntry.Text.Trim());


        string loginData = "101" + string.Format("{0:D2}", user.Length) + user + string.Format("{0:D2}", password.Length) + password;


        await App.Current.MainPage.DisplayAlert("loginData", loginData, "OK");

        var response = await serverConnection.SendDataAsync(loginData);

      

        serverConnection.ExtractAndSetToken(response);


        await App.Current.MainPage.DisplayAlert("Server Response", response, "OK");



        /*

        Tuple<string, string> inputCredentials = new Tuple<string, string>( user, password );



        if (userCredentials.Equals(inputCredentials))
        {

            //Para poder ver el menu en un modal sin flecha a atrás
            await Navigation.PushModalAsync(new MainPageUser());


        }
        else if (adminCredentials.Equals(inputCredentials))
        {

            await Navigation.PushModalAsync( new MainPageAdmin());

        }
            return;
    */
      
    
    }
}

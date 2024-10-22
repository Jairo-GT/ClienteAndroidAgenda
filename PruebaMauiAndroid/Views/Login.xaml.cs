namespace PruebaMauiAndroid.Views;
using System.Threading.Tasks;
using System.Net.Http;
using PruebaMauiAndroid.Models;

public partial class Login : ContentView
{

    //IP para conectar al server local en emulador android (no android studio)
    private ServerConnection serverConnection  = new ServerConnection("10.0.2.2", 12522);

	public Login()
	{
		InitializeComponent();
	}


    //Falta que  quedemos en algunas pautas de tamaño,caracters permitidos..
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


        if (usernameEntry.Text == null || passwordEntry.Text == null)
        {
            Console.WriteLine("Invalid data");

            //TODO:
            return;
        }

        string user = sanetizeAndValidateUsername(usernameEntry.Text.Trim());
        string password = sanetizeAndValidatePassword(passwordEntry.Text.Trim());


        string loginData = "101" + string.Format("{0:D2}", user.Length) + user + string.Format("{0:D2}", password.Length) + password;


        var response = await ServerConnection.SendDataAsync(loginData);

        serverConnection.ExtractAndSetToken(response);


        ServerConnection.user= new(user);



        string dataToSend = "205" + string.Format("{0:D2}", ServerConnection.token.Length) + ServerConnection.token +
                  string.Format("{0:D2}", ServerConnection.user.userName.Length) + ServerConnection.user.userName +
                  string.Format("{0:D2}", ServerConnection.user.userName.Length) + ServerConnection.user.userName;




        string response2 = await ServerConnection.SendDataAsync(dataToSend);

   


        usernameEntry.Text = "";
        passwordEntry.Text = "";

       if (response2.TrimEnd().Last().ToString() == "1") { 
            
            await Navigation.PushModalAsync(new MainPageAdmin()); } 
        
        
        else { await Navigation.PushModalAsync(new MainPageUser()); }



    }
}

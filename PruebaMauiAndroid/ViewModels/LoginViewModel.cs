using LibraryClienteAgenda;
using ClienteAndroidAgenda.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ClienteAndroidAgenda.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private string? _username;
        public string? UserName
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged();
                }
            }
        }
        private string? _password;
        public string? Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password= value;
                    OnPropertyChanged();
                }
            }
        }
   

        //Falta que  quedemos en algunas pautas de tamaño,caracters permitidos..
        public string sanetizeAndValidateUsername(string username)
        {




            return username;

        }

        public string sanetizeAndValidatePassword(string password)
        {
            if (password.Length > 24)
            {
                password = password.Substring(0, 24);
            }

            return password;

        }

        public ICommand LoginCommand { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }
    
        public LoginViewModel(INavigation nav)
        {
            
            LoginCommand = new Command( async () => LoginAsync(nav));
          
        }

        private async Task LoginAsync(INavigation nav)
        {

          

            if (String.IsNullOrWhiteSpace(UserName) || String.IsNullOrWhiteSpace(Password)) return;

            string user = sanetizeAndValidateUsername(UserName);
            string password = sanetizeAndValidatePassword(Password);

            var success = await ServerConnection.UserLogin(user, password);
            Password = "";


          
          

            if (success)
            {
                UserName = "";
                if (ServerConnection.ConnectedUser != null && ServerConnection.ConnectedUser.IsAdmin == true)
                {

                    await nav.PushModalAsync(new MainPageAdmin());
                }


                else { await nav.PushModalAsync(new MainPageUser()); }

            }

            await Task.Delay(2000);
        }
    }
}

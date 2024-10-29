using LibraryClienteAgenda;
using ClienteAndroidAgenda.Views;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace ClienteAndroidAgenda.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private IServerConnection connection;
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

        public Visibility LoginVisibility
        {
            get
            {
                if (IsLoginVisible)
                    return Visibility.Collapsed;
                else
                    return Visibility.Visible;
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
        public bool IsLoginVisible => !IsLoading;
        private bool isLoading;
        public bool IsLoading
        {
            get => isLoading;
            set
            {
                isLoading = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsLoginVisible));
                OnPropertyChanged(nameof(LoginVisibility));
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
    
        public LoginViewModel(INavigation nav, IServerConnection conn)
        {
            connection = conn;
            LoginCommand = new Command( async () => LoginAsync(nav));
          
        }

        private async Task LoginAsync(INavigation nav)
        {
            IsLoading = true;
          

            if (String.IsNullOrWhiteSpace(UserName) || String.IsNullOrWhiteSpace(Password)) return;

            string user = sanetizeAndValidateUsername(UserName);
            string password = sanetizeAndValidatePassword(Password);

            var success = await connection.UserLogin(user, password);
            Password = "";

            //Simulamos espera
            await Task.Delay(4000);


            if (success == ResponseStatus.ACTION_SUCCESS)
            {
                UserName = "";
                if (connection.ConnectedUser != null && connection.ConnectedUser.IsAdmin == true)
                {

                    var adminPage = new NavigationPage(new MainPageAdmin(connection));
                    NavigationPage.SetHasBackButton(adminPage.CurrentPage, false);
                    await nav.PushModalAsync(adminPage);
                }


                else {
                    var userPage = new NavigationPage(new MainPageUser(connection));
                    NavigationPage.SetHasBackButton(userPage.CurrentPage, false);
                    await nav.PushModalAsync(userPage);
                }

            }

            IsLoading = false;
        }
    }
}

using LibraryClienteAgenda;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAndroidAgenda.ViewModels
{
    public class UserRegistrationPageViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler? PropertyChanged;
        private IServerConnection connection;



        private string? _password;
        public string? Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value;
                    OnPropertyChanged();
                }
            }
        }


        private string? _role;
        public string? Role
        {
            get => _role;
            set
            {
                if (_role != value)
                {
                    _role = value;
                    OnPropertyChanged();
                }
            }
        }

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

        private string? _fullname;
        public string? FullName
        {
            get => _fullname;
            set
            {
                if (_fullname != value)
                {
                    _fullname = value;
                    OnPropertyChanged();
                }
            }
        }

        private string? _dateBorn;
        public string? DateBorn
        {
            get => _dateBorn;
            set
            {
                if (_dateBorn != value) //&& ValidateDateBornFormat(value))
                {

                    _dateBorn = value;
                    OnPropertyChanged();
                }
            }
        }


        private string? _details;
        public string? Details
        {
            get => _details;
            set
            {
                if (_details != value)
                {
                    _details = value;
                    OnPropertyChanged();
                }
            }
        }


        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


        public UserRegistrationPageViewModel(IServerConnection connection)
        {
            this.connection = connection;


        }


        public async void CreateUser() {


            //VALIDAMOS DATOS

           // var response  = await connection.AddNewUser(UserName, Password, FullName, DateBorn, Details, Role);
            var response = await connection.AddNewUser("prueba", "prueba", "prueba fullname", DateBorn, "DetailsDetailsDD", "Predeterminado");


            if (response == ResponseStatus.ACTION_SUCCESS)
            {

                //usuario añadido

            }
        
        }
        private bool ValidateDateBornFormat(string date) { return true; }

    }
}

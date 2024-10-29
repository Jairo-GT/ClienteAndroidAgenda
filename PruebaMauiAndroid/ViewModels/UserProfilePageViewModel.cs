using LibraryClienteAgenda;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ClienteAndroidAgenda.ViewModels
{
    public class UserProfilePageViewModel : INotifyPropertyChanged
    {
        private IServerConnection connection;
        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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
                if (_dateBorn != value && ValidateDateBornFormat(value))
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

        public UserProfilePageViewModel(IServerConnection conn)
        {
            connection = conn;
            if(connection.ConnectedUser != null)
            {
                Details = connection.ConnectedUser.DatosExtra;
                FullName = connection.ConnectedUser.FullName;
                UserName = connection.ConnectedUser.UserName;
                DateBorn = connection.ConnectedUser.DataNaixement;

            }
            
        }


        public async Task ChangeFullName()
        {
           



            if( connection.ConnectedUser != null) {


                var response = FullName != null ? await connection.ChangeUserFullName(FullName) : ResponseStatus.ACTION_FAILED;

                if (response == ResponseStatus.ACTION_SUCCESS)                    
                    connection.ConnectedUser.FullName = FullName;
            

                response = DateBorn != null ? await connection.ChangeUserDateBorn(DateBorn): ResponseStatus.ACTION_FAILED;
                if (response == ResponseStatus.ACTION_SUCCESS)
                    connection.ConnectedUser.DataNaixement = DateBorn;



                response = FullName != null ? await connection.ChangeUserDetails(FullName) : ResponseStatus.ACTION_FAILED;
                if (response == ResponseStatus.ACTION_SUCCESS)
                   connection.ConnectedUser.DatosExtra= Details;

            }
        }

        private bool ValidateDateBornFormat(string date) { return true; }



    }
}

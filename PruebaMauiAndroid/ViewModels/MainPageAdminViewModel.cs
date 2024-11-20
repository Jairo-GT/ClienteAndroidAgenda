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
    public class MainPageAdminViewModel : INotifyPropertyChanged
    {

        public IServerConnection connection;
        public event PropertyChangedEventHandler? PropertyChanged;



        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        }


        private List<string> _existingPermissions;

        public List<string> ExistingPermissions
        {
            get => _existingPermissions;
            set
            {
                if (_existingPermissions != value)
                {
                    _existingPermissions = value;
                    OnPropertyChanged();
                }
            }
        }


        private List<string> _users;

        public List<string> Users
        {
            get => _users;
            set
            {
                if (_users != value)
                {
                    _users = value;
                    OnPropertyChanged();
                }
            }
        }


        public async void Logout(INavigation n)
        {

            var succes = await connection.UserLogout();

            if (succes == ResponseStatus.ACTION_SUCCESS)
            {
                await n.PopModalAsync();


            }

        }
        public MainPageAdminViewModel(IServerConnection connection)
        {
            this.connection = connection;

            Users = new() { "User1", "User2" };
        }
    

        public async void DeleteUser(UserInfo user)
        {



            var response = await connection.DeleteUser(user.UserName);

            if(response == ResponseStatus.ACTION_SUCCESS)
            {

                //handle
            };

        }

        private async void GetAvailablePermissions()
        {

            ExistingPermissions.Clear();

            var tuple = await connection.ShowPermission();

            if((ResponseStatus)tuple.Item1 == ResponseStatus.ACTION_SUCCESS)
            {
                var permissions = (List<string>)tuple.Item2;

                foreach (var permission in permissions)
                {
                    ExistingPermissions.Add(permission);


                }


            }

        }

        public async void GetAllUsers()
        {
            var tuple = await connection.GetAllUsers();
            if((ResponseStatus)tuple.Item1 == ResponseStatus.ACTION_SUCCESS)
            {
                Users.Clear();
                var users = (List<string>)tuple.Item2;
                foreach (var user in users)
                {

                    Users.Add(user);
                }
            }
        }




    }
}

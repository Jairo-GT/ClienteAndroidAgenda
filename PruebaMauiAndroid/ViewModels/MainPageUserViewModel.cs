using ClienteAndroidAgenda.Models;
using ClienteAndroidAgenda.Views;
using LibraryClienteAgenda;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;


namespace ClienteAndroidAgenda.ViewModels
{
    
    public class MainPageUserViewModel :INotifyPropertyChanged
    {

        private IServerConnection connection;
    public event PropertyChangedEventHandler? PropertyChanged;



        public ObservableCollection<AgendaEvent> Events { get; set; }


        private System.Timers.Timer _temporizador;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }


        public MainPageUserViewModel(IServerConnection connection)
        {

            this.connection = connection;

            //DEMO DATA
            Events = new ObservableCollection<AgendaEvent>
        {
            new AgendaEvent{ Titulo = "Reunión", HoraObjetivo = DateTime.Now.AddMinutes(120) },
            new AgendaEvent { Titulo = "Cena", HoraObjetivo = DateTime.Now.AddHours(5) },
            new AgendaEvent{ Titulo = "Entrega", HoraObjetivo = DateTime.Now.AddDays(2) }


        };


            _temporizador = new System.Timers.Timer(10000); //Actualizamos cada 10 segundos
            _temporizador.Elapsed += ActualizarEventos;
            _temporizador.Start();

        }
        private void ActualizarEventos(object sender, ElapsedEventArgs e)
        {
            OnPropertyChanged(nameof(Events));
        }


        public async void Logout(INavigation n) {

            var succes = await connection.UserLogout();

            if (succes == ResponseStatus.ACTION_SUCCESS)
            {
                await n.PopModalAsync();

            }


        }

        public  async void GoToProfilePage(INavigation n) {

            await n.PushAsync(new UserProfilePage(connection));


        }

    }
}

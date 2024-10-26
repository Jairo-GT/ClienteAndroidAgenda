namespace ClienteAndroidAgenda.Views;
using LibraryClienteAgenda;
using ClienteAndroidAgenda.ViewModels;
using System.Diagnostics;

public partial class Login : ContentView
{


    public readonly LoginViewModel viewModel;
    public Login()
    {
        InitializeComponent();
        
        viewModel = new(Navigation);
        BindingContext = viewModel;
        ServerConnection.SetupServerVars("10.0.2.2", 12522);
       
    }


}

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
        ServerMessageFactory mFactory = new();
        ServerMessageHandler mHandler = new();
        ServerConnection connection = new(mFactory,mHandler);
        connection.SetupServerVars("10.0.2.2", 12522);
        viewModel = new(Navigation,connection);
        BindingContext = viewModel;
        
       
    }


}

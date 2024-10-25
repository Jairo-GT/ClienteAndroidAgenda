namespace PruebaMauiAndroid.Views;
using LibraryClienteAgenda;
using PruebaMauiAndroid.ViewModels;
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
        Debug.WriteLine(BindingContext?.GetType().Name ?? "BindingContext is null");
    }


}

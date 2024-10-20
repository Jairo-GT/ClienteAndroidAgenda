namespace PruebaMauiAndroid.Views;

public partial class MainPageAdmin : ContentPage
{
    public List<string> Users { get; set; } = new List<string>() { "User1", "User2", "User3" };


    public MainPageAdmin()
    {
        InitializeComponent();


        userListViewUI.ItemsSource = Users;
    }
}
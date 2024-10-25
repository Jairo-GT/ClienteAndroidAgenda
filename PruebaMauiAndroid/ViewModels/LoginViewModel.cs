namespace PruebaMauiAndroid.ViewModels
{
    public class LoginViewModel
    {

        public string? UserName { get; set; }
        public string? Password { get; set; }


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

    }
}

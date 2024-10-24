using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaMauiAndroid.ViewModels
{
    public class LoginViewModel
    {

        string userName { get; set; }
        string password { get; set; }


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

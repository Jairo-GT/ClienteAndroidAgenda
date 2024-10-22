using Org.Apache.Http.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PruebaMauiAndroid.Models
{
    
    public class UserInfo

    {

        public bool isAdmin { get; set; }

        public string userName { get; set; }
   
    public UserInfo(string name)
        {
            userName = name;
        }
    }
}

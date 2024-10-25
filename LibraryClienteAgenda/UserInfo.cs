using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClienteAgenda
{
    public class UserInfo(string name)


    {

        public bool IsAdmin { get; set; }

        public string UserName { get; set; } = name;
    }
}

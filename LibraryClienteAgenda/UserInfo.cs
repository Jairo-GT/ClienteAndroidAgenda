using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryClienteAgenda
{
    public class UserInfo(string name)


    {

        public bool IsAdmin { get; set; }

        public string UserName { get; set; } = name;

        public string Userrol { get; set; }
        public string FullName { get; set; }
        public string DataNaixement { get; set; }
        public string UserToken { get; set; }
        public string DatosExtra { get; set; }


    }
}

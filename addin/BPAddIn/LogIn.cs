using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class LogIn
    {
        public String name { get; set; }

        public String password { get; set; }
        
        public LogIn(String name, String password)
        {
            this.name = name;
            this.password = password;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }     
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class ModelInformation
    {
        public string token { get; set; }

        public string modelGUID { get; set; }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        } 
    }
}

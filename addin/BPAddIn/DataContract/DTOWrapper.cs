using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.DataContract
{
    class DTOWrapper
    {
        public ModelChange modelChange { get; set; }
        public string userGUID { get; set; }
        public DTOWrapper()
        {
            this.userGUID = "0000-0000-0001";
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}

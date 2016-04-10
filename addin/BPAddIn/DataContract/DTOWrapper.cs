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
        public string userToken { get; set; }
        public DTOWrapper()
        {
            this.userToken = ChangeService.userToken;
        }

        public string serialize()
        {
            var settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects
            };
            return JsonConvert.SerializeObject(this, settings);
        }

    }
}

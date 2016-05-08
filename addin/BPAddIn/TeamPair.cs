using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class TeamPairDTO
    {
        public string token { get; set; }

        public string teamMemberName { get; set; }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }


}

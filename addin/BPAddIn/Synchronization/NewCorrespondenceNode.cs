using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class NewCorrespondenceNode
    {
        public string firstUsername { get; set; }

        public string firstItemGUID { get; set; }

        public string secondUsername { get; set; }

        public string secondItemGUID { get; set; }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }  
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPAddIn
{
    [Table("user")]
    public class User
    {
        [Key]
        public int id { get; set; }

        public string username { get; set; }

        public string token { get; set; }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }  

    }
}

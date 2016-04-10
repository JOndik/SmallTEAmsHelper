using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.DataContract
{
    [Table("defect_reports")]
    public class DefectReport
    {
        [Key]
        public long id { get; set; }
        public string timestamp { get; set; }
        [NotMapped]
        public string userName { get; set; }
        [NotMapped]
        public string userToken { get; set; }
        public string modelGUID { get; set; }
        public string ruleName { get; set; }
        public string ruleGUID { get; set; }
        public int actionsBeforeCorrection { get; set; }
        public int isHidden { get; set; }

        public DefectReport()
        {
            this.timestamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000).ToString();
            this.actionsBeforeCorrection = 0;
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

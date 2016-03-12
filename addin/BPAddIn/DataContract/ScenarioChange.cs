using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.DataContract
{
    [Table("scenario_changes")]
    public class ScenarioChange : ModelChange
    {
        public string name { get; set; }
        public string type { get; set; }
        public int status { get; set; }
        public string scenarioGUID { get; set; }

        public ScenarioChange() : base() {
            base.classType = "ScenarioChange";
        }
    }
}

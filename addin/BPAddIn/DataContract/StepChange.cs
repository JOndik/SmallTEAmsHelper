using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.DataContract
{
    [Table("step_changes")]
    public class StepChange : ModelChange
    {
        public int status { get; set; }
        public string scenarioGUID { get; set; }
        public string stepGUID { get; set; }
        public int position { get; set; }
        public string stepType { get; set; }
        public string name { get; set; }
        public string uses { get; set; }
        public string results { get; set; }
        public string state { get; set; }
        public string extensionGUID { get; set; }
        public string joiningStepGUID { get; set; }
        public string joiningStepPosition { get; set; }

        public StepChange() : base() {
            base.classType = "StepChange";
        }
    }
}

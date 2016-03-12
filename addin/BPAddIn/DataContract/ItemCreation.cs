using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPAddIn.DataContract
{
    [Table("item_creations")]
    public class ItemCreation : ModelChange
    {
        public string parentGUID { get; set; }
        public string author { get; set; }
        public string name { get; set; }
        public string packageGUID { get; set; }
        public string diagramGUID { get; set; }
        public string srcGUID { get; set; }
        public string targetGUID { get; set; }
        public string coordinates { get; set; }

        public ItemCreation() : base() {
            base.classType = "ItemCreation";
        }
    }
}

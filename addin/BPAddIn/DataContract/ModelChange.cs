using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mapping = System.Data.Linq.Mapping;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.DataContract
{
    [Table("model_changes")]
    [Mapping.InheritanceMapping(Code = "P", Type = typeof(PropertyChange))]
    public class ModelChange
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long id { get; set; }
        //public string classType { get; set; }
        public string timestamp { get; set; }
        public string userGUID { get; set; }

        public ModelChange()
        {
            this.timestamp = DateTime.Now.ToString("yyyyMMddHHmmssffff");
            this.userGUID = "0000-0000-0001";
        }
    }
}

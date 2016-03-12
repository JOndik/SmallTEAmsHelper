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
    [Mapping.InheritanceMapping(Code = "I", Type = typeof(ItemCreation))]
    public class ModelChange
    {
        [Key]
        public long id { get; set; }
        public string timestamp { get; set; }
        public string itemGUID { get; set; }
        public string modelGUID { get; set; }
        public int elementType { get; set; }
        [NotMapped]
        public string classType { get; set; }

        public ModelChange()
        {
            this.timestamp = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds * 1000).ToString();
        }
    }
}

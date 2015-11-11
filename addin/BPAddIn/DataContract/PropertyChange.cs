﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace BPAddIn.DataContract
{
    [Table("property_changes")]
    public class PropertyChange : ModelChange
    {
        public int propertyType { get; set; }
        public string propertyBody { get; set; } 
           
        public PropertyChange() : base() {
            base.classType = "PropertyChange";
        }
    }
}

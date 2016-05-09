using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    [Table(name:"dictionary")]
    public class Word
    {
        [Column(name: "base")]
        public string wordBase { get; set; }

        [Key]
        [Column(name: "word")]
        public string word { get; set; }

        [Column(name: "flags")]
        public string flags { get; set; }

        public bool hasFlag(char flag)
        {
            if (flag == 'N')
            {
                return flags[0] == 'S';
            }

            return flags[0] == flag;
        }
    }
}

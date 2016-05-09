using BPAddInTry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn.Rules
{
    public class RuleEntry
    {
        public int actions { get; set; }
        public string GUID { get; set; }
        public string name { get; set; }
        public string elGUID { get; set; }
        public Rule rule { get; set; }
        public Model model { get; set; }
        public object element { get; set; }
        public ListBoxObject listBoxObject { get; set; }

        internal ListBoxObject createListBoxObject(string defectDescription)
        {
            listBoxObject = new ListBoxObject(defectDescription, defectDescription, defectDescription, this);
            return listBoxObject;
        }
    }
}

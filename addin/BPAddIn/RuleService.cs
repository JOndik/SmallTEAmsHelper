using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddInTry
{
    class RuleService
    {
        public Dictionary<string, Rule> ruleStorage = new Dictionary<string, Rule>();
        //public static Rule rule = null;

        public void broadcastEvent(Wrapper.Model model, EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            /*foreach (Rule rule in ruleStorage[ot])
            {
                rule.activate(model, Repository.GetConnectorByGuid(GUID));
            }*/

            if (!ruleStorage.ContainsKey(GUID))
            {
                Rule rule = new Rule();
                ruleStorage.Add(GUID, rule);
            }

            //MessageBox.Show("pridany");
            ruleStorage[GUID].activate(model, Repository.GetConnectorByGuid(GUID));
        }

        public void broadcastEvent(Wrapper.Model model, EA.Repository Repository, long ID)
        {
            /*foreach (Rule rule in ruleStorage[ot])
            {
                rule.activate(model, Repository.GetConnectorByGuid(GUID));
            }*/

            /*Rule rule = null;

            if (rule == null)
            {
                rule = new Rule();
            }
            rule.activate(model, Repository.GetConnectorByID(Convert.ToInt32(ID)));*/
        }
    }
}

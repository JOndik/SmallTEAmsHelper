using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    class RuleParser
    {
        public Dictionary<string, List<Rule>> parseRules()
        {
            Dictionary<string, List<Rule>> eventWatchers = new Dictionary<string, List<Rule>>();
            string addInPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/sthAddin/rules";
            string[] files = Directory.GetFiles(addInPath, "*.rule");

            foreach (string file in files)
            {
                try
                {
                    string content = File.ReadAllText(file);
                    JObject obj = (JObject)JsonConvert.DeserializeObject(content);

                    Rule rule = new Rule();
                    rule.name = obj["name"].ToString();
                    rule.elementType = obj["element"]["type"].ToString();
                    rule.elementStereotype = obj["element"]["stereotype"].ToString();

                    rule.attributeType = obj["attribute"]["type"].ToString();
                    rule.attributeStereotype = obj["element"]["stereotype"].ToString();

                    rule.contentDefectMsg = obj["content"]["defectMsg"].ToString();
                    rule.contentValid = obj["content"]["valid"].ToString();
                    rule.contentCorrect = obj["content"]["correct"].ToString();

                    if (obj["content"]["cond"] != null)
                    {
                        rule.contentCond = obj["content"]["cond"].ToString();
                    }

                    if (!eventWatchers.ContainsKey(rule.elementType))
                    {
                        eventWatchers.Add(rule.elementType, new List<Rule>());
                    }

                    eventWatchers[rule.elementType].Add(rule);
                }
                catch (Exception ex) { }
            }

            return eventWatchers;
        }
    }
}

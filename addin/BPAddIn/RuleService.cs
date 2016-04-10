using BPAddIn;
using BPAddIn.DataContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddInTry
{
    public class RuleService
    {
        public static Dictionary<string, List<Rule>> ruleStorage = new Dictionary<string, List<Rule>>();
        private DefectReportService defectReportService;
        private Thread defectReportDispatcherThread;

        public RuleService()
        {
            this.defectReportService = new DefectReportService();
            this.defectReportDispatcherThread = new Thread(new ThreadStart(this.defectReportService.startActivityDispatcher));
            this.defectReportDispatcherThread.Start();
        }

        public void broadcastEvent(Wrapper.Model model, EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            showErrorWindow(Repository);

            if (!ruleStorage.ContainsKey(GUID))
            {
                List<Rule> ruleList = new List<Rule>();
                Rule rule = new Rule();
                RuleUC ruleUC = new RuleUC();

                if (ot == EA.ObjectType.otElement)
                {
                    ruleList.Add(new RuleClass());
                }
                else
                {
                    ruleList.Add(rule);
                    ruleList.Add(ruleUC);
                    ruleList.Add(new RuleClassRelation());
                    ruleList.Add(new RuleAssociationCardinality());
                }

                ruleStorage.Add(GUID, ruleList);
            }

            foreach (Rule rule in ruleStorage[GUID])
            {
                try {
                    rule.activate(model, GUID);
                    //this.ping(rule, Repository);
                }
                catch (Exception ex) { }
            }

            foreach (KeyValuePair<string, List<Rule>> ruleList in ruleStorage)
            {
                foreach (Rule rule in ruleList.Value)
                {
                    try
                    {
                        rule.ping();
                        this.ping(rule, Repository);
                    }
                    catch (Exception ex) { }
                }
            }
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
        public static void addRule(string GUID, Rule rule)
        {
            MessageBox.Show(GUID);
            if (!ruleStorage.ContainsKey(GUID))
            {
                List<Rule> ruleList = new List<Rule>();
                ruleList.Add(rule);

                ruleStorage.Add(GUID, ruleList);
            }
            else
            {
                ruleStorage[GUID].Add(rule);
            }
        }

        public void ping(Rule rule, EA.Repository repository)
        {
            if (rule.actions < 0)
            {
                return;
            }

            DefectReport defectReport = new DefectReport();
            defectReport.modelGUID = repository.GetPackageByID(1).PackageGUID;
            defectReport.ruleGUID = rule.ruleGUID;
            defectReport.ruleName = rule.name;
            defectReport.actionsBeforeCorrection = rule.actions;
            defectReport.isHidden = rule.isHidden;

            defectReportService.saveReport(defectReport);
        }

        public virtual void showErrorWindow(EA.Repository repository)
        {
            if (BPAddIn.BPAddIn.defectsWindow == null)
            {
                BPAddIn.BPAddIn.defectsWindow = repository.AddWindow("Detekované chyby", "BPAddIn.DefectsWindow") as DefectsWindow;
            }

            repository.ShowAddinWindow("Detekované chyby");
        }
    }
}

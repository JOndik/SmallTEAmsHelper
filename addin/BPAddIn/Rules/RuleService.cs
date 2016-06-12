using BPAddIn;
using BPAddIn.DataContract;
using BPAddIn.Rules;
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
        Dictionary<string, List<Rule>> eventWatchers;
        public static Dictionary<string, List<RuleEntry>> active = new Dictionary<string, List<RuleEntry>>();

        private DefectReportService defectReportService;
        private Thread defectReportDispatcherThread;

        public RuleService()
        {
            RuleParser parser = new RuleParser();
            eventWatchers = parser.parseRules();
            this.defectReportService = new DefectReportService();
            this.defectReportDispatcherThread = new Thread(new ThreadStart(this.defectReportService.startActivityDispatcher));
            this.defectReportDispatcherThread.Start();
        }

        public void broadcastEvent(Wrapper.Model model, EA.Repository Repository, string GUID, EA.ObjectType ot)
        {
            ping(GUID, Repository);

            if (ot == EA.ObjectType.otElement)
            {
                object element = Repository.GetElementByGuid(GUID);
                foreach (Rule registeredRule in eventWatchers[((EA.Element)element).Type])
                {
                    string rslt = registeredRule.activate(element, model);
                    if (rslt != "")
                    {
                        if (!active.ContainsKey(GUID))
                        {
                            RuleEntry ruleEntry = createRuleEntry(model, rslt, element, registeredRule);

                            if (active.ContainsKey(GUID))
                            {
                                active[GUID].Add(ruleEntry);
                            }
                            else
                            {
                                List<RuleEntry> ruleEntryList = new List<RuleEntry>();
                                ruleEntryList.Add(ruleEntry);
                                active.Add(GUID, ruleEntryList);
                            }

                            showErrorWindow(Repository);
                            addToDefectsList(rslt, ruleEntry);
                        }
                        else
                        {
                            foreach (RuleEntry ruleEntry in active[GUID])
                            {
                                if (ruleEntry.rule == registeredRule)
                                {
                                    addToDefectsList(rslt, ruleEntry);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (active.ContainsKey(GUID))
                        {
                            RuleEntry ruleEntry = active[GUID].Find(x => x.rule == registeredRule);

                            if (ruleEntry != null)
                            {
                                BPAddIn.BPAddIn.defectsWindow.removeFromList(ruleEntry.listBoxObject);
                                BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(ruleEntry.listBoxObject);
                                active.Remove(GUID);
                            }
                        }
                    }
                }
            }
            else if (ot == EA.ObjectType.otConnector)
            {
                object connector = Repository.GetConnectorByGuid(GUID);
                foreach (Rule registeredRule in eventWatchers[((EA.Connector)connector).Type])
                {
                    string rslt = registeredRule.activate(connector, model);
                    if (rslt != "")
                    {
                        if (!active.ContainsKey(GUID) || active[GUID].Find(x => x.rule == registeredRule) == null)
                        {
                            RuleEntry ruleEntry = createRuleEntry(model, rslt, connector, registeredRule);

                            if (active.ContainsKey(GUID))
                            {
                                active[GUID].Add(ruleEntry);
                            }
                            else
                            {
                                List<RuleEntry> ruleEntryList = new List<RuleEntry>();
                                ruleEntryList.Add(ruleEntry);
                                active.Add(GUID, ruleEntryList);
                            }

                            showErrorWindow(Repository);
                            addToDefectsList(rslt, ruleEntry);
                        }
                        else
                        {
                            foreach (RuleEntry ruleEntry in active[GUID])
                            {
                                if (ruleEntry.rule == registeredRule)
                                {
                                    addToDefectsList(rslt, ruleEntry);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (active.ContainsKey(GUID))
                        {
                            RuleEntry ruleEntry = active[GUID].Find(x => x.rule == registeredRule);

                            if (ruleEntry != null)
                            {
                                BPAddIn.BPAddIn.defectsWindow.removeFromList(ruleEntry.listBoxObject);
                                BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(ruleEntry.listBoxObject);
                                active.Remove(GUID);
                            }
                        }
                    }
                }
            }
            else if (ot == EA.ObjectType.otDiagram)
            {
                object diagram = Repository.GetDiagramByGuid(GUID);
                foreach (Rule registeredRule in eventWatchers[((EA.Diagram)diagram).Type])
                {
                    string rslt = registeredRule.activate(diagram, model);
                    if (rslt != "")
                    {
                        if (!active.ContainsKey(GUID) || active[GUID].Find(x => x.rule == registeredRule) == null)
                        {
                            RuleEntry ruleEntry = createRuleEntry(model, rslt, diagram, registeredRule);

                            if (active.ContainsKey(GUID))
                            {
                                active[GUID].Add(ruleEntry);
                            }
                            else
                            {
                                List<RuleEntry> ruleEntryList = new List<RuleEntry>();
                                ruleEntryList.Add(ruleEntry);
                                active.Add(GUID, ruleEntryList);
                            }

                            showErrorWindow(Repository);
                            addToDefectsList(rslt, ruleEntry);
                        }
                        else
                        {
                            foreach (RuleEntry ruleEntry in active[GUID])
                            {
                                if (ruleEntry.rule == registeredRule)
                                {
                                    addToDefectsList(rslt, ruleEntry);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (active.ContainsKey(GUID))
                        {
                            RuleEntry ruleEntry = active[GUID].Find(x => x.rule == registeredRule);

                            if (ruleEntry != null)
                            {
                                BPAddIn.BPAddIn.defectsWindow.removeFromList(ruleEntry.listBoxObject);
                                BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(ruleEntry.listBoxObject);
                                active.Remove(GUID);
                            }
                        }
                    }
                }
            }
        }

        public void ping(string GUID, EA.Repository repository)
        {
            if (!active.ContainsKey(GUID))
            {
                return;
            }

            foreach (RuleEntry ruleEntry in active[GUID]) {
                ruleEntry.actions++;
                DefectReport defectReport = new DefectReport();
                defectReport.modelGUID = repository.GetPackageByID(1).PackageGUID;
                defectReport.ruleGUID = ruleEntry.GUID;
                defectReport.ruleName = ruleEntry.name;
                defectReport.actionsBeforeCorrection = ruleEntry.actions;

                defectReportService.saveReport(defectReport);
            }
        }

        public virtual void showErrorWindow(EA.Repository repository)
        {
            if (BPAddIn.BPAddIn.defectsWindow == null)
            {
                BPAddIn.BPAddIn.defectsWindow = repository.AddWindow("Defects detection", "BPAddIn.DefectsWindow") as DefectsWindow;
            }

            repository.ShowAddinWindow("Defects detection");
        }

        private RuleEntry createRuleEntry(Wrapper.Model model, string name, object element, Rule rule)
        {
            RuleEntry ruleEntry = new RuleEntry();
            ruleEntry.name = name;
            ruleEntry.GUID = Guid.NewGuid().ToString();
            ruleEntry.actions = 0;
            ruleEntry.element = element;
            ruleEntry.rule = rule;
            ruleEntry.model = model;

            return ruleEntry;
        }

        private void addToDefectsList(string defectDescription, RuleEntry ruleEntry)
        {
            ListBoxObject listBoxObject = ruleEntry.createListBoxObject(defectDescription);
            BPAddIn.BPAddIn.defectsWindow.addToList(listBoxObject);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSF.UmlToolingFramework.Wrappers.EA;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using UML = TSF.UmlToolingFramework.UML;
using System.Windows.Forms;
using System.Threading;
using BPAddIn;

namespace BPAddInTry
{
    public class RuleUC : Rule
    {
        public RuleUC() : base()
        {
            this.name = "UseCaseDiagram::ExtendRelation::MissingExtensionPoint";
            this.actions = -1;
        }

        public override void activate(Wrapper.Model modelArg, string GUID)
        {
            objGUID = "MEXT" + GUID;
            EA.Connector connector = modelArg.getWrappedModel().GetConnectorByGuid(GUID);
            defectDescription = "Chybajuci extension point.";
            model = modelArg;
            connectorWrapper = new ConnectorWrapper(model, connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.target;
            string targetType = elementWrapper.WrappedElement.Type;            

            //MessageBox.Show(ownerType);

            if ("UseCase".Equals(targetType) && "extend".Equals(connectorWrapper.WrappedConnector.Stereotype))
            {
                int numExtendConnectors = 0;
                int numExtensionPoints = elementWrapper.WrappedElement.ExtensionPoints.Count(c => c == ',');

                foreach (EA.Connector con in elementWrapper.WrappedElement.Connectors)
                {
                    if ("extend".Equals(con.Stereotype))
                    {
                        numExtendConnectors++;
                    }
                }
                
                if (numExtendConnectors != numExtensionPoints)
                {
                    if (!isActive)
                    {
                        this.actions = 0;
                        this.ruleGUID = Guid.NewGuid().ToString();
                    }
                    this.isActive = true;
                    //this.ping();
                    showErrorWindow();
                    listBoxObject = new ListBoxObject(defectDescription, defectDescription, defectDescription, this);
                    BPAddIn.BPAddIn.defectsWindow.addToList(listBoxObject);

                    RuleUC2 ruleUC2 = new RuleUC2();
                    ruleUC2.listBoxObject = listBoxObject;
                    RuleService.addRule(elementWrapper.WrappedElement.ElementGUID, ruleUC2);
                    MessageBox.Show(elementWrapper.WrappedElement.ElementGUID);
                }
                else
                {
                    BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
                    BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
                    this.isActive = false;
                }
            }
        }

        public override void correct()
        {
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.target;
            elementWrapper.WrappedElement.ExtensionPoints += "," + connectorWrapper.source.name + " extension point";
            elementWrapper.WrappedElement.Update();
            model.adviseChange(elementWrapper);
            BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
            BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
            this.isActive = false;
        }

        public override void selectInDiagram()
        {
            connectorWrapper.select();
            connectorWrapper.selectInCurrentDiagram();
        }

        public override void activate(EA.Element element)
        {

        }

        public override void showErrorWindow()
        {
            if (BPAddIn.BPAddIn.defectsWindow == null)
            {
                BPAddIn.BPAddIn.defectsWindow = model.getWrappedModel().AddWindow("Detekované chyby", "BPAddIn.DefectsWindow") as DefectsWindow;
            }

            model.getWrappedModel().ShowAddinWindow("Detekované chyby");
        }

        public override string ToString()
        {
            return defectDescription;
        }
    }
}

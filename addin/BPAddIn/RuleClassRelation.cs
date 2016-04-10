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
    public class RuleClassRelation : Rule
    {
        public RuleClassRelation() : base()
        {
            this.name = "ClassDiagram::Association::MissingName";
            this.actions = -1;
        }

        public override void activate(Wrapper.Model modelArg, string GUID)
        {
            objGUID = "ANAME" + GUID;
            EA.Connector connector = modelArg.getWrappedModel().GetConnectorByGuid(GUID);
            defectDescription = "Chybajuci nazov asociacie medzi triedami '";
            model = modelArg;
            connectorWrapper = new ConnectorWrapper(model, connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.owner;
            EA.Element sourceElement = ((ElementWrapper)connectorWrapper.source).WrappedElement;
            EA.Element targetElement = ((ElementWrapper)connectorWrapper.target).WrappedElement;
            defectDescription += sourceElement.Name + "' a '" + targetElement.Name + "'.";

            //MessageBox.Show(ownerType);

            if ("Class".Equals(sourceElement.Type) && "Class".Equals(targetElement.Type) && "Association".Equals(connector.Type))
            {                
                if (String.IsNullOrEmpty(connectorWrapper.WrappedConnector.Name))
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
            connectorWrapper.WrappedConnector.Name = "vztah";
            connectorWrapper.WrappedConnector.Update();
            model.adviseChange(connectorWrapper);
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

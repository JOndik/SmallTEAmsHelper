//using System;
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
using System;

namespace BPAddInTry
{
    public class Rule
    {
        public string name { get; set; }
        public EA.Element element { get; set; }
        public Content content { get; set; }
        public string defectDescription;
        protected ConnectorWrapper connectorWrapper;
        protected Model model;
        public ListBoxObject listBoxObject { get; set; }
        public string objGUID { get; set; }
        public int actions { get; set; }
        public string ruleGUID { get; set; }
        public int isHidden { get; set; }
        public bool isActive { get; set; }

        public Rule()
        {
            this.name = "ActivityDiagram::Flow::MissingGuard";
            this.actions = -1;
            this.isHidden = 0;
            this.isActive = false;
        }

        public virtual void activate(Wrapper.Model modelArg, string GUID)
        {
            objGUID = "GUARD" + GUID;
            EA.Connector connector = modelArg.getWrappedModel().GetConnectorByGuid(GUID);
            defectDescription = "Chybajuca podmienka (guard) pri prechode z rozhodovacieho (decision) uzla.";
            model = modelArg;
            connectorWrapper = new ConnectorWrapper(model, connector);
            ElementWrapper elementWrapper = (ElementWrapper)connectorWrapper.owner;
            string ownerType = elementWrapper.WrappedElement.Type;
  
            //MessageBox.Show(ownerType);

            if ("Decision".Equals(ownerType))
            {
                if (String.IsNullOrEmpty(connectorWrapper.WrappedConnector.TransitionGuard))
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
                    this.isActive = false;
                    BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
                    BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
                }
            }
        }

        public virtual void correct()
        {
            connectorWrapper.WrappedConnector.TransitionGuard = "podmienka";
            connectorWrapper.WrappedConnector.Update();
            model.adviseChange(connectorWrapper);
            BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
            BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
            this.isActive = false;
        }

        public virtual void selectInDiagram()
        {
            connectorWrapper.select();
            connectorWrapper.selectInCurrentDiagram();
        }

        public virtual void activate(EA.Element element)
        {
            
        }

        public virtual void showErrorWindow()
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

        public virtual void ping()
        {
            if (isActive)
            {
                this.actions++;
            }
            //this.isHidden = BPAddIn.BPAddIn.defectsWindow.isHidden(this.ruleGUID);
            this.isHidden = 0;
        }
    }
}

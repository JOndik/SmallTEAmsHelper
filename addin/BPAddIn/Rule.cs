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
    class Rule
    {
        public string name { get; set; }
        public Element element { get; set; }
        public Content content { get; set; }
        public string defectDescription;
        private ConnectorWrapper connectorWrapper;
        private Model model;
        private ListBoxObject listBoxObject;

        public void activate(Wrapper.Model modelArg, EA.Connector connector)
        {
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
                    showErrorWindow();
                    listBoxObject = new ListBoxObject(defectDescription, defectDescription, defectDescription, this);
                    BPAddIn.BPAddIn.defectsWindow.addToList(listBoxObject);
                }
                else
                {
                    BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
                }
            }
        }

        public void correct()
        {
            connectorWrapper.WrappedConnector.TransitionGuard = "podmienka";
            connectorWrapper.WrappedConnector.Update();
            model.adviseChange(connectorWrapper);
            BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
        }

        public void selectInDiagram()
        {
            connectorWrapper.select();
            connectorWrapper.selectInCurrentDiagram();
        }

        public void activate(EA.Element element)
        {
            
        }

        public void showErrorWindow()
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

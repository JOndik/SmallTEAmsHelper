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
    public class RuleUC2 : Rule
    {
        public RuleUC2() : base()
        {
            this.name = "UseCaseDiagram::ExtendRelation::MissingExtensionPoint2";
            this.actions = -1;
        }

        public override void activate(Wrapper.Model modelArg, string GUID)
        {
            objGUID = "EXT" + GUID;
            EA.Element element = modelArg.getWrappedModel().GetElementByGuid(GUID);
            defectDescription = "Chybajuci extension point.";
            model = modelArg;

            if (element.Type == "UseCase")
            {
                int numExtendConnectors = 0;
                int numExtensionPoints = element.ExtensionPoints.Count(c => c == ',');

                if (numExtendConnectors == 0 && !String.IsNullOrEmpty(element.ExtensionPoints))
                {
                    numExtensionPoints = 1;
                }

                foreach (EA.Connector con in element.Connectors)
                {
                    if ("extend".Equals(con.Stereotype))
                    {
                        numExtendConnectors++;
                    }
                }

                if (numExtendConnectors == numExtensionPoints)
                {
                    if (!isActive)
                    {
                        this.actions = 0;
                        this.ruleGUID = Guid.NewGuid().ToString();
                    }
                    BPAddIn.BPAddIn.defectsWindow.removeFromList(listBoxObject);
                    BPAddIn.BPAddIn.defectsWindow.removeFromHiddenList(listBoxObject);
                    this.isActive = false;
                }
            }
        }

        public override void correct()
        {
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

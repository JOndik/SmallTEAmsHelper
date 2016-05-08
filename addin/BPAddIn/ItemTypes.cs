using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn
{
    public class ItemTypes
    {
        private Wrapper.Model model;

        public ItemTypes(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
        }

        public int getElementType(string GUID)
        {
            EA.Element element = (EA.Element)model.getWrappedModel().GetElementByGuid(GUID);
            string type = model.getWrappedModel().GetElementByGuid(GUID).Type;

            switch (type)
            {
                case "Class":
                    return 0;
                case "Activity":
                    if (element.Stereotype == "process")
                    {
                        return 4;
                    }
                    return 1;
                case "UseCase":
                    return 2;
                case "Package":
                    return 3;
                case "Interface":
                    return 5;
                case "State":
                    return 6;
                case "StateNode":
                    if (element.Subtype == 3)               //Initial (statechart diagram)
                    {
                        return 7;
                    }
                    if (element.Subtype == 4)               //Final (statechart diagram)
                    {
                        return 8;
                    }
                    if (element.Subtype == 100)             //Initial (activity diagram)
                    {
                        return 9;
                    }
                    if (element.Subtype == 101)             //Final (activity diagram)
                    {
                        return 10;
                    }
                    return -1;
                case "ActivityPartition":
                    return 11;
                case "Decision":
                    return 12;
                case "MergeNode":
                    return 13;
                case "Object":
                    if (element.Stereotype == "resource")
                    {
                        return 15;
                    }
                    if (element.Stereotype == "goal")
                    {
                        return 16;
                    }
                    if (element.Stereotype == "information")
                    {
                        return 17;
                    }
                    return 14;
                case "Event":
                    if (element.Subtype == 1)           //receiver
                    {
                        return 18;
                    }
                    if (element.Subtype == 0)           //sender
                    {
                        return 19;
                    }
                    return -1;
             /*   case "InteractionFragment":
                    if (element.Subtype == 0)           //alt
                    {
                        return 20;
                    }
                    if (element.Subtype == 1)           //opt
                    {
                        return 21;
                    }
                    if (element.Subtype == 2)           //break
                    {
                        return 22;
                    }
                    if (element.Subtype == 4)           //loop
                    {
                        return 23;
                    }
                    return -1;
                case "Sequence":                        //lifeline
                    return 24;*/
                case "Component":
                    return 25;
                case "Screen":
                    return 26;
                case "Actor":
                    return 30;
                case "GUIElement":
                    if (element.Stereotype == "list")
                    {
                        return 31;
                    }
                    if (element.Stereotype == "table")
                    {
                        return 32;
                    }
                    if (element.Stereotype == "text")
                    {
                        return 33;
                    }
                    if (element.Stereotype == "label")
                    {
                        return 34;
                    }
                    if (element.Stereotype == "form")
                    {
                        return 35;
                    }
                    if (element.Stereotype == "panel")
                    {
                        return 36;
                    }
                    if (element.Stereotype == "button")
                    {
                        return 37;
                    }
                    if (element.Stereotype == "combobox")
                    {
                        return 38;
                    }
                    if (element.Stereotype == "checkbox")
                    {
                        return 39;
                    }
                    if (element.Stereotype == "lcheckbox")
                    {
                        return 40;
                    }
                    if (element.Stereotype == "radio")
                    {
                        return 41;
                    }
                    if (element.Stereotype == "lradio")
                    {
                        return 42;
                    }
                    if (element.Stereotype == "vline")
                    {
                        return 43;
                    }
                    if (element.Stereotype == "hline")
                    {
                        return 44;
                    }
                    return -1;
                case "Note":
                    return 45;
              /*  case "InteractionOccurrence":
                    return 46;*/
                case "Requirement":
                    return 47;
              /*  case "Interaction":
                    return 48;*/
                case "StateMachine":
                    return 49;
                default:
                    return -1;
            }
        }

        public int getDiagramType(string GUID)
        {
            string type = ((EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID)).Type;
            string metatype = ((EA.Diagram)model.getWrappedModel().GetDiagramByGuid(GUID)).MetaType;

            switch (type)
            {
                case "Logical":
                    return 50;
                case "Activity":
                    return 51;
                case "Use Case":
                    return 52;
                case "Sequence":
                    return 53;
                case "Statechart":
                    return 54;
                case "Analysis":
                    return 55;
                case "Component":
                    return 56;
                case "Deployment":
                    return 57;
                case "Custom":
                    if (metatype == "Extended::User Interface")
                    {
                        return 58;
                    }
                    if (metatype == "Extended::Requirements")
                    {
                        return 59;
                    }
                    return -1;
                case "Package":
                    return 60;
                default:
                    return -1;
            }
        }

        public int getConnectorType(string GUID)
        {
            EA.Connector connector = (EA.Connector)model.getWrappedModel().GetConnectorByGuid(GUID);
            string type = ((EA.Connector)model.getWrappedModel().GetConnectorByGuid(GUID)).Type;

            switch (type)
            {
                case "Association":
                    return 70;
                case "Generalization":
                    return 71;
                case "Realization":
                    return 72;
                case "UseCase":
                    if (connector.Stereotype == "include")
                    {
                        return 73;
                    }

                    if (connector.Stereotype == "extend")
                    {
                        return 74;
                    }
                    return -1;
                case "ObjectFlow":
                    return 75;
                case "ControlFlow":
                    return 76;
                case "StateFlow":
                    return 77;
               /* case "Sequence":
                    return 78;*/
                case "Dependency":
                    return 79;
                default:
                    return -1;
            }
        }

        public string getElementTypeInSlovak(int elementType)
        {
            switch (elementType)
            {                
                case 0:
                    return "triedy";
                case 1:
                    return "aktivity";
                case 2:
                    return "prípadu použitia";
                case 3:
                    return "balíka";
                case 4:
                    return "procesu";
                case 5:
                    return "rozhrania";
                case 6:
                    return "stavu";
                case 7:
                    return "iniciálneho stavu";
                case 8:
                    return "koncového stavu";
                case 9:
                    return "začiatočného bodu";
                case 10:
                    return "koncového bodu";
                case 11:
                    return "elementu typu ActivityPartition";
                case 12:
                    return "rozhodovacieho bloku";
                case 13:
                    return "bodu zlúčenia";
                case 14:
                    return "objektu";
                case 15:
                    return "zdroja";
                case 16:
                    return "cieľa";
                case 17:
                    return "informácie";
                case 18:
                    return "príjemcu";
                case 19:
                    return "odosielateľa";
              /*  case 20:
                    return "fragmentu typu alt";
                case 21:
                    return "fragmentu typu opt";
                case 22:
                    return "fragmentu typu break";
                case 23:
                    return "fragmentu typu loop";
                case 24:
                    return "čiary života";*/
                case 25:
                    return "komponentu";
                case 26:
                    return "obrazovky";
                case 30:
                    return "aktéra";
                case 31:
                    return "zoznamu";
                case 32:
                    return "tabuľky";
                case 33:
                    return "textu";
                case 34:
                    return "labelu";
                case 35:
                    return "formulára";
                case 36:
                    return "panelu";
                case 37:
                    return "tlačidla";
                case 38:
                    return "comboboxu";
                case 39:
                    return "checkboxu";
                case 40:
                    return "lcheckboxu";
                case 41:
                    return "radio";
                case 42:
                    return "lradio";
                case 43:
                    return "vertikálnej čiary";
                case 44:
                    return "horizontálnej čiary";
                case 45:
                    return "poznámky";
              /*  case 46:
                    return "elementu typu InteractionOccurrence";*/
                case 47:
                    return "požiadavky";
            /*    case 48:
                    return "interakcie";*/
                case 49:
                    return "elementu typu StateMachine";
                case 50:
                    return "diagramu tried";
                case 51:
                    return "diagramu aktivít";
                case 52:
                    return "diagramu prípadov použitia";
                case 53:
                    return "sekvenčného diagramu";
                case 54:
                    return "stavového diagramu";
                case 55:
                    return "biznis procesného modelu";
                case 56:
                    return "diagramu komponentov";
                case 57:
                    return "diagramu rozmiestnenia";
                case 58:
                    return "diagramu používateľského rozhrania";
                case 59:
                    return "diagramu požiadaviek";
                case 60:
                    return "diagramu balíkov";
                case 70:
                    return "asociácie";
                case 71:
                    return "generalizácie";
                case 72:
                    return "realizácie";
                case 73:
                    return "vzťahu include";
                case 74:
                    return "vzťahu extend";
                case 75:
                    return "vzťahu typu ObjectFlow";
                case 76:
                    return "vzťahu typu ControlFlow";
                case 77:
                    return "udalosti";
              /*  case 78:
                    return "správy";*/
                case 79:
                    return "závislosti";
                case 90:
                    return "atribútu";
                default:
                    return "";
            }
        }

        public string getLocationOfItem(EA.Repository repository, int packageID, int parentID)
        {
            EA.Package parentPackage = (EA.Package)repository.GetPackageByID(packageID);
            if (parentID == 0)
            {
                return " balík '" + parentPackage.Name + "')";
            }
            else
            {
                EA.Element parentElement = (EA.Element)repository.GetElementByID(parentID);
                return " element '" + parentElement.Name + "' v balíku '" + parentPackage.Name + "')";
            }
        }
    }
}

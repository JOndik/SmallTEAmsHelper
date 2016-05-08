using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn.SynchronizationPackage
{
    public class SynchronizationAdditions
    {
        private ItemTypes itemTypes;

        public SynchronizationAdditions(EA.Repository repository)
        {
            this.itemTypes = new ItemTypes(repository);
        }

        public String addPackage(EA.Repository Repository, string packageGUID, string name, string author)
        {
            EA.Package parentPackage = (EA.Package)Repository.GetPackageByGuid(packageGUID);
           
            EA.Package newPackage = (EA.Package)parentPackage.Packages.AddNew(name, "");
            newPackage.Update();

            EA.Element packageMetaElement = (EA.Element)newPackage.Element;
            packageMetaElement.Author = author;
            newPackage.Update();

            parentPackage.Packages.Refresh();
            BPAddIn.synchronizationWindow.addToList("Pridanie balíka '" + name + "' do balíka '" + parentPackage.Name
                   + "' - autor: '" + author + "'");

            return newPackage.PackageGUID;
        }

        public string addDiagram(EA.Repository Repository, string parentGUID, string packageGUID, int elementType, string name, string author)
        {
            if (getDiagramType(elementType) == "")
            {
                return "";
            }

            EA.Collection diagrams;
            if (parentGUID == "0")
            {
                diagrams = (EA.Collection)Repository.GetPackageByGuid(packageGUID).Diagrams;
            }
            else 
            {
                diagrams = (EA.Collection)Repository.GetElementByGuid(parentGUID).Diagrams;
            }

            EA.Diagram newDiagram = (EA.Diagram)diagrams.AddNew(name, getDiagramType(elementType));
            newDiagram.Author = author;
            newDiagram.Update();
            diagrams.Refresh();

            EA.Package parentPackage = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            if (parentGUID == "0")
            {              
                BPAddIn.synchronizationWindow.addToList("Pridanie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + name +
                    "' do balíka '" + parentPackage.Name + "' - autor: '" + author + "'");
            }
            else
            {
                EA.Element parentElement = (EA.Element)Repository.GetElementByGuid(parentGUID);
                BPAddIn.synchronizationWindow.addToList("Pridanie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + name +
                    "' do elementu '" + parentElement.Name + "' v balíku '" + parentPackage.Name + "' - autor: '" + author + "'");
            }

            return newDiagram.DiagramGUID;           
        }

        public string addElement(EA.Repository Repository, string parentGUID, string packageGUID, string coordinates, 
            int elementType, string name, string author)
        {
            if (getElementType(elementType) == "" && getElementStereotype(elementType) == "" && getElementSubtype(elementType) == -1)
            {
                return "";
            }

            EA.Collection elements;

            if (parentGUID == "0")
            {
                elements = (EA.Collection)Repository.GetPackageByGuid(packageGUID).Elements;
            }
            else
            {
                elements = (EA.Collection)Repository.GetElementByGuid(parentGUID).Elements;
            }

            if (elementType == 6 || (elementType > 30 && elementType < 45))
            {
                elements = (EA.Collection)Repository.GetPackageByGuid(packageGUID).Elements;
            }

            EA.Element newElement = (EA.Element)elements.AddNew(name, getElementType(elementType));

            if ((elementType >= 7 && elementType <= 10) || (elementType >= 18 && elementType <= 23))
            {
                newElement.Subtype = getElementSubtype(elementType);
            }

            if (elementType == 4 || (elementType >= 15 && elementType <= 17) || (elementType >= 31 && elementType <= 44))
            {
                newElement.Stereotype = getElementStereotype(elementType);
            }

            newElement.Author = author;
            newElement.Update();
            elements.Refresh();

            EA.Package parentPackage = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            if (parentGUID == "0")
            {
                BPAddIn.synchronizationWindow.addToList("Pridanie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + name +
                    "' do balíka '" + parentPackage.Name + "' - autor: '" + author + "'");
            }
            else
            {
                EA.Element parentElement = (EA.Element)Repository.GetElementByGuid(parentGUID);
                BPAddIn.synchronizationWindow.addToList("Pridanie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + name +
                    "' do elementu '" + parentElement.Name + "' v balíku '" + parentPackage.Name + "' - autor: '" + author + "'");
            }
                        
            return newElement.ElementGUID;
        }

        public void addDiagramObject(EA.Repository Repository, string elementGUID, string diagramGUID, string coordinates)
        {
            int left, right, top, bottom;

            string[] coordinate;
            string str;
            string[] parts = coordinates.Split(';');

            str = parts[0];
            coordinate = str.Split('=');
            left = Convert.ToInt32(coordinate[1]);

            str = parts[1];
            coordinate = str.Split('=');
            right = Convert.ToInt32(coordinate[1]);

            str = parts[2];
            coordinate = str.Split('=');
            top = Convert.ToInt32(coordinate[1]);

            str = parts[3];
            coordinate = str.Split('=');
            bottom = Convert.ToInt32(coordinate[1]);

            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObj = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                EA.Element el = (EA.Element)Repository.GetElementByID(diagramObj.ElementID);
                if (diagramObj.left <= left && diagramObj.right >= right && diagramObj.top >= top && diagramObj.bottom <= bottom)
                {
                    diagramObj.Sequence += 1;
                    diagramObj.Update();
                }
            }
            diagram.DiagramObjects.Refresh();

            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew(coordinates, "");

            displayElement.ElementID = element.ElementID;
            displayElement.Sequence = 1;
            displayElement.Update();
            diagram.DiagramObjects.Refresh();      

            EA.Package parentPackage = (EA.Package)Repository.GetPackageByID(diagram.PackageID);
            if (diagram.ParentID == 0)
            {
                BPAddIn.synchronizationWindow.addToList("Pridanie elementu '" + element.Name + "' do diagramu '"
                    + diagram.Name + "' (Umiestnenie diagramu: balík '" + parentPackage.Name + "')");
            }
            else
            {
                EA.Element parentElement = (EA.Element)Repository.GetElementByID(diagram.ParentID);
                BPAddIn.synchronizationWindow.addToList("Pridanie elementu '" + element.Name + "' do diagramu '"
                   + diagram.Name + "' (Umiestnenie diagramu: element '" + parentElement.Name
                   + "' v balíku '" + parentPackage.Name + "')");
            }
        }

        public string addConnector(EA.Repository Repository, string srcGUID, string targetGUID, string name, int elementType)
        {
            EA.Element source = (EA.Element)Repository.GetElementByGuid(srcGUID);
            EA.Element target = (EA.Element)Repository.GetElementByGuid(targetGUID);

            EA.Connector newConnector = (EA.Connector)source.Connectors.AddNew(name, getConnectorType(elementType));

            if (elementType == 73)
            {
                newConnector.Subtype = "Includes";
                newConnector.Stereotype = "include";
            }
            else if (elementType == 74)
            {
                newConnector.Subtype = "Extends";
                newConnector.Stereotype = "extend";
            }

            newConnector.SupplierID = target.ElementID;
            newConnector.Update();
            source.Connectors.Refresh();

            BPAddIn.synchronizationWindow.addToList("Pridanie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + name +
                   "' medzi elementom '" + source.Name + "' a elementom '" + target.Name + "'");

            return newConnector.ConnectorGUID;
        }     

        public string addScenario(EA.Repository Repository, string elementGUID, string name, string type, string XMLContent, int elementType)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            EA.Scenario scenario = (EA.Scenario)element.Scenarios.AddNew(name, type);
            scenario.XMLContent = XMLContent;
            scenario.Update();
            element.Scenarios.Refresh();

            BPAddIn.synchronizationWindow.addToList("Pridanie scenára '" + name + "' typu '" + type + "' do " 
                + itemTypes.getElementTypeInSlovak(elementType) + " '" + element.Name + "' (Umiestnenie elementu: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));

            return scenario.ScenarioGUID;
        }

        public string addAttribute(EA.Repository Repository, string elementGUID, string name, string scope)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            EA.Attribute attribute = (EA.Attribute)element.Attributes.AddNew(name, "");
            attribute.Visibility = scope;
            attribute.Update();

            element.Attributes.Refresh();

            BPAddIn.synchronizationWindow.addToList("Pridanie atribútu '" + name + "' s viditeľnosťou '" + scope 
                + "' do triedy '" + element.Name + "' (Umiestnenie elementu: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));

            return attribute.AttributeGUID;
        }

        public string getElementType(int elementType)
        {
            if (elementType >= 31 && elementType <= 44)
            {
                return "GUIElement";
            }
            switch (elementType)
            {
                case 0:
                    return "Class";
                case 1:
                    return "Activity";
                case 2:
                    return "UseCase";
                case 4:
                    return "Activity";
                case 5:
                    return "Interface";
                case 6:
                    return "State";
                case 7:
                    return "StateNode";
                case 8:
                    return "StateNode";
                case 9:
                    return "StateNode";
                case 10:
                    return "StateNode";
                case 11:
                    return "ActivityPartition";
                case 12:
                    return "Decision";
                case 13:
                    return "MergeNode";
                case 14:
                    return "Object";
                case 15:
                    return "Object";
                case 16:
                    return "Object";
                case 17:
                    return "Object";
                case 18:
                    return "Event";
                case 19:
                    return "Event";
              /*  case 20:
                    return "InteractionFragment";
                case 21:
                    return "InteractionFragment";
                case 22:
                    return "InteractionFragment";
                case 23:
                    return "InteractionFragment";
                case 24:
                    return "Sequence";*/
                case 25:
                    return "Component";
                case 26:
                    return "Screen";
                case 30:
                    return "Actor";
                case 45:
                    return "Note";
            /*    case 46:
                    return "InteractionOccurrence";*/
                case 47:
                    return "Requirement";
              /*  case 48:
                    return "Interaction";*/
                case 49:
                    return "StateMachine";
                default:
                    return "";
            }
        }

        public string getElementStereotype(int elementType)
        {
            switch (elementType)
            {
                case 4:
                    return "process";
                case 15:
                    return "resource";
                case 16:
                    return "goal";
                case 17:
                    return "information";
                case 31:
                    return "list";
                case 32:
                    return "table";
                case 33:
                    return "text";
                case 34:
                    return "label";
                case 35:
                    return "form";
                case 36:
                    return "panel";
                case 37:
                    return "button";
                case 38:
                    return "combobox";
                case 39:
                    return "checkbox";
                case 40:
                    return "lcheckbox";
                case 41:
                    return "radio";
                case 42:
                    return "lradio";
                case 43:
                    return "vline";
                case 44:
                    return "hline";
                default:
                    return "";
            }
        }

        public int getElementSubtype(int elementType)
        {
            switch (elementType)
            {
                case 7:
                    return 3;
                case 8:
                    return 4;
                case 9:
                    return 100;
                case 10:
                    return 101;
                case 18:
                    return 1;
                case 19:
                    return 0;
              /*  case 20:
                    return 0;
                case 21:
                    return 1;
                case 22:
                    return 2;
                case 23:
                    return 4;*/
                default:
                    return -1;
            }
        }


        public string getDiagramType(int elementType)
        {
            switch (elementType)
            {
                case 50:
                    return "Logical";
                case 51:
                    return "Activity";
                case 52:
                    return "Use Case";
                case 53:
                    return "Sequence";
                case 54:
                    return "Statechart";
                case 55:
                    return "Analysis";
                case 56:
                    return "Component";
                case 57:
                    return "Deployment";
                case 58:
                    return "Extended::User Interface";
                case 59:
                    return "Extended::Requirements";
                case 60:
                    return "Package";
                default:
                    return "";
            }
        }

        public string getConnectorType(int elementType)
        {
            switch (elementType)
            {
                case 70:
                    return "Association";
                case 71:
                    return "Generalization";
                case 72:
                    return "Realization";
                case 73:
                    return "UseCase";
                case 74:
                    return "UseCase";
                case 75:
                    return "ObjectFlow";
                case 76:
                    return "ControlFlow";
                case 77:
                    return "StateFlow";
               /* case 78:
                    return "Sequence";*/
                case 79:
                    return "Dependency";
                default:
                    return "";
            }
        }  
    }
}

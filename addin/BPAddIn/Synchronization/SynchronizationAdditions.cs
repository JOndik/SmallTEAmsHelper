using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddIn
{
    public class SynchronizationAdditions
    {
        //public Wrapper.Model model;

        public SynchronizationAdditions()
        {

        }

        /*public SynchronizationAdditions(EA.Repository repository)
        {
            this.model = new Wrapper.Model(repository);
        }*/

        public String addPackage(EA.Repository Repository, string packageGUID, string name, string author)
        {
            EA.Collection packages = (EA.Collection)Repository.GetPackageByGuid(packageGUID).Packages;
            EA.Package newPackage = (EA.Package)packages.AddNew(name, "");
            newPackage.Update();

            EA.Element packageMetaElement = (EA.Element)newPackage.Element;
            packageMetaElement.Author = author;
            newPackage.Update();

            packages.Refresh();
            MessageBox.Show("pridanie balika " + newPackage.PackageGUID);
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
            MessageBox.Show("pridanie diagramu: " + newDiagram.DiagramGUID);
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

            EA.Element newElement = (EA.Element)elements.AddNew(name, getElementType(elementType));

            if ((elementType >= 7 && elementType <= 10) || (elementType >= 18 && elementType <= 23))
            {
                newElement.Subtype = getElementSubtype(elementType);
            }

            if (elementType == 4 || (elementType >= 15 && elementType <= 17) || (elementType >= 31 && elementType <= 44))
            {
                newElement.Stereotype = getElementStereotype(elementType);
            }

            MessageBox.Show("pridanie elementu: " + newElement.ElementGUID);
            newElement.Author = author;
            newElement.Update();
            elements.Refresh();
                        
            return newElement.ElementGUID;
        }

        public void addDiagramObject(EA.Repository Repository, string elementGUID, string diagramGUID, string coordinates)
        {
            MessageBox.Show("pridanie diagram objektu: " + elementGUID);
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
                    MessageBox.Show("naslo: " + el.Name + " " + diagramObj.Sequence);
                    /*if (diagramObj.Sequence > 1)
                    {*/
                        diagramObj.Sequence += 1;
                        diagramObj.Update();
                    //}
                    MessageBox.Show("zvysene: " + el.Name + " " + diagramObj.Sequence);
                }
            }
            diagram.DiagramObjects.Refresh();

            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.DiagramObject displayElement = (EA.DiagramObject)diagram.DiagramObjects.AddNew(coordinates, "");
            //string[] coords = coordinates.Split(';');
            displayElement.ElementID = element.ElementID;
            displayElement.Sequence = 1;
            displayElement.Update();
            diagram.DiagramObjects.Refresh();
            
            /*displayElement.left = Convert.ToInt32(coords[0].Split('=')[1]);
            displayElement.Update();*/
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

            MessageBox.Show("pridanie spojenia guid: " + newConnector.ConnectorGUID);

            return newConnector.ConnectorGUID;
        }     

        public string addScenario(EA.Repository Repository, string elementGUID, string name, string type)
        {
            MessageBox.Show("pridanie scenara");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            EA.Scenario scenario = (EA.Scenario)element.Scenarios.AddNew(name, type);
            scenario.Update();
            element.Scenarios.Refresh();
            

            MessageBox.Show(scenario.ScenarioGUID);

            ///////////////
            EA.ScenarioStep scenarioStep = (EA.ScenarioStep)scenario.Steps.AddNew("a", "stActor");
            scenarioStep.Uses = "";
            scenarioStep.Results = "";
            scenarioStep.State = "";
            scenarioStep.Update();
            scenario.Steps.Refresh();
            element.Scenarios.Refresh();

            /*scenario.Steps.DeleteAt(0, false);
            scenario.Steps.Refresh();
            element.Scenarios.Refresh();*/

            Repository.AdviseElementChange(element.ElementID);
            return scenario.ScenarioGUID;
        }

        public string addScenarioStep(EA.Repository Repository, string elementGUID, string scenarioGUID, int position, string stepType,
            string name, string uses, string results, string state)
        {
            MessageBox.Show("pridanie kroku do scenara");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario scenario = (EA.Scenario)element.Scenarios.GetAt(i);
                MessageBox.Show(scenario.ScenarioGUID + " " + scenarioGUID);
                if (scenario.ScenarioGUID == scenarioGUID)
                {
                    MessageBox.Show("zac if");
                    EA.ScenarioStep scenarioStep = (EA.ScenarioStep)scenario.Steps.AddNew(name, stepType);
                    scenarioStep.Uses = uses;
                    scenarioStep.Results = results;
                    scenarioStep.State = state;
                    scenarioStep.Update();
                    scenario.Steps.Refresh();
                    element.Scenarios.Refresh();
                    MessageBox.Show(scenarioStep.StepGUID);
                    return scenarioStep.StepGUID;
                }
            }
            return "";
        }       

        public string addAttribute(EA.Repository Repository, string elementGUID, string name, string scope)
        {
            MessageBox.Show("pridanie atributu");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            EA.Attribute attribute = (EA.Attribute)element.Attributes.AddNew(name, "");
            attribute.Visibility = scope;
            attribute.Update();

            element.Attributes.Refresh();
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
                case 20:
                    return "InteractionFragment";
                case 21:
                    return "InteractionFragment";
                case 22:
                    return "InteractionFragment";
                case 23:
                    return "InteractionFragment";
                case 24:
                    return "Sequence";
                case 25:
                    return "Component";
                case 26:
                    return "Screen";
                case 30:
                    return "Actor";
                case 45:
                    return "Note";
                case 46:
                    return "InteractionOccurrence";
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
                case 20:
                    return 0;
                case 21:
                    return 1;
                case 22:
                    return 2;
                case 23:
                    return 4;
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
                case 78:
                    return "Sequence";
                case 79:
                    return "Dependency";
                default:
                    return "";
            }
        }  
    }
}

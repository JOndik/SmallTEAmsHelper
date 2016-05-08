using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn
{
    public class SynchronizationDeletions
    {
        public void deletePackage(EA.Repository Repository, string packageGUID)
        {
            MessageBox.Show("odstranenie balika");
            EA.Package packageToDelete = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Package parentPackage = (EA.Package)Repository.GetPackageByID(packageToDelete.ParentID);

            for (short i = 0; i < parentPackage.Packages.Count; i++)
            {
                EA.Package actualPackage = (EA.Package)parentPackage.Packages.GetAt(i);
                if (actualPackage.PackageGUID == packageGUID)
                {
                    parentPackage.Packages.DeleteAt(i, false);
                    break;
                }
            }
            parentPackage.Packages.Refresh();
        }

        public void deleteDiagram(EA.Repository Repository, string diagramGUID)
        {
            MessageBox.Show("odstranenie diagramu");
            EA.Diagram diagramToDelete = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            EA.Collection diagrams;
            if (diagramToDelete.ParentID == 0)
            {
                diagrams = (EA.Collection)Repository.GetPackageByID(diagramToDelete.PackageID).Diagrams;
            }
            else
            {
                diagrams = (EA.Collection)Repository.GetElementByID(diagramToDelete.ParentID).Diagrams;
            }

            for (short i = 0; i < diagrams.Count; i++)
            {
                EA.Diagram actualDiagram = (EA.Diagram)diagrams.GetAt(i);
                if (actualDiagram.DiagramGUID == diagramGUID)
                {
                    diagrams.DeleteAt(i, false);
                    break;
                }
            }
            diagrams.Refresh();
        }

        public void deleteElement(EA.Repository Repository, string elementGUID)
        {
            MessageBox.Show("odstranenie elementu");
            EA.Element elementToDelete = (EA.Element)Repository.GetElementByGuid(elementGUID);
            EA.Collection elements;
            if (elementToDelete.ParentID == 0)
            {
                elements = (EA.Collection)Repository.GetPackageByID(elementToDelete.PackageID).Elements;
            }
            else
            {
                elements = (EA.Collection)Repository.GetElementByID(elementToDelete.ParentID).Elements;
            }

            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element actualElement = (EA.Element)elements.GetAt(i);
                if (actualElement.ElementGUID == elementGUID)
                {
                    elements.DeleteAt(i, false);
                    break;
                }
            }
            elements.Refresh();
        }

        public void deleteDiagramObject(EA.Repository Repository, string elementGUID, string diagramGUID)
        {
            MessageBox.Show("odstranenie diagram objektu");
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                if (diagramObject.ElementID == element.ElementID)
                {
                    diagram.DiagramObjects.DeleteAt(i, false);
                    break;
                }
            }
            diagram.DiagramObjects.Refresh();
        }

        public void deleteConnector(EA.Repository Repository, string connectorGUID, string diagramGUID)
        {
            /*EA.Connector connectorToDelete = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            MessageBox.Show("odstranenie spojenia guid: " + connectorGUID + " v diagrame: " + diagram.Name + " " + diagram.DiagramGUID);

            for (short i = 0; i < diagram.DiagramLinks.Count; i++)
            {
                EA.DiagramLink actualDiagramLink = (EA.DiagramLink)diagram.DiagramLinks.GetAt(i);
                if (actualDiagramLink.ConnectorID == connectorToDelete.ConnectorID)
                {
                    MessageBox.Show("delete");
                    diagram.DiagramLinks.DeleteAt(i, false);
                    break;
                }
            }
            diagram.DiagramLinks.Refresh();*/

            MessageBox.Show("odstranenie spojenia guid: " + connectorGUID);

            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            EA.Element element = (EA.Element)Repository.GetElementByID(connector.ClientID);
            for (short i = 0; i < element.Connectors.Count; i++)
            {
                EA.Connector conn = (EA.Connector)element.Connectors.GetAt(i);
                if (conn.ConnectorGUID == connectorGUID)
                {
                    element.Connectors.DeleteAt(i, false);
                    break;
                }
            }
        }

        public void deleteScenario(EA.Repository Repository, string scenarioGUID, string elementGUID)
        {
            MessageBox.Show("odstranenie scenara");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario actualScenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (actualScenario.ScenarioGUID == scenarioGUID)
                {
                    element.Scenarios.DeleteAt(i, false);
                    break;
                }
            }
            element.Scenarios.Refresh();
        }

        public void deleteScenarioStep(EA.Repository Repository, string elementGUID, string scenarioGUID, string stepGUID)
        {
            MessageBox.Show("pridanie kroku do scenara");
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario scenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (scenario.ScenarioGUID == scenarioGUID)
                {
                    for (short j = 0; j < scenario.Steps.Count; j++)
                    {
                        EA.ScenarioStep scenarioStep = (EA.ScenarioStep)scenario.Steps.GetAt(i);
                        if (scenarioStep.StepGUID == stepGUID)
                        {
                            scenario.Steps.DeleteAt(j, false);
                            scenario.Update();
                            break;
                        }
                    }
                }
            }
            element.Scenarios.Refresh();
        }

        public void deleteAttribute(EA.Repository Repository, string attributeGUID)
        {
            MessageBox.Show("odstranenie atributu");
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            EA.Element element = (EA.Element)Repository.GetElementByID(attribute.ParentID);

            for (short i = 0; i < element.Attributes.Count; i++)
            {
                EA.Attribute actualAttribute = (EA.Attribute)element.Attributes.GetAt(i);
                if (actualAttribute.AttributeGUID == attributeGUID)
                {
                    element.Attributes.DeleteAt(i, false);
                    break;
                }
            }
            element.Attributes.Refresh();
        }

        public void deleteConstraint(EA.Repository repository, string elementGUID, string name, string type)
        {
            MessageBox.Show("odstranenie obmedzenia");
            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);

            for (short i = 0; i < element.Constraints.Count; i++)
            {
                EA.Constraint actualConstraint = (EA.Constraint)element.Constraints.GetAt(i);
                if (actualConstraint.Name == name && actualConstraint.Type == type)
                {
                    element.Constraints.DeleteAt(i, false);
                    break;
                }
            }
            element.Constraints.Refresh();
        }
    }
}

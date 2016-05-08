using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BPAddIn.SynchronizationPackage
{
    public class SynchronizationDeletions
    {
        private ItemTypes itemTypes;

        public SynchronizationDeletions(EA.Repository repository)
        {
            this.itemTypes = new ItemTypes(repository);
        }

        public void deletePackage(EA.Repository Repository, string packageGUID)
        {
            EA.Package packageToDelete = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            string name = packageToDelete.Name;
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

            BPAddIn.synchronizationWindow.addToList("Odstránenie balíka '" + name + "' z balíka '" + parentPackage.Name + "'");
        }

        public void deleteDiagram(EA.Repository Repository, string diagramGUID, int elementType)
        {
            EA.Diagram diagramToDelete = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            string name = diagramToDelete.Name;
            int packageID = diagramToDelete.PackageID;
            int parentID = diagramToDelete.ParentID;

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

            BPAddIn.synchronizationWindow.addToList("Odstránenie " + itemTypes.getElementTypeInSlovak(elementType) + " '" + 
                name + "' (Pôvodné umiestnenie diagramu: " +
                itemTypes.getLocationOfItem(Repository, packageID, parentID));
        }

        public void deleteElement(EA.Repository Repository, string elementGUID, int elementType)
        {
            EA.Element elementToDelete = (EA.Element)Repository.GetElementByGuid(elementGUID);
            string name = elementToDelete.Name;
            int packageID = elementToDelete.PackageID;
            int parentID = elementToDelete.ParentID;

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

            BPAddIn.synchronizationWindow.addToList("Odstránenie " + itemTypes.getElementTypeInSlovak(elementType) + " '" 
                + name + "' (Pôvodné umiestnenie elementu: " +
                itemTypes.getLocationOfItem(Repository, packageID, parentID));
        }

        public void deleteDiagramObject(EA.Repository Repository, string elementGUID, string diagramGUID)
        {
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

            BPAddIn.synchronizationWindow.addToList("Odstránenie elementu '" + element.Name + "' z diagramu '" + diagram.Name + 
                "' (Umiestnenie diagramu: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        public void deleteConnector(EA.Repository Repository, string connectorGUID, string diagramGUID, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            string name = connector.Name;
            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            for (short i = 0; i < srcElement.Connectors.Count; i++)
            {
                EA.Connector conn = (EA.Connector)srcElement.Connectors.GetAt(i);
                if (conn.ConnectorGUID == connectorGUID)
                {
                    srcElement.Connectors.DeleteAt(i, false);
                    break;
                }
            }
            srcElement.Connectors.Refresh();

            BPAddIn.synchronizationWindow.addToList("Odstránenie " + itemTypes.getElementTypeInSlovak(elementType) + " '" 
                + name + "' medzi elementom '" + srcElement.Name +
                "' a elementom '" + targetElement.Name + "'");
        }

        public void deleteScenario(EA.Repository Repository, string scenarioGUID, string elementGUID, int elementType)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            string name = "", type = "";
            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario currentScenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (currentScenario.ScenarioGUID == scenarioGUID)
                {
                    name = currentScenario.Name;
                    type = currentScenario.Type;
                    element.Scenarios.DeleteAt(i, false);
                    break;
                }
            }
            element.Scenarios.Refresh();

            BPAddIn.synchronizationWindow.addToList("Odstránenie scenára '" + name + "' typu '" + type + 
                "' z " + itemTypes.getElementTypeInSlovak(elementType) + " '" + element.Name 
                + "' (Umiestnenie elementu: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void deleteAttribute(EA.Repository Repository, string attributeGUID)
        {
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            string name = attribute.Name;
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

            BPAddIn.synchronizationWindow.addToList("Odstránenie atribútu '" + name + "' z elementu '" + element.Name 
                + "' (Umiestnenie elementu: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void deleteConstraint(EA.Repository repository, string elementGUID, string name, string type)
        {
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

            BPAddIn.synchronizationWindow.addToList("Odstránenie obmedzenia '" + name + "' z elementu '" + element.Name 
                + "' (Umiestnenie elementu: " + itemTypes.getLocationOfItem(repository, element.PackageID, element.ParentID));
        }
    }
}

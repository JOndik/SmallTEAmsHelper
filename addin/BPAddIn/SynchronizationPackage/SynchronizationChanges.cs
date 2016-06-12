using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;

namespace BPAddIn.SynchronizationPackage
{
    public class SynchronizationChanges
    {
        private ItemTypes itemTypes;

        public SynchronizationChanges(EA.Repository repository)
        {
            this.itemTypes = new ItemTypes(repository);
        }

        public void changePackageName(EA.Repository Repository, string packageGUID, string name, string oldName)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            package.Name = name;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of package '" + name + "' - former name: '" + oldName + 
                "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        public void changePackageAuthor(EA.Repository Repository, string packageGUID, string author, string oldAuthor)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Element packageElement = (EA.Element)package.Element;
            packageElement.Author = author;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author of package '" + package.Name + "' - former author: '" + oldAuthor +
                 "', current author: '" + author + "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        public void changePackageNotes(EA.Repository Repository, string packageGUID, string notes)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            package.Notes = notes;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of package '" + package.Name +
                "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        public void changeDiagramName(EA.Repository Repository, string diagramGUID, string name, string oldName, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Name = name;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInSlovak(elementType) + " '" + 
                name + "' - former name: '" + oldName +
                "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        public void changeDiagramAuthor(EA.Repository Repository, string diagramGUID, string author, string oldAuthor, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Author = author;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
              diagram.Name + "' - former author: '" + oldAuthor + "', current autor: '" + author +
              "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        public void changeDiagramNotes(EA.Repository Repository, string diagramGUID, string notes, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Notes = notes;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInSlovak(elementType) + " '" + diagram.Name +
                 "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        public void changeDiagramStereotype(EA.Repository Repository, string diagramGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Stereotype = stereotype;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInSlovak(elementType) + " '" + diagram.Name
                + "' - former stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
                 "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        public void changeElementName(EA.Repository Repository, string elementGUID, string name, string oldName, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Name = name;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInSlovak(elementType) + " '" + 
                name + "' - former name: '" + oldName +
                 "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeElementAuthor(EA.Repository Repository, string elementGUID, string author, string oldAuthor, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Author = author;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author of" + itemTypes.getElementTypeInSlovak(elementType) + " '" + 
                element.Name + "' - former author: '" + oldAuthor + "', current author: '" + author +
                  "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeElementNotes(EA.Repository Repository, string elementGUID, string notes, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Notes = notes;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
                element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeElementStereotype(EA.Repository Repository, string elementGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Stereotype = stereotype;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
                element.Name + "' - former stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
                  "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeConnectorName(EA.Repository Repository, string connectorGUID, string name, string oldName, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Name = name;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former name: '" + oldName + "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorNotes(EA.Repository Repository, string connectorGUID, string notes, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Notes = notes;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorStereotype(EA.Repository Repository, string connectorGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Stereotype = stereotype;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorSource(EA.Repository Repository, string connectorGUID, string sourceGUID, string oldSourceGUID, int elementType)
        {
            EA.Element sourceElement = (EA.Element)Repository.GetElementByGuid(sourceGUID);
            EA.Element oldSourceElement = (EA.Element)Repository.GetElementByGuid(oldSourceGUID);

            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientID = sourceElement.ElementID;
            connector.Update();

            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of source element of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former source element: '" + oldSourceElement.Name + "', current source element: '" + sourceElement.Name +
               "' (Connector between element '" + sourceElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorTarget(EA.Repository Repository, string connectorGUID, string targetGUID, string oldTargetGUID, int elementType)
        {
            EA.Element targetElement = (EA.Element)Repository.GetElementByGuid(targetGUID);
            EA.Element oldTargetElement = (EA.Element)Repository.GetElementByGuid(oldTargetGUID);

            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierID = targetElement.ElementID;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);

            BPAddIn.synchronizationWindow.addToList("Change of target element of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former target element: '" + oldTargetElement.Name + "', current target element: '" + targetElement.Name +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorSourceCardinality(EA.Repository Repository, string connectorGUID, string cardinality, string oldCardinality, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientEnd.Cardinality = cardinality;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of cardinality of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' at source element - former cardinality: '" + oldCardinality + "', current cardinality: '" + cardinality +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorTargetCardinality(EA.Repository Repository, string connectorGUID, string cardinality, string oldCardinality, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierEnd.Cardinality = cardinality;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of cardinality of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' at target element - former cardinality: '" + oldCardinality + "', current cardinality: '" + cardinality +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorGuard(EA.Repository Repository, string connectorGUID, string guard, string oldGuard, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.TransitionGuard = guard;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of guard of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former guard: '" + oldGuard + "', current guard: '" + guard +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void changeConnectorDirection(EA.Repository Repository, string connectorGUID, string direction, string oldDirection, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Direction = direction;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of direction of " + itemTypes.getElementTypeInSlovak(elementType) + " '" +
               connector.Name + "' - former direction: '" + oldDirection + "', current direction: '" + direction +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        public void setExtensionPoints(EA.Repository Repository, string elementGUID, string extensionPoints, int elementType)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            element.ExtensionPoints = extensionPoints;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of extension points of " + itemTypes.getElementTypeInSlovak(elementType)
                + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeAttributeName(EA.Repository Repository, string attributeGUID, string name, string oldName)
        {
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Name = name;
            attribute.Update();

            EA.Element element = (EA.Element)Repository.GetElementByID(attribute.ParentID);

            BPAddIn.synchronizationWindow.addToList("Change of name of attribute '" +
               attribute.Name + "' - former name: '" + oldName + "', current name: '" + name +
               "' (Attribute of element '" + element.Name + "', location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeAttributeVisibility(EA.Repository Repository, string attributeGUID, string scope, string oldScope)
        {
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Visibility = scope;
            attribute.Update();

            EA.Element element = (EA.Element)Repository.GetElementByID(attribute.ParentID);

            BPAddIn.synchronizationWindow.addToList("Change of scope of attribute '" +
               attribute.Name + "' - former scope: '" + oldScope + "', current scope: '" + scope +
               "' (Attribute of element '" + element.Name + "', location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        public void changeScenario(EA.Repository repository, string scenarioGUID, string elementGUID, string name, string type, string XMLContent, int elementType)
        {
            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);
            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario currentScenario = (EA.Scenario)element.Scenarios.GetAt(i);
                if (currentScenario.ScenarioGUID == scenarioGUID)
                {
                    currentScenario.Name = name;
                    currentScenario.Type = type;
                    currentScenario.XMLContent = XMLContent;
                    currentScenario.Update();
                    break;
                }
            }
            element.Scenarios.Refresh();

            BPAddIn.synchronizationWindow.addToList("Change of scenario '" + name + "' of type '" + type + "' "
                + itemTypes.getElementTypeInSlovak(elementType) + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(repository, element.PackageID, element.ParentID));
        }

        public void addConstraint(EA.Repository repository, string elementGUID, string name, string type, int elementType)
        {
            int index = name.IndexOf("notes:=") + 7;

            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);
            EA.Constraint constraint = (EA.Constraint)element.Constraints.AddNew(name.Substring(0, index-8), type);
            constraint.Notes = name.Substring(index, name.Length - index);
            constraint.Update();
            element.Constraints.Refresh();

            BPAddIn.synchronizationWindow.addToList("Addition of constraint '" + name.Substring(0, index - 8) + "' of type '" + type + "' to "
               + itemTypes.getElementTypeInSlovak(elementType) + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(repository, element.PackageID, element.ParentID));
        }
    }
}

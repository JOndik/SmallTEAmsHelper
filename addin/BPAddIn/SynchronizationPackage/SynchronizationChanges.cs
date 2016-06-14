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

        /// <summary>
        /// method changes name of package
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="packageGUID">GUID of changed package</param>
        /// <param name="name">new name of changed package</param>
        /// <param name="oldName">previous name of changed package</param>
        public void changePackageName(EA.Repository Repository, string packageGUID, string name, string oldName)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            package.Name = name;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of package '" + name + "' - previous name: '" + oldName + 
                "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        /// <summary>
        /// method changes author of package
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="packageGUID">GUID of changed package</param>
        /// <param name="author">new author of changed package</param>
        /// <param name="oldAuthor">previous author of changed package</param>
        public void changePackageAuthor(EA.Repository Repository, string packageGUID, string author, string oldAuthor)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            EA.Element packageElement = (EA.Element)package.Element;
            packageElement.Author = author;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author of package '" + package.Name + "' - previous author: '" + oldAuthor +
                 "', current author: '" + author + "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        /// <summary>
        /// method changes notes of package
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="packageGUID">GUID of changed package</param>
        /// <param name="notes">new notes of changed package</param>
        public void changePackageNotes(EA.Repository Repository, string packageGUID, string notes)
        {
            EA.Package package = (EA.Package)Repository.GetPackageByGuid(packageGUID);
            package.Notes = notes;
            package.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of package '" + package.Name +
                "' (Location of package: " + itemTypes.getLocationOfItem(Repository, package.ParentID, 0));
        }

        /// <summary>
        /// method changes name of diagram
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of changed diagram</param>
        /// <param name="name">new name of changed diagram</param>
        /// <param name="oldName">previous name of changed diagram</param>
        /// <param name="elementType">type of changed diagram</param>
        public void changeDiagramName(EA.Repository Repository, string diagramGUID, string name, string oldName, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Name = name;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
                name + "' - previous name: '" + oldName +
                "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        /// <summary>
        /// method changes author of diagram
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of changed diagram</param>
        /// <param name="author">new author of changed diagram</param>
        /// <param name="oldAuthor">previous author of changed diagram</param>
        /// <param name="elementType">type of changed diagram</param>
        public void changeDiagramAuthor(EA.Repository Repository, string diagramGUID, string author, string oldAuthor, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Author = author;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
              diagram.Name + "' - previous author: '" + oldAuthor + "', current autor: '" + author +
              "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        /// <summary>
        /// method changes notes of diagram
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of changed diagram</param>
        /// <param name="notes">new notes of changed diagram</param>
        /// <param name="elementType">type of changed diagram</param>
        public void changeDiagramNotes(EA.Repository Repository, string diagramGUID, string notes, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Notes = notes;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInEnglish(elementType) + " '" + diagram.Name +
                 "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        /// <summary>
        /// method changes stereotype of diagram
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="diagramGUID">GUID of changed diagram</param>
        /// <param name="stereotype">new stereotype of changed diagram</param>
        /// <param name="oldStereotype">previous stereotype of changed diagram</param>
        /// <param name="elementType">type of changed diagram</param>
        public void changeDiagramStereotype(EA.Repository Repository, string diagramGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Diagram diagram = (EA.Diagram)Repository.GetDiagramByGuid(diagramGUID);
            diagram.Stereotype = stereotype;
            diagram.Update();

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInEnglish(elementType) + " '" + diagram.Name
                + "' - previous stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
                 "' (Location of diagram: " + itemTypes.getLocationOfItem(Repository, diagram.PackageID, diagram.ParentID));
        }

        /// <summary>
        /// method changes name of element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="name">new name of changed element</param>
        /// <param name="oldName">previous name of changed element</param>
        /// <param name="elementType">type of changed element</param>
        public void changeElementName(EA.Repository Repository, string elementGUID, string name, string oldName, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Name = name;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
                name + "' - previous name: '" + oldName +
                 "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes author of element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="author">new author of changed element</param>
        /// <param name="oldAuthor">previous author of changed element</param>
        /// <param name="elementType">type of changed element</param>
        public void changeElementAuthor(EA.Repository Repository, string elementGUID, string author, string oldAuthor, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Author = author;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of author of" + itemTypes.getElementTypeInEnglish(elementType) + " '" +
                element.Name + "' - previous author: '" + oldAuthor + "', current author: '" + author +
                  "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes notes of element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="notes">new notes of changed element</param>
        /// <param name="elementType">type of changed element</param>
        public void changeElementNotes(EA.Repository Repository, string elementGUID, string notes, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Notes = notes;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
                element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes stereotype of element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="stereotype">new stereotype of changed element</param>
        /// <param name="oldStereotype">previous stereotype of changed element</param>
        /// <param name="elementType">type of changed element</param>
        public void changeElementStereotype(EA.Repository Repository, string elementGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Element element = Repository.GetElementByGuid(elementGUID);
            element.Stereotype = stereotype;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
                element.Name + "' - previous stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
                  "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes name of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="name">new name of changed connector</param>
        /// <param name="oldName">previous name of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorName(EA.Repository Repository, string connectorGUID, string name, string oldName, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Name = name;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of name of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous name: '" + oldName + "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes notes of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="notes">new notes of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorNotes(EA.Repository Repository, string connectorGUID, string notes, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Notes = notes;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of notes of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes stereotype of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="stereotype">new stereotype of changed connector</param>
        /// <param name="oldStereotype">previous stereotype of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorStereotype(EA.Repository Repository, string connectorGUID, string stereotype, string oldStereotype, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Stereotype = stereotype;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of stereotype of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous stereotype: '" + oldStereotype + "', current stereotype: '" + stereotype +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes source element of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="sourceGUID">GUID of new source element of changed connector</param>
        /// <param name="oldSourceGUID">GUID of previous source element of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorSource(EA.Repository Repository, string connectorGUID, string sourceGUID, string oldSourceGUID, int elementType)
        {
            EA.Element sourceElement = (EA.Element)Repository.GetElementByGuid(sourceGUID);
            EA.Element oldSourceElement = (EA.Element)Repository.GetElementByGuid(oldSourceGUID);

            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientID = sourceElement.ElementID;
            connector.Update();

            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of source element of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous source element: '" + oldSourceElement.Name + "', current source element: '" + sourceElement.Name +
               "' (Connector between element '" + sourceElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes target element of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="targetGUID">GUID of new target element of changed connector</param>
        /// <param name="oldTargetGUID">GUID of previous target element of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorTarget(EA.Repository Repository, string connectorGUID, string targetGUID, string oldTargetGUID, int elementType)
        {
            EA.Element targetElement = (EA.Element)Repository.GetElementByGuid(targetGUID);
            EA.Element oldTargetElement = (EA.Element)Repository.GetElementByGuid(oldTargetGUID);

            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierID = targetElement.ElementID;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);

            BPAddIn.synchronizationWindow.addToList("Change of target element of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous target element: '" + oldTargetElement.Name + "', current target element: '" + targetElement.Name +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes cardinality of connector at source element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="cardinality">new cardinality of changed connector at source element</param>
        /// <param name="oldCardinality">previous cardinality of changed connector at source element</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorSourceCardinality(EA.Repository Repository, string connectorGUID, string cardinality, string oldCardinality, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.ClientEnd.Cardinality = cardinality;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of cardinality of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' at source element - previous cardinality: '" + oldCardinality + "', current cardinality: '" + cardinality +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes cardinality of connector at target element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector at target element</param>
        /// <param name="cardinality">new cardinality of changed connector at target element</param>
        /// <param name="oldCardinality">old cardinality of changed connector at target element</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorTargetCardinality(EA.Repository Repository, string connectorGUID, string cardinality, string oldCardinality, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.SupplierEnd.Cardinality = cardinality;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of cardinality of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' at target element - previous cardinality: '" + oldCardinality + "', current cardinality: '" + cardinality +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes guard of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="guard">new guard of changed connector</param>
        /// <param name="oldGuard">previous guard of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorGuard(EA.Repository Repository, string connectorGUID, string guard, string oldGuard, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.TransitionGuard = guard;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of guard of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous guard: '" + oldGuard + "', current guard: '" + guard +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method changes direction of connector
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="connectorGUID">GUID of changed connector</param>
        /// <param name="direction">new direction of changed connector</param>
        /// <param name="oldDirection">previous direction of changed connector</param>
        /// <param name="elementType">type of changed connector</param>
        public void changeConnectorDirection(EA.Repository Repository, string connectorGUID, string direction, string oldDirection, int elementType)
        {
            EA.Connector connector = (EA.Connector)Repository.GetConnectorByGuid(connectorGUID);
            connector.Direction = direction;
            connector.Update();

            EA.Element srcElement = (EA.Element)Repository.GetElementByID(connector.ClientID);
            EA.Element targetElement = (EA.Element)Repository.GetElementByID(connector.SupplierID);

            BPAddIn.synchronizationWindow.addToList("Change of direction of " + itemTypes.getElementTypeInEnglish(elementType) + " '" +
               connector.Name + "' - previous direction: '" + oldDirection + "', current direction: '" + direction +
               "' (Connector between element '" + srcElement.Name + "' and element '" + targetElement.Name + "')");
        }

        /// <summary>
        /// method sets extension points of element
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="extensionPoints">new extension points</param>
        /// <param name="elementType">type of changed element</param>
        public void setExtensionPoints(EA.Repository Repository, string elementGUID, string extensionPoints, int elementType)
        {
            EA.Element element = (EA.Element)Repository.GetElementByGuid(elementGUID);
            element.ExtensionPoints = extensionPoints;
            element.Update();

            BPAddIn.synchronizationWindow.addToList("Change of extension points of " + itemTypes.getElementTypeInEnglish(elementType)
                + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes name of attribute
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="attributeGUID">GUID of changed attribute</param>
        /// <param name="name">new name of changed attribute</param>
        /// <param name="oldName">previous name of changed attribute</param>
        public void changeAttributeName(EA.Repository Repository, string attributeGUID, string name, string oldName)
        {
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Name = name;
            attribute.Update();

            EA.Element element = (EA.Element)Repository.GetElementByID(attribute.ParentID);

            BPAddIn.synchronizationWindow.addToList("Change of name of attribute '" +
               attribute.Name + "' - previous name: '" + oldName + "', current name: '" + name +
               "' (Attribute of element '" + element.Name + "', location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes scope of attribute
        /// </summary>
        /// <param name="Repository">EA repository</param>
        /// <param name="attributeGUID">GUID of changed attribute</param>
        /// <param name="scope">new scope of changed attribute</param>
        /// <param name="oldScope">previous scope of changed attribute</param>
        public void changeAttributeVisibility(EA.Repository Repository, string attributeGUID, string scope, string oldScope)
        {
            EA.Attribute attribute = (EA.Attribute)Repository.GetAttributeByGuid(attributeGUID);
            attribute.Visibility = scope;
            attribute.Update();

            EA.Element element = (EA.Element)Repository.GetElementByID(attribute.ParentID);

            BPAddIn.synchronizationWindow.addToList("Change of scope of attribute '" +
               attribute.Name + "' - previous scope: '" + oldScope + "', current scope: '" + scope +
               "' (Attribute of element '" + element.Name + "', location of element: " + itemTypes.getLocationOfItem(Repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method changes scenario of element
        /// </summary>
        /// <param name="repository">EA repository</param>
        /// <param name="scenarioGUID">GUID of changed scenario</param>
        /// <param name="elementGUID">GUID of element</param>
        /// <param name="name">new name of changed scenario</param>
        /// <param name="type">new type of changed scenario</param>
        /// <param name="XMLContent">new steps of changed scenario</param>
        /// <param name="elementType">type of element</param>
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
                + itemTypes.getElementTypeInEnglish(elementType) + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(repository, element.PackageID, element.ParentID));
        }

        /// <summary>
        /// method adds constraint to element
        /// </summary>
        /// <param name="repository">EA repository</param>
        /// <param name="elementGUID">GUID of changed element</param>
        /// <param name="name">name of new constraint</param>
        /// <param name="type">type of new constraint</param>
        /// <param name="elementType">type of changed element</param>
        public void addConstraint(EA.Repository repository, string elementGUID, string name, string type, int elementType)
        {
            int index = name.IndexOf("notes:=") + 7;

            EA.Element element = (EA.Element)repository.GetElementByGuid(elementGUID);
            EA.Constraint constraint = (EA.Constraint)element.Constraints.AddNew(name.Substring(0, index-8), type);
            constraint.Notes = name.Substring(index, name.Length - index);
            constraint.Update();
            element.Constraints.Refresh();

            BPAddIn.synchronizationWindow.addToList("Addition of constraint '" + name.Substring(0, index - 8) + "' of type '" + type + "' to "
               + itemTypes.getElementTypeInEnglish(elementType) + " '" + element.Name + "' (Location of element: " + itemTypes.getLocationOfItem(repository, element.PackageID, element.ParentID));
        }
    }
}

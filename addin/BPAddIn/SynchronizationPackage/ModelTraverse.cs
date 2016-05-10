using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using System.Windows.Forms;

namespace BPAddIn.SynchronizationPackage
{
    public class ModelTraverse
    {
        private ItemTypes itemTypes;
        private ChangeService changeService;
        public Wrapper.Model model;
        public HashSet<String> connectorGUIDs;

        public ModelTraverse(EA.Repository repository)
        {
            this.itemTypes = new ItemTypes(repository);
            this.changeService = new ChangeService();
            this.model = new Wrapper.Model(repository);
        }

        public void sendDataAboutModel(EA.Repository Repository)
        {
            this.connectorGUIDs = new HashSet<string>();
            for (short i = 0; i < Repository.Models.Count; i++)
            {
                EA.Package model = (EA.Package)Repository.Models.GetAt(i);

                if (model.Packages.Count > 0)
                {
                    traversePackages(Repository, model.Packages);
                    traverseModelForElements(Repository, model.Packages);
                    traverseModelForDiagrams(Repository, model.Packages);                   
                    traverseModelForDiagramObjects(Repository, model.Packages);
                    traverseModelForConnectors(Repository, model.Packages);
                }
            }
        }

        public void traversePackages(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Name == "Dokumentácia")
                {
                    continue;
                }

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = package.PackageGUID;
                itemCreation.elementType = 3;
                itemCreation.author = package.Element.Author;
                itemCreation.name = package.Name;
                itemCreation.parentGUID = "0";
                EA.Package parentPackage = repository.GetPackageByID(package.ParentID);
                if (parentPackage != null)
                {
                    itemCreation.packageGUID = parentPackage.PackageGUID;
                }

                saveCreate(itemCreation, repository);

                if (package.Packages.Count > 0)
                {
                    traversePackages(repository, package.Packages);
                }
            }
        }

        public void traverseModelForDiagrams(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Name == "Dokumentácia")
                {
                    continue;
                }

                if (package.Diagrams.Count > 0)
                {
                    traverseDiagrams(repository, package.Diagrams);
                }
                if (package.Elements.Count > 0)
                {
                    traverseDiagramsInElements(repository, package.Elements);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForDiagrams(repository, package.Packages);
                }
            }
        }

        public void traverseModelForConnectors(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Name == "Dokumentácia")
                {
                    continue;
                }

                if (package.Elements.Count > 0)
                {
                    traverseConnectors(repository, package.Elements);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForConnectors(repository, package.Packages);
                }
            }
        }

        public void traverseDiagramsInElements(EA.Repository repository, EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element element = (EA.Element)elements.GetAt(i);

                if (element.Diagrams.Count > 0)
                {
                    traverseDiagrams(repository, element.Diagrams);
                }
                if (element.Elements.Count > 0)
                {
                    traverseDiagramsInElements(repository, element.Elements);
                }
            }
        }

        public void traverseModelForElements(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Name == "Dokumentácia")
                {
                    continue;
                }

                if (package.Elements.Count > 0)
                {
                    traverseElements(repository, package.Elements);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForElements(repository, package.Packages);
                }
            }
        }

        public void traverseModelForDiagramObjects(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Name == "Dokumentácia")
                {
                    continue;
                }

                if (package.Diagrams.Count > 0)
                {
                    traverseDiagramsForDiagramObjects(repository, package.Diagrams);
                }
                if (package.Elements.Count > 0)
                {
                    traverseDiagramsInElementsForDiagramObjects(repository, package.Elements);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForDiagramObjects(repository, package.Packages);
                }
            }
        }

        public void traverseDiagramsInElementsForDiagramObjects(EA.Repository repository, EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element element = (EA.Element)elements.GetAt(i);

                if (element.Diagrams.Count > 0)
                {
                    traverseDiagramsForDiagramObjects(repository, element.Diagrams);
                }
                if (element.Elements.Count > 0)
                {
                    traverseDiagramsInElementsForDiagramObjects(repository, element.Elements);
                }
            }
        }

        public void traverseDiagramsForDiagramObjects(EA.Repository repository, EA.Collection diagrams)
        {
            for (short i = 0; i < diagrams.Count; i++)
            {
                EA.Diagram diagram = (EA.Diagram)diagrams.GetAt(i);

                if (diagram.DiagramObjects.Count > 0)
                {
                    traverseDiagramObjects(repository, diagram);
                }
            }
        }

        public void traverseDiagrams(EA.Repository repository, EA.Collection diagrams)
        {
            for (short i = 0; i < diagrams.Count; i++)
            {
                EA.Diagram diagram = (EA.Diagram)diagrams.GetAt(i);

                if (itemTypes.getDiagramType(diagram.DiagramGUID) == -1)
                {
                    continue;
                }

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = diagram.DiagramGUID;
                itemCreation.elementType = itemTypes.getDiagramType(diagram.DiagramGUID);
                itemCreation.author = diagram.Author;
                itemCreation.name = diagram.Name;
                itemCreation.parentGUID = "0";

                if (diagram.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(diagram.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }

                EA.Package package = repository.GetPackageByID(diagram.PackageID);
                if (package != null)
                {
                    itemCreation.packageGUID = package.PackageGUID;
                }

                saveCreate(itemCreation, repository);

                if (diagram.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = diagram.DiagramGUID;
                    propertyChange.elementType = itemTypes.getDiagramType(diagram.DiagramGUID);
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = diagram.Notes;
                    saveCreate(propertyChange, repository);
                }
                if (diagram.Stereotype != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = diagram.DiagramGUID;
                    propertyChange.elementType = itemTypes.getDiagramType(diagram.DiagramGUID);
                    propertyChange.propertyType = 200;
                    propertyChange.propertyBody = diagram.Stereotype;
                    propertyChange.oldPropertyBody = "";
                    saveCreate(propertyChange, repository);
                }
            }
        }

        public void traverseConnectors(EA.Repository repository, EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element element = (EA.Element)elements.GetAt(i);

                if (itemTypes.getElementType(element.ElementGUID) == -1)
                {
                    continue;
                }

                for (short j = 0; j < element.Connectors.Count; j++)
                {
                    EA.Connector currentConnector = (EA.Connector)element.Connectors.GetAt(j);

                    if (itemTypes.getConnectorType(currentConnector.ConnectorGUID) == -1)
                    {
                        continue;
                    }

                    if (!connectorGUIDs.Contains(currentConnector.ConnectorGUID))
                    {
                        connectorGUIDs.Add(currentConnector.ConnectorGUID);

                        ItemCreation itemCreation = new ItemCreation();
                        itemCreation.itemGUID = currentConnector.ConnectorGUID;
                        itemCreation.elementType = itemTypes.getConnectorType(currentConnector.ConnectorGUID);
                        itemCreation.name = currentConnector.Name;
                        itemCreation.srcGUID = repository.GetElementByID(currentConnector.ClientID).ElementGUID;
                        itemCreation.targetGUID = repository.GetElementByID(currentConnector.SupplierID).ElementGUID;

                        saveCreate(itemCreation, repository);

                        if (currentConnector.ClientEnd.Cardinality != "")
                        {
                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.itemGUID = currentConnector.ConnectorGUID;
                            propertyChange.elementType = itemTypes.getConnectorType(currentConnector.ConnectorGUID);
                            propertyChange.propertyType = 303;
                            propertyChange.propertyBody = currentConnector.ClientEnd.Cardinality;
                            propertyChange.oldPropertyBody = "";
                            saveCreate(propertyChange, repository);
                        }

                        if (currentConnector.SupplierEnd.Cardinality != "")
                        {
                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.itemGUID = currentConnector.ConnectorGUID;
                            propertyChange.elementType = itemTypes.getConnectorType(currentConnector.ConnectorGUID);
                            propertyChange.propertyType = 304;
                            propertyChange.propertyBody = currentConnector.SupplierEnd.Cardinality;
                            propertyChange.oldPropertyBody = "";
                            saveCreate(propertyChange, repository);
                        }

                        if (currentConnector.TransitionGuard != "")
                        {
                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.itemGUID = currentConnector.ConnectorGUID;
                            propertyChange.elementType = itemTypes.getConnectorType(currentConnector.ConnectorGUID);
                            propertyChange.propertyType = 305;
                            propertyChange.propertyBody = currentConnector.TransitionGuard;
                            propertyChange.oldPropertyBody = "";
                            saveCreate(propertyChange, repository);
                        }

                        if (currentConnector.Direction != "Unspecified")
                        {
                            PropertyChange propertyChange = new PropertyChange();
                            propertyChange.itemGUID = currentConnector.ConnectorGUID;
                            propertyChange.elementType = itemTypes.getConnectorType(currentConnector.ConnectorGUID);
                            propertyChange.propertyType = 307;
                            propertyChange.propertyBody = currentConnector.Direction;
                            propertyChange.oldPropertyBody = "Unspecified";
                            saveCreate(propertyChange, repository);
                        }
                    }
                }

                if (element.Elements.Count > 0)
                {
                    traverseConnectors(repository, element.Elements);
                }
            }
        }

        public void traverseElements(EA.Repository repository, EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element element = (EA.Element)elements.GetAt(i);

                if (itemTypes.getElementType(element.ElementGUID) == -1)
                {
                    continue;
                }

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = element.ElementGUID;
                itemCreation.elementType = itemTypes.getElementType(element.ElementGUID);
                itemCreation.author = element.Author;
                itemCreation.name = element.Name;
                itemCreation.parentGUID = "0";

                if (element.ParentID != 0)
                {
                    EA.Element parent = repository.GetElementByID(element.ParentID);
                    if (parent != null)
                    {
                        itemCreation.parentGUID = parent.ElementGUID;
                    }
                }

                EA.Package package = repository.GetPackageByID(element.PackageID);
                if (package != null)
                {
                    itemCreation.packageGUID = package.PackageGUID;
                }

                saveCreate(itemCreation, repository);

                if (element.Stereotype != "")
                {
                    int itemType = itemTypes.getElementType(element.ElementGUID);
                    if (itemType == 4 || (itemType >= 15 && itemType <= 17) || (itemType >= 31 && itemType <= 44) || itemType == 26)
                    {

                    }
                    else
                    {
                        PropertyChange propertyChange = new PropertyChange();
                        propertyChange.itemGUID = element.ElementGUID;
                        propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                        propertyChange.propertyType = 200;
                        propertyChange.propertyBody = element.Stereotype;
                        propertyChange.oldPropertyBody = "";
                        saveCreate(propertyChange, repository);
                    }
                }

                if (element.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = element.ElementGUID;
                    propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = element.Notes;
                    saveCreate(propertyChange, repository);
                }

                if (element.ExtensionPoints != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = element.ElementGUID;
                    propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                    propertyChange.propertyType = 13;
                    propertyChange.propertyBody = element.ExtensionPoints;
                    propertyChange.oldPropertyBody = "";
                    saveCreate(propertyChange, repository);
                }

                if (element.Attributes.Count > 0)
                {
                    traverseAttributes(repository, element);
                }

                if (element.Scenarios.Count > 0)
                {
                    traverseScenarios(repository, element);
                }

                if (element.Constraints.Count > 0)
                {
                    traverseConstraints(repository, element);
                }

                if (element.Elements.Count > 0)
                {
                    traverseElements(repository, element.Elements);
                }
            }
        }

        public void traverseConstraints(EA.Repository repository, EA.Element element)
        {
            for (short i = 0; i < element.Constraints.Count; i++)
            {
                EA.Constraint constraint = (EA.Constraint)element.Constraints.GetAt(i);

                PropertyChange propertyChange = new PropertyChange();
                propertyChange.itemGUID = element.ElementGUID;
                propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                propertyChange.propertyType = 10;
                propertyChange.propertyBody = constraint.Name + ",notes:=" + constraint.Notes;
                propertyChange.oldPropertyBody = constraint.Type;

                saveCreate(propertyChange, repository);
            }
        }

        public void traverseAttributes(EA.Repository repository, EA.Element element)
        {
            for (short i = 0; i < element.Attributes.Count; i++)
            {
                EA.Attribute attribute = (EA.Attribute)element.Attributes.GetAt(i);

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = attribute.AttributeGUID;
                itemCreation.elementType = 90;
                itemCreation.name = attribute.Name;
                itemCreation.coordinates = attribute.Visibility;
                itemCreation.author = attribute.Type;
                itemCreation.parentGUID = element.ElementGUID;

                saveCreate(itemCreation, repository);

                if (attribute.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = attribute.AttributeGUID;
                    propertyChange.elementType = 90;
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = attribute.Notes;
                    propertyChange.oldPropertyBody = "";
                    saveCreate(propertyChange, repository);
                }
            }
        }

        public void traverseScenarios(EA.Repository repository, EA.Element element)
        {
            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario scenario = (EA.Scenario)element.Scenarios.GetAt(i);

                StepChange scenarioChange = new StepChange();
                scenarioChange.scenarioGUID = scenario.ScenarioGUID;
                scenarioChange.itemGUID = element.ElementGUID;
                scenarioChange.elementType = itemTypes.getElementType(element.ElementGUID);
                scenarioChange.name = scenario.Name;
                scenarioChange.stepType = scenario.Type;
                scenarioChange.state = scenario.XMLContent;
                scenarioChange.status = 1;

                saveCreate(scenarioChange, repository);
            }
        }

        public void traverseDiagramObjects(EA.Repository repository, EA.Diagram diagram)
        {
            string coordinates = "";

            if (itemTypes.getDiagramType(diagram.DiagramGUID) == -1)
            {
                return;
            }

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                EA.Element element = repository.GetElementByID(diagramObject.ElementID);

                if (itemTypes.getElementType(element.ElementGUID) == -1)
                {
                    continue;
                }

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.elementType = 700;
                itemCreation.itemGUID = element.ElementGUID;
                itemCreation.diagramGUID = diagram.DiagramGUID;
                itemCreation.name = element.Name;
                coordinates += "l=" + diagramObject.left + ";";
                coordinates += "r=" + diagramObject.right + ";";
                coordinates += "t=" + diagramObject.top + ";";
                coordinates += "b=" + diagramObject.bottom + ";";
                itemCreation.coordinates = coordinates;

                coordinates = "";

                saveCreate(itemCreation, repository);
            }
        }

        public void saveCreate(ModelChange change, EA.Repository repository)
        {
            change.modelGUID = repository.GetPackageByID(1).PackageGUID;
            change.timestamp = repository.GetPackageByID(1).PackageGUID;
            changeService.saveChange(change);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using Wrapper = TSF.UmlToolingFramework.Wrappers.EA;
using System.Windows.Forms;

namespace BPAddIn
{
    public class ModelTraverse
    {
        private ItemTypes itemTypes;
        private ChangeService changeService;
        public Wrapper.Model model;

        public ModelTraverse(EA.Repository repository)
        {
            this.itemTypes = new ItemTypes(repository);
            this.changeService = new ChangeService();
            this.model = new Wrapper.Model(repository);
        }

        public void sendDataAboutModel(EA.Repository Repository)
        {
            for (short i = 0; i < Repository.Models.Count; i++)
            {
                EA.Package model = (EA.Package)Repository.Models.GetAt(i);

                if (model.Packages.Count > 0)
                {
                    traversePackages(Repository, model.Packages);
                    traverseModelForElements(Repository, model.Packages);
                    traverseModelForDiagrams(Repository, model.Packages);                   
                    traverseModelForDiagramLinksAndObjects(Repository, model.Packages);
                }
            }
        }

        public void traversePackages(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

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

                if (package.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = package.PackageGUID;
                    propertyChange.elementType = 3;
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = package.Notes;
                    saveCreate(propertyChange, repository);
                }

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

                if (package.Diagrams.Count > 0)
                {
                    traverseDiagrams(repository, package.Diagrams);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForDiagrams(repository, package.Packages);
                }
            }
        }

        public void traverseModelForElements(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

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

        public void traverseModelForDiagramLinksAndObjects(EA.Repository repository, EA.Collection packages)
        {
            for (short i = 0; i < packages.Count; i++)
            {
                EA.Package package = (EA.Package)packages.GetAt(i);

                if (package.Diagrams.Count > 0)
                {
                    traverseModelDiagrams(repository, package.Diagrams);
                }
                if (package.Packages.Count > 0)
                {
                    traverseModelForDiagramLinksAndObjects(repository, package.Packages);
                }
            }
        }

        public void traverseModelDiagrams(EA.Repository repository, EA.Collection diagrams)
        {
            for (short i = 0; i < diagrams.Count; i++)
            {
                EA.Diagram diagram = (EA.Diagram)diagrams.GetAt(i);

                if (diagram.DiagramLinks.Count > 0)
                {
                    traverseDiagramLinks(repository, diagram.DiagramLinks);
                }
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
                    saveCreate(propertyChange, repository);
                }
            }
        }

        public void traverseElements(EA.Repository repository, EA.Collection elements)
        {
            for (short i = 0; i < elements.Count; i++)
            {
                EA.Element element = (EA.Element)elements.GetAt(i);

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

                string sqlGetDiagram = @"select do.Diagram_ID from t_diagramobjects do
                                        inner join t_object o on do.Object_ID = o.Object_ID
                                        where o.Object_ID=" + element.ElementID;

                List<Wrapper.Diagram> diagrams = model.getDiagramsByQuery(sqlGetDiagram);

                if (diagrams.Count > 0)
                {
                    Wrapper.Diagram diagram = diagrams.ElementAt(0);
                    itemCreation.diagramGUID = diagram.diagramGUID;
                }

                saveCreate(itemCreation, repository);

                if (element.Stereotype != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = element.ElementGUID;
                    propertyChange.elementType = itemTypes.getElementType(element.ElementGUID);
                    propertyChange.propertyType = 200;
                    propertyChange.propertyBody = element.Stereotype;
                    saveCreate(propertyChange, repository);
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

                if (element.Elements.Count > 0)
                {
                    traverseElements(repository, element.Elements);
                }
            }
        }

        public void traverseAttributes(EA.Repository repository, EA.Element element)
        {
            for (short i = 0; i < element.Attributes.Count; i++)
            {
                EA.Attribute attribute = (EA.Attribute)element.Attributes.GetAt(i);
                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = attribute.AttributeGUID;
                itemCreation.elementType = 100;
                itemCreation.name = attribute.Name;
                itemCreation.coordinates = attribute.Visibility;
                itemCreation.author = attribute.Type;

                saveCreate(itemCreation, repository);

                if (attribute.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    propertyChange.itemGUID = attribute.AttributeGUID;
                    propertyChange.elementType = 100;
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = attribute.Notes;
                    saveCreate(propertyChange, repository);
                }
            }
        }

        public void traverseScenarios(EA.Repository repository, EA.Element element)
        {
            for (short i = 0; i < element.Scenarios.Count; i++)
            {
                EA.Scenario scenario = (EA.Scenario)element.Scenarios.GetAt(i);

                ScenarioChange scenarioChange = new ScenarioChange();
                scenarioChange.scenarioGUID = scenario.ScenarioGUID;
                scenarioChange.itemGUID = element.ElementGUID;
                scenarioChange.elementType = itemTypes.getElementType(element.ElementGUID);
                scenarioChange.name = scenario.Name;
                scenarioChange.type = scenario.Type;
                scenarioChange.status = 1;

                saveCreate(scenarioChange, repository);

                if (scenario.Steps.Count > 0)
                {
                    traverseScenarioSteps(repository, element, scenario);
                }
            }
        }

        public void traverseScenarioSteps(EA.Repository repository, EA.Element element, EA.Scenario scenario)
        {
            for (short i = 0; i < scenario.Steps.Count; i++)
            {
                EA.ScenarioStep scenarioStep = (EA.ScenarioStep)scenario.Steps.GetAt(i);

                StepChange stepChange = new StepChange();
                stepChange.itemGUID = element.ElementGUID;
                stepChange.elementType = itemTypes.getElementType(element.ElementGUID);
                stepChange.status = 1;
                stepChange.scenarioGUID = scenario.ScenarioGUID;
                stepChange.stepGUID = scenarioStep.StepGUID;
                stepChange.stepType = scenarioStep.StepType.ToString();
                stepChange.position = scenarioStep.Pos;
                stepChange.name = scenarioStep.Name;
                stepChange.uses = scenarioStep.Uses;
                stepChange.results = scenarioStep.Results;
                stepChange.state = scenarioStep.State;
                stepChange.extensionGUID = "";
                stepChange.joiningStepGUID = "";
                stepChange.joiningStepPosition = "";

                foreach (EA.ScenarioExtension scenarioExtension in scenarioStep.Extensions)
                {
                    stepChange.extensionGUID += scenarioExtension.ExtensionGUID + ",";
                    stepChange.joiningStepGUID += scenarioExtension.Join + ",";
                    stepChange.joiningStepPosition += scenarioExtension.JoiningStep == null ? "" : scenarioExtension.JoiningStep.Pos + ",";
                }

                saveCreate(stepChange, repository);
            }
        }

        public void traverseDiagramLinks(EA.Repository repository, EA.Collection diagramLinks)
        {
            for (short i = 0; i < diagramLinks.Count; i++)
            {
                EA.DiagramLink diagramLink = (EA.DiagramLink)diagramLinks.GetAt(i);
                EA.Connector connector = repository.GetConnectorByID(diagramLink.ConnectorID);

                ItemCreation itemCreation = new ItemCreation();
                itemCreation.itemGUID = connector.ConnectorGUID;
                itemCreation.elementType = itemTypes.getConnectorType(connector.ConnectorGUID);
                itemCreation.name = connector.Name;
                itemCreation.srcGUID = repository.GetElementByID(connector.ClientID).ElementGUID;
                itemCreation.targetGUID = repository.GetElementByID(connector.SupplierID).ElementGUID;

                saveCreate(itemCreation, repository);

                if (connector.Notes != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    itemCreation.itemGUID = connector.ConnectorGUID;
                    itemCreation.elementType = itemTypes.getConnectorType(connector.ConnectorGUID);
                    propertyChange.propertyType = 2;
                    propertyChange.propertyBody = connector.Notes;
                    saveCreate(propertyChange, repository);
                }
                if (connector.Stereotype != "")
                {
                    PropertyChange propertyChange = new PropertyChange();
                    itemCreation.itemGUID = connector.ConnectorGUID;
                    itemCreation.elementType = itemTypes.getConnectorType(connector.ConnectorGUID);
                    propertyChange.propertyType = 200;
                    propertyChange.propertyBody = connector.Stereotype;
                    saveCreate(propertyChange, repository);
                }
            }
        }

        public void traverseDiagramObjects(EA.Repository repository, EA.Diagram diagram)
        {
            string coordinates = "";

            for (short i = 0; i < diagram.DiagramObjects.Count; i++)
            {
                EA.DiagramObject diagramObject = (EA.DiagramObject)diagram.DiagramObjects.GetAt(i);
                EA.Element element = repository.GetElementByID(diagramObject.ElementID);

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
            change.timestamp = "0";
            changeService.saveChange(change);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using System.Windows.Forms;

namespace BPAddIn.SynchronizationPackage
{
    public class Synchronization
    {
        private SynchronizationMovements synchronizationMovements;
        private SynchronizationDeletions synchronizationDeletions;
        private SynchronizationAdditions synchronizationAdditions;
        private SynchronizationChanges synchronizationChanges;

        public Synchronization(EA.Repository repository)
        {
            this.synchronizationMovements = new SynchronizationMovements(repository);
            this.synchronizationDeletions = new SynchronizationDeletions(repository);
            this.synchronizationAdditions = new SynchronizationAdditions(repository);
            this.synchronizationChanges = new SynchronizationChanges(repository);
        }

        public string handleSynchronizationAdditions(ItemCreation itemCreation, EA.Repository repository)
        {
            string GUID = "";
            if (itemCreation.elementType == 3)
            {
                GUID = synchronizationAdditions.addPackage(repository, itemCreation.packageGUID, itemCreation.name, itemCreation.author);
            }
            else if (itemCreation.elementType >= 50 && itemCreation.elementType < 70)
            {
                GUID = synchronizationAdditions.addDiagram(repository, itemCreation.parentGUID, itemCreation.packageGUID,
                    itemCreation.elementType, itemCreation.name, itemCreation.author);
            }
            else if (itemCreation.elementType < 50)
            {
                GUID = synchronizationAdditions.addElement(repository, itemCreation.parentGUID, itemCreation.packageGUID,
                    itemCreation.coordinates, itemCreation.elementType, itemCreation.name, itemCreation.author);
            }
            else if (itemCreation.elementType >= 70 && itemCreation.elementType <= 79)
            {
                GUID = synchronizationAdditions.addConnector(repository, itemCreation.srcGUID, itemCreation.targetGUID, itemCreation.name, itemCreation.elementType);
            }
            else if (itemCreation.elementType == 90)
            {
                GUID = synchronizationAdditions.addAttribute(repository, itemCreation.parentGUID, itemCreation.name, itemCreation.coordinates);
            }
            else if (itemCreation.elementType == 700)
            {
                synchronizationAdditions.addDiagramObject(repository, itemCreation.itemGUID, itemCreation.diagramGUID, itemCreation.coordinates);
                GUID = "";
            }
            return GUID;
        }

        public void handleSynchronizationChanges(PropertyChange propertyChange, EA.Repository repository)
        {
            switch (propertyChange.propertyType)
            {
                case 0:                                                                 //zmena mena
                    if (propertyChange.elementType == 3)
                    {
                        synchronizationChanges.changePackageName(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
                    }
                    else if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementName(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType < 70)
                    {
                        synchronizationChanges.changeDiagramName(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorName(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType == 90)
                    {
                        synchronizationChanges.changeAttributeName(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
                    }
                    break;
                case 1:                                                                 //zmena autora
                    if (propertyChange.elementType == 3)
                    {
                        synchronizationChanges.changePackageAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType < 70)
                    {
                        synchronizationChanges.changeDiagramAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    break;
                case 2:                                                                 //zmena poznamok
                    if (propertyChange.elementType == 3)
                    {
                        synchronizationChanges.changePackageNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType < 70)
                    {
                        synchronizationChanges.changeDiagramNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.elementType);
                    }
                    break;
                case 13:                                                                 //zmena bodov rozsirenia
                    synchronizationChanges.setExtensionPoints(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.elementType);
                    break;
                case 200:                                                               //zmena stereotypu
                    if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType < 70)
                    {
                        synchronizationChanges.changeDiagramStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    }
                    break;
                case 300:                                                                  //zmena scope atributu
                    synchronizationChanges.changeAttributeVisibility(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
                    break;
                case 301:                                                                   //zmena cieloveho elementu spojenia
                    synchronizationChanges.changeConnectorTarget(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 302:                                                                   //zmena zdrojoveho elementu spojenia   
                    synchronizationChanges.changeConnectorSource(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 303:                                                                   //zmena kardinality zdroja spojenia  
                    synchronizationChanges.changeConnectorSourceCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 304:                                                                   //zmena kardinality ciela spojenia 
                    synchronizationChanges.changeConnectorTargetCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 305:                                                                   //zmena guard spojenia
                    synchronizationChanges.changeConnectorGuard(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 307:                                                                   //zmena smeru spojenia
                    synchronizationChanges.changeConnectorDirection(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 10:                                                                   //pridanie obmedzenia
                    synchronizationChanges.addConstraint(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody, propertyChange.elementType);
                    break;
                case 401:                                                                   //presun elementu alebo balika do balika
                    synchronizationMovements.moveElementOrPackageToPackage(repository, propertyChange.itemGUID, propertyChange.propertyBody, 
                    propertyChange.elementType);
                    break;
                case 402:                                                                   //presun elementu do elementu
                    synchronizationMovements.moveElementToElement(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 403:                                                                   //zmena diagramu do elementu
                    synchronizationMovements.moveDiagramToElement(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 404:                                                                   //zmena diagramu do balika
                    synchronizationMovements.moveDiagramToPackage(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 405:                                                                   //zmena suradnic elementu v diagrame
                    synchronizationMovements.moveElementInDiagram(repository, propertyChange.itemGUID, propertyChange.oldPropertyBody,
                        propertyChange.propertyBody);
                    break;
            }           
        }

        public void handleSynchronizationDeletions(PropertyChange propertyChange, EA.Repository repository)
        {
            if (propertyChange.elementType == 3)                   //odstranenie
            {
                synchronizationDeletions.deletePackage(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.elementType >= 50 && propertyChange.elementType < 70)
            {
                synchronizationDeletions.deleteDiagram(repository, propertyChange.itemGUID, propertyChange.elementType);
            }
            else if (propertyChange.propertyType == 11)
            {
                synchronizationDeletions.deleteConstraint(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
            }
            else if (propertyChange.elementType < 50)
            {
                synchronizationDeletions.deleteElement(repository, propertyChange.itemGUID, propertyChange.elementType);
            }
            else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
            {
                synchronizationDeletions.deleteConnector(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.elementType);
            }
            else if (propertyChange.elementType == 90)
            {
                synchronizationDeletions.deleteAttribute(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.elementType == 700)
            {
                synchronizationDeletions.deleteDiagramObject(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
        }

        public string handleScenarioAddition(StepChange scenarioChange, EA.Repository repository)
        {
            return synchronizationAdditions.addScenario(repository, scenarioChange.itemGUID, scenarioChange.name, scenarioChange.stepType, scenarioChange.state, scenarioChange.elementType);
        }

        public void handleScenarioChange(StepChange scenarioChange, EA.Repository repository)
        {
            if (scenarioChange.status == 2)
            {
                synchronizationChanges.changeScenario(repository, scenarioChange.scenarioGUID, scenarioChange.itemGUID, scenarioChange.name,
                    scenarioChange.stepType, scenarioChange.state, scenarioChange.elementType);
            }
            else if (scenarioChange.status == 0)
            {
                synchronizationDeletions.deleteScenario(repository, scenarioChange.scenarioGUID, scenarioChange.itemGUID, scenarioChange.elementType);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPAddIn.DataContract;
using System.Windows.Forms;

namespace BPAddIn
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
            this.synchronizationDeletions = new SynchronizationDeletions();
            this.synchronizationAdditions = new SynchronizationAdditions();
            this.synchronizationChanges = new SynchronizationChanges(repository);
        }

        public string handleSynchronizationAdditions(ItemCreation itemCreation, EA.Repository repository)
        {
            string GUID = "";
            if (itemCreation.elementType == 3)
            {
                GUID = synchronizationAdditions.addPackage(repository, itemCreation.packageGUID, itemCreation.name, itemCreation.author);
            }
            else if (itemCreation.elementType >= 50 && itemCreation.elementType <= 59)
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
                    if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                    {
                        synchronizationChanges.changeDiagramName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType == 90)
                    {
                        synchronizationChanges.changeAttributeName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    break;
                case 1:                                                                 //zmena autora
                    if (propertyChange.elementType == 3)
                    {
                        synchronizationChanges.changePackageAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                    {
                        synchronizationChanges.changeDiagramAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    break;
                case 2:                                                                 //zmena poznamok
                    if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                    {
                        synchronizationChanges.changeDiagramNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    break;
                case 13:                                                                 //zmena bodov rozsirenia
                    synchronizationChanges.setExtensionPoints(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 200:                                                               //zmena stereotypu
                    if (propertyChange.elementType < 50)
                    {
                        synchronizationChanges.changeElementStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                    {
                        synchronizationChanges.changeDiagramStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                    {
                        synchronizationChanges.changeConnectorStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    }
                    break;
                case 300:                                                                  //zmena scope atributu
                    synchronizationChanges.changeAttributeVisibility(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 301:                                                                   //zmena cieloveho elementu spojenia
                    synchronizationChanges.changeConnectorTarget(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 302:                                                                   //zmena zdrojoveho elementu spojenia   
                    synchronizationChanges.changeConnectorSource(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 303:                                                                   //zmena kardinality zdroja spojenia  
                    synchronizationChanges.changeConnectorSourceCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 304:                                                                   //zmena kardinality ciela spojenia 
                    synchronizationChanges.changeConnectorTargetCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 305:                                                                   //zmena guard spojenia
                    synchronizationChanges.changeConnectorGuard(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 307:                                                                   //zmena smeru spojenia
                    synchronizationChanges.changeConnectorDirection(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                    break;
                case 10:                                                                   //pridanie obmedzenia
                    synchronizationChanges.addConstraint(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
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
            
            /*if (propertyChange.propertyType == 0)                   //zmena mena
            {
                if (propertyChange.elementType < 50)
                {
                    synchronizationChanges.changeElementName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                {
                    synchronizationChanges.changeDiagramName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                {
                    synchronizationChanges.changeConnectorName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType == 90){
                    synchronizationChanges.changeAttributeName(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
            }
            else if (propertyChange.propertyType == 1)              //zmena autora
            {
                if (propertyChange.elementType == 3)
                {
                    synchronizationChanges.changePackageAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                {
                    synchronizationChanges.changeDiagramAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType < 50)
                {
                    synchronizationChanges.changeElementAuthor(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
            }
            else if (propertyChange.propertyType == 2)              //zmena poznamok
            {
                if (propertyChange.elementType < 50)
                {
                    synchronizationChanges.changeElementNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                {
                    synchronizationChanges.changeDiagramNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                {
                    synchronizationChanges.changeConnectorNotes(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
            }
            /*else if (propertyChange.propertyType == 13)                 //zmena extension points
            {
                synchronizationChanges.setExtensionPoints(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }*/
            /*else if (propertyChange.propertyType == 200)              //zmena stereotypu
            {
                if (propertyChange.elementType < 50)
                {
                    synchronizationChanges.changeElementStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 50 && propertyChange.elementType <= 59)
                {
                    synchronizationChanges.changeDiagramStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
                else if (propertyChange.elementType >= 70 && propertyChange.elementType <= 79)
                {
                    synchronizationChanges.changeConnectorStereotype(repository, propertyChange.itemGUID, propertyChange.propertyBody);
                }
            }
            else if (propertyChange.propertyType == 300)
            {
                synchronizationChanges.changeAttributeVisibility(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 301)           //zmena cieloveho elementu spojenia
            {
                synchronizationChanges.changeConnectorTarget(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 302)            //zmena zdrojoveho elementu spojenia
            {
                synchronizationChanges.changeConnectorSource(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 303)            //zmena kardinality zdroja spojenia
            {
                synchronizationChanges.changeConnectorSourceCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 304)            //zmena kardinality ciela spojenia
            {
                synchronizationChanges.changeConnectorTargetCardinality(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 305)            //zmena guard spojenia
            {
                synchronizationChanges.changeConnectorGuard(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 307)            //zmena smeru spojenia
            {
                synchronizationChanges.changeConnectorDirection(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 10)             //pridanie obmedzenia
            {
                synchronizationChanges.addConstraint(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
            }
            else if (propertyChange.propertyType == 401)
            {
                synchronizationMovements.moveElementOrPackageToPackage(repository, propertyChange.itemGUID, propertyChange.propertyBody, 
                    propertyChange.elementType);
            }
            else if (propertyChange.propertyType == 402)
            {
                synchronizationMovements.moveElementToElement(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 404)
            {
                synchronizationMovements.moveDiagramToPackage(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 403)
            {
                synchronizationMovements.moveDiagramToElement(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }*/
            
        }

        /*public void handleSynchronizationMovements(PropertyChange propertyChange, EA.Repository repository)
        {
            if (propertyChange.propertyType == 303)        //presun elementu do balika
            {
                synchronizationMovements.moveElementToPackage(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.propertyType == 304)        //presun elementu do elementu
            {
                synchronizationMovements.moveElementToElement(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
        }*/

        public void handleSynchronizationDeletions(PropertyChange propertyChange, EA.Repository repository)
        {
            if (propertyChange.elementType == 3)                   //odstranenie
            {
                synchronizationDeletions.deletePackage(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.elementType == 50)
            {
                synchronizationDeletions.deleteDiagram(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.elementType == 0)
            {
                synchronizationDeletions.deleteElement(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.elementType == 70)
            {
                synchronizationDeletions.deleteConnector(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
            else if (propertyChange.elementType == 90)
            {
                synchronizationDeletions.deleteAttribute(repository, propertyChange.itemGUID);
            }
            else if (propertyChange.propertyType == 11)
            {
                synchronizationDeletions.deleteConstraint(repository, propertyChange.itemGUID, propertyChange.propertyBody, propertyChange.oldPropertyBody);
            }
            else if (propertyChange.elementType == 700)
            {
                synchronizationDeletions.deleteDiagramObject(repository, propertyChange.itemGUID, propertyChange.propertyBody);
            }
        }

        public string handleScenarioAddition(ScenarioChange scenarioChange, EA.Repository repository)
        {
            return synchronizationAdditions.addScenario(repository, scenarioChange.itemGUID, scenarioChange.name, scenarioChange.type);
        }

        public void handleScenarioChange(ScenarioChange scenarioChange, EA.Repository repository)
        {
            if (scenarioChange.status == 2)
            {
                synchronizationChanges.changeScenario(repository, scenarioChange.scenarioGUID, scenarioChange.itemGUID, scenarioChange.name,
                    scenarioChange.type);
            }
            else if (scenarioChange.status == 0)
            {
                synchronizationDeletions.deleteScenario(repository, scenarioChange.scenarioGUID, scenarioChange.itemGUID);
            }
        }

        public string handleScenarioStepAddition(StepChange stepChange, EA.Repository repository)
        {
            return synchronizationAdditions.addScenarioStep(repository, stepChange.itemGUID, stepChange.scenarioGUID, stepChange.position,
                stepChange.stepType.ToString(), stepChange.name, stepChange.uses, stepChange.results, stepChange.state);
        }

        public void handleScenarioStepChange(StepChange stepChange, EA.Repository repository)
        {
            if (stepChange.status == 2)
            {
                synchronizationChanges.changeScenarioStep(repository, stepChange.itemGUID, stepChange.scenarioGUID, stepChange.stepGUID,
                    stepChange.position, stepChange.stepType.ToString(), stepChange.name, stepChange.uses, stepChange.results, stepChange.state);
            }
            else if (stepChange.status == 0)
            {
                synchronizationDeletions.deleteScenarioStep(repository, stepChange.itemGUID, stepChange.scenarioGUID, stepChange.stepGUID);
            }
        }
    }
}

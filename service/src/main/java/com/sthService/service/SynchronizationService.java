package com.sthService.service;

import com.sthService.dataContract.*;
import com.sthService.repository.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.ArrayList;
import java.util.List;

@Service
@Transactional
public class SynchronizationService {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamRepository smallTeamRepository;

    @Inject
    private ModelChangeRepository modelChangeRepository;

    @Inject
    private CorrespondenceNodeRepository correspondenceNodeRepository;

    @Inject
    private ChangesForSyncRepository changesForSyncRepository;

    @Inject
    private AuthorizationRepository authorizationRepository;

    @Inject
    private CorrNodePartRepository corrNodePartRepository;

    private final Logger log = LoggerFactory.getLogger(SynchronizationService.class);

    public String checkSynchronizationInfo(ModelInformation modelInformation){
        User actualUser = authorizationService.getUser(modelInformation.getToken());
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(actualUser.getId());
        if(smallTeam == null){
            log.info("noJoin");
            return "noJoin";
        } else {
            log.info(String.valueOf(actualUser.isAllModelData()));
            if (actualUser.isAllModelData()) {                                  //ak uz poslal cely model
                if (smallTeam.isSynchronizationAllowed()) {
                    if (actualUser.getModelGUID().equals(modelInformation.getModelGUID())) {            // ak uz prebieha sync a poslal zo spravneho modelu
                        log.info("sync");
                        return "true";
                    } else {                                                    // ak uz prebieha sync ale neposlal zo spravneho modelu
                        log.info("wrong");
                        return "wrong";
                    }
                } else {                                                        //ak este neprebieha sync
                    log.info("nosync");
                    return "noSync";
                }
            } else {                                                            //ak este neposlal model
                log.info("nodata");
                return "false";
            }
        }
    }

    public int getSizeOfChangeIDs(String token){
        User user = authorizationRepository.findByToken(token);
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null){
            return 0;
        }
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        if (changeIDs != null) {
            return changeIDs.size();
        } else {
            return 0;
        }
    }

    public ModelChange getChangeForSynchronization(String token){
        String elementGUID, srcGUID, targetGUID, diagramGUID, endElementGUID, packageGUID, parentGUID, scenarioGUID, stepGUID;
        User user = authorizationRepository.findByToken(token);
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null){
            return null;
        }
        ModelChange modelChange;
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        if (changeIDs.size() > 0){
            String changeID = changeIDs.get(0);
            modelChange = modelChangeRepository.findById(changeID);
            changesForSynchronization.getChangeIDs().remove(0);
            changesForSyncRepository.save(changesForSynchronization);
            if (modelChange instanceof ItemCreation) {

                if (modelChange.getElementType() == 3) {                                                //pridanie balika
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() >= 50 && modelChange.getElementType() <= 59){           //pridanie diagramu
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() < 50){                                          //pridanie elementu
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() >= 70 && modelChange.getElementType() <= 79){           //pridanie spojenia
                    srcGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getSrcGUID(), smallTeam, user);
                    targetGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getTargetGUID(), smallTeam, user);
                    if (!("").equals(srcGUID) && !("").equals(targetGUID)){
                        log.info("getChangeForSynchronization");
                        log.info("ItemCreation of connection: " + srcGUID + " " + targetGUID);
                        ((ItemCreation) modelChange).setSrcGUID(srcGUID);
                        ((ItemCreation) modelChange).setTargetGUID(targetGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (modelChange.getElementType() == 90){                                                 //pridanie atributu
                    parentGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getParentGUID(), smallTeam, user);
                    if (!("").equals(parentGUID)){
                        log.info("getChangeForSynchronization");
                        log.info("ItemCreation of attribute in: " + parentGUID);
                        ((ItemCreation) modelChange).setParentGUID(parentGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (modelChange.getElementType() == 700){
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    diagramGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getDiagramGUID(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(diagramGUID)){
                        log.info("getChangeForSynchronization");
                        log.info("ItemCreation of diagram object: " + elementGUID + " " + diagramGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((ItemCreation) modelChange).setDiagramGUID(diagramGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                }

            } else if (modelChange instanceof PropertyChange) {
                if (modelChange.getElementDeleted() == 1 && (modelChange.getElementType() == 70 || modelChange.getElementType() == 700)) {     //vymazanie spojenia a DO
                    log.info("delete connector or diagram object");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    diagramGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(diagramGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("Delete connection: " + elementGUID + " in " + diagramGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(diagramGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (modelChange.getElementDeleted() == 1 && modelChange.getElementType() == 90) {                                   //vymazanie atributu
                    log.info("delete attribute");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("Delete attribute: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() >= 301 && ((PropertyChange) modelChange).getPropertyType() <= 302){     //zmena v spojeni
                    log.info("change target/source of connector");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    endElementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(endElementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange connection: " + elementGUID + " and " + endElementGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(endElementGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() >= 303 && ((PropertyChange) modelChange).getPropertyType() <= 304) {     //zmena v spojeni
                    log.info("change cardinality");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange connection 2: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() >= 401 && ((PropertyChange) modelChange).getPropertyType() <= 404) {            //presuvanie
                    log.info("movement");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    packageGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    //changesForSynchronization.getChangeIDs().remove(0);
                    //changesForSyncRepository.save(changesForSynchronization);
                    if (!("").equals(elementGUID) && !("").equals(packageGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange: " + elementGUID + " and " + packageGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(packageGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() == 405) {            //presuvanie v diagrame
                    log.info("movement in diagram");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    diagramGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getOldPropertyBody(), smallTeam, user);
                    //changesForSynchronization.getChangeIDs().remove(0);
                    //changesForSyncRepository.save(changesForSynchronization);
                    if (!("").equals(elementGUID) && !("").equals(diagramGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange: " + elementGUID + " and " + diagramGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setOldPropertyBody(diagramGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else {
                                                                                                                                          //ina lubovolna zmena
                    log.info("other change");
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange other change: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        //changesForSynchronization.getChangeIDs().remove(0);
                        //changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                }
            } else if (modelChange instanceof ScenarioChange){
                if (((ScenarioChange) modelChange).getStatus() == 1) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("ScenarioChange create: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((ScenarioChange) modelChange).getStatus() == 2 || ((ScenarioChange) modelChange).getStatus() == 0){
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    scenarioGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ScenarioChange) modelChange).getScenarioGUID(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(scenarioGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("ScenarioChange change or delete: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((ScenarioChange) modelChange).setScenarioGUID(scenarioGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                }
            } else if (modelChange instanceof StepChange){
                if (((StepChange) modelChange).getStatus() == 1) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    scenarioGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((StepChange) modelChange).getScenarioGUID(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(scenarioGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("StepChange create: element " + elementGUID + " scenario " + scenarioGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((StepChange) modelChange).setScenarioGUID(scenarioGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((StepChange) modelChange).getStatus() == 2 || ((StepChange) modelChange).getStatus() == 0){
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    scenarioGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((StepChange) modelChange).getScenarioGUID(), smallTeam, user);
                    stepGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((StepChange) modelChange).getStepGUID(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(scenarioGUID) && !("").equals(stepGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("StepChange change or delete: element " + elementGUID + " scenario " + scenarioGUID + " step " + stepGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((StepChange) modelChange).setScenarioGUID(scenarioGUID);
                        ((StepChange) modelChange).setStepGUID(stepGUID);
                       // changesForSynchronization.getChangeIDs().remove(0);
                       // changesForSyncRepository.save(changesForSynchronization);
                        return modelChange;
                    } else {
                        return null;
                    }
                }
            }
        }
        return null;
    }

    public ModelChange changeParentOrPackageGUID(ModelChange modelChange, SmallTeam smallTeam, User user){
        String elementGUID;
        if (!("0").equals(((ItemCreation) modelChange).getParentGUID())) {
            elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getParentGUID(), smallTeam, user);
            if (!("").equals(elementGUID)) {
                log.info("changeParentOrPackageGUID");
                log.info("ParentGUID: " + elementGUID);
                ((ItemCreation) modelChange).setParentGUID(elementGUID);
            } else {
                return null;
            }
        } else {
            elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getPackageGUID(),smallTeam, user);
            if (!("").equals(elementGUID)){
                log.info("changeParentOrPackageGUID");
                log.info("PackageGUID: " + elementGUID);
                ((ItemCreation) modelChange).setPackageGUID(elementGUID);
            } else {
                return null;
            }
        }
        return modelChange;
    }

    public ModelChange changeDiagramGUID(ModelChange modelChange, SmallTeam smallTeam, User user){
        if (modelChange == null){
            return null;
        }
        String elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getDiagramGUID(), smallTeam, user);
        if (!("").equals(elementGUID)) {
            log.info("changeDiagramGUID");
            log.info("DiagramGUID: " + elementGUID);
            ((ItemCreation) modelChange).setDiagramGUID(elementGUID);
        }
        return modelChange;
    }

    public String getElementGUIDForSynchronization(String userName, String elementGUID, SmallTeam smallTeam, User user){
        User authorOfChange = authorizationRepository.findByName(userName);
        CorrespondenceNodePart actualCorrNodePart;
        CorrespondenceNodePart correspondenceNodePart = corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(userName, elementGUID, authorOfChange.getModelGUID(),
                smallTeam.getId());
        if (correspondenceNodePart == null) {
            return "";
        }
        CorrespondenceNode correspondenceNode = correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(correspondenceNodePart.getId());
        if (correspondenceNode == null){
            return "";
        }
        for (String ID : correspondenceNode.getCorrespondenceNodePartIDs()){
            actualCorrNodePart = corrNodePartRepository.findById(ID);
            if (user.getName().equals(actualCorrNodePart.getUserName())) {
                log.info("getElementGUID");
                log.info(ID);
                log.info(actualCorrNodePart.getElementGUID());
                return actualCorrNodePart.getElementGUID();
            }
        }
        return "";
    }

    public void processChange(ModelChange modelChange, User user){
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(),
                smallTeam.getId());
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        boolean found = false;
        String packageGUID, elementGUID, parentGUID, diagramGUID;
        /*for (String id : changeIDs){
            ModelChange actualModelChange = modelChangeRepository.findById(id);
            if (modelChange instanceof ItemCreation && actualModelChange instanceof ItemCreation) {

                if (modelChange.getElementType() == actualModelChange.getElementType()
                        && ((ItemCreation) modelChange).getName().equals(((ItemCreation) actualModelChange).getName())) {           //potencionalna zhoda, rovnake zmeny

                    if (modelChange.getElementType() < 50 && modelChange.getElementType() != 3){                  //ide o elementy
                        diagramGUID = getElementGUIDForSynchronization(actualModelChange.getUserName(), ((ItemCreation) actualModelChange).getDiagramGUID(),
                                smallTeam, user);
                        if (diagramGUID.equals(((ItemCreation) modelChange).getDiagramGUID())) {
                            log.info("processChange");
                            log.info("ModelChange: naslo " + diagramGUID);
                            found = true;
                            changesForSynchronization.getChangeIDs().remove(id);
                            changesForSyncRepository.save(changesForSynchronization);
                            saveNewNode(smallTeam.getId(), user.getName(), modelChange.getItemGUID(), actualModelChange.getUserName(),
                                    actualModelChange.getItemGUID());
                            break;
                        }

                    } else if (("0").equals(((ItemCreation) modelChange).getParentGUID())) {
                        packageGUID = getElementGUIDForSynchronization(actualModelChange.getUserName(), ((ItemCreation) actualModelChange).getPackageGUID(),
                                smallTeam, user);
                        if (packageGUID.equals(((ItemCreation) modelChange).getPackageGUID())) {
                            log.info("processChange");
                            log.info("ModelChange: naslo " + packageGUID);
                            found = true;
                            changesForSynchronization.getChangeIDs().remove(id);
                            changesForSyncRepository.save(changesForSynchronization);
                            saveNewNode(smallTeam.getId(), user.getName(), modelChange.getItemGUID(), actualModelChange.getUserName(),
                                    actualModelChange.getItemGUID());
                            break;
                        }
                    } else {
                        parentGUID = getElementGUIDForSynchronization(actualModelChange.getUserName(), ((ItemCreation) actualModelChange).getParentGUID(),
                                smallTeam, user);
                        if (parentGUID.equals(((ItemCreation) modelChange).getParentGUID())) {
                            log.info("processChange");
                            log.info("ModelChange: naslo " + parentGUID);
                            found = true;
                            changesForSynchronization.getChangeIDs().remove(id);
                            changesForSyncRepository.save(changesForSynchronization);
                            saveNewNode(smallTeam.getId(), user.getName(), modelChange.getItemGUID(), actualModelChange.getUserName(),
                                    actualModelChange.getItemGUID());
                            break;
                        }
                    }
                }
            } else if (modelChange instanceof PropertyChange && actualModelChange instanceof PropertyChange) {

                if (modelChange.getElementDeleted() == 0 && actualModelChange.getElementDeleted() == 0) {           //uprava vlastnosti
                    if (modelChange.getElementType() == actualModelChange.getElementType()
                            && ((PropertyChange) modelChange).getPropertyBody().equals(((PropertyChange) actualModelChange).getPropertyBody())) {
                        elementGUID = getElementGUIDForSynchronization(actualModelChange.getUserName(), actualModelChange.getItemGUID(), smallTeam, user);

                        if (elementGUID.equals(modelChange.getItemGUID())) {
                            log.info("processChange");
                            log.info("PropertyChange: naslo " + elementGUID);
                            found = true;
                            changesForSynchronization.getChangeIDs().remove(id);
                            changesForSyncRepository.save(changesForSynchronization);
                            break;
                        }
                    }
                } else {                                                                                            //zmazanie
                    if (modelChange.getElementType() == actualModelChange.getElementType()) {
                        elementGUID = getElementGUIDForSynchronization(actualModelChange.getUserName(), actualModelChange.getItemGUID(), smallTeam, user);

                        if (elementGUID.equals(modelChange.getItemGUID())) {
                            log.info("processChange");
                            log.info("ElementDeleted: naslo " + elementGUID);
                            found = true;
                            changesForSynchronization.getChangeIDs().remove(id);
                            changesForSyncRepository.save(changesForSynchronization);
                            break;
                        }
                    }
                }
            }

        }*/
        if (!found){                                                    //pridanie zmeny ostatnym
            log.info("processChange");
            log.info("nenaslo");
            for (String id : smallTeam.getTeamMembersId()){
                User actualUser = authorizationRepository.findById(id);
                if (!actualUser.getName().equals(user.getName())){
                    changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(actualUser.getName(),
                            smallTeam.getId());
                    changesForSynchronization.getChangeIDs().add(modelChange.getId());
                    changesForSyncRepository.save(changesForSynchronization);
                }
            }
        }
    }

   /* public void deleteFromList(NewCorrespondenceNode newCorrespondenceNode){
        User user = authorizationRepository.findByName(newCorrespondenceNode.getFirstUsername());
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(),
                smallTeam.getId());
        changesForSynchronization.getChangeIDs().remove(0);
        changesForSyncRepository.save(changesForSynchronization);
    }*/

   /* public void deleteAfterWrongSynchronization(String token){
        User user = authorizationRepository.findByToken(token);
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(),
                smallTeam.getId());
        changesForSynchronization.getChangeIDs().remove(0);
        changesForSyncRepository.save(changesForSynchronization);
    }*/

    public void findNodeParametres(NewCorrespondenceNode newCorrespondenceNode){                                //ulozenie noveho uzla, ak neexistuje
        User firstUser = authorizationRepository.findByName(newCorrespondenceNode.getFirstUsername());
        User secondUser = authorizationRepository.findByName(newCorrespondenceNode.getSecondUsername());
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(firstUser.getId());

        /*ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(user.getName(),
                smallTeam.getId());

        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        ModelChange modelChange = modelChangeRepository.findById(changeIDs.get(0));

        changesForSynchronization.getChangeIDs().remove(0);
        changesForSyncRepository.save(changesForSynchronization);*/

        CorrespondenceNodePart correspondenceNodePart = corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(newCorrespondenceNode.getSecondUsername(),
                newCorrespondenceNode.getSecondItemGUID(), secondUser.getModelGUID(), smallTeam.getId());
        if (correspondenceNodePart == null){
            /*if (modelChange instanceof ScenarioChange){
                saveNewNode(smallTeam.getId(), newCorrespondenceNode.getFirstUsername(), newCorrespondenceNode.getFirstItemGUID(),
                        modelChange.getUserName(), ((ScenarioChange) modelChange).getScenarioGUID());
                log.info("findNodeParametres");
                log.info("ulozeny novy uzol");
                return;
            }*/
            saveNewNode(smallTeam.getId(), newCorrespondenceNode.getFirstUsername(), newCorrespondenceNode.getFirstItemGUID(),
                    newCorrespondenceNode.getSecondUsername(), newCorrespondenceNode.getSecondItemGUID());
            log.info("findNodeParametres");
            log.info("ulozeny novy uzol");
            return;
        }

        /*CorrespondenceNodePart correspondenceNodePart = corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(modelChange.getUserName(),
                modelChange.getItemGUID(), firstUser.getModelGUID(), smallTeam.getId());
        if (correspondenceNodePart == null) {
            if (modelChange instanceof ScenarioChange){
                saveNewNode(smallTeam.getId(), newCorrespondenceNode.getFirstUsername(), newCorrespondenceNode.getFirstItemGUID(),
                        modelChange.getUserName(), ((ScenarioChange) modelChange).getScenarioGUID());
                log.info("findNodeParametres");
                log.info("ulozeny novy uzol");
                return;
            }
            saveNewNode(smallTeam.getId(), newCorrespondenceNode.getFirstUsername(), newCorrespondenceNode.getFirstItemGUID(),
                    modelChange.getUserName(), modelChange.getItemGUID());
            log.info("findNodeParametres");
            log.info("ulozeny novy uzol");
            return;
        }*/
        CorrespondenceNode correspondenceNode = correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(correspondenceNodePart.getId());
        if (correspondenceNode != null) {
            log.info("uzol najdeny, pridanie novej casti uzla");
            CorrespondenceNodePart correspondenceNodePart1 = new CorrespondenceNodePart();
            correspondenceNodePart1.setElementGUID(newCorrespondenceNode.getFirstItemGUID());
            correspondenceNodePart1.setUserName(newCorrespondenceNode.getFirstUsername());
            correspondenceNodePart1.setSmallTeamID(smallTeam.getId());
            correspondenceNodePart1.setModelGUID(firstUser.getModelGUID());
            corrNodePartRepository.save(correspondenceNodePart1);

            correspondenceNode.getCorrespondenceNodePartIDs().add(correspondenceNodePart1.getId());
            correspondenceNodeRepository.save(correspondenceNode);
        }
    }

    public void checkOtherTeamMembers(User actualUser){
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(actualUser.getId());
        List<String> teamMembersIds = smallTeam.getTeamMembersId();
        for(String id : teamMembersIds){
            User user = authorizationService.getUserById(id);
            if (!id.equals(actualUser.getId())) {
                if (user.isAllModelData()) {
                    log.info("spojenie dat aktualneho usera s datami vyhladaneho " + user.getName());
                    addDataToCorrespondenceModel(smallTeam, actualUser, user);
                } else {
                    log.info("kolega " + user.getName() + " este neposlal data modelu");
                }
            }
        }
    }

    /*public boolean compareNodes(User firstUser, User secondUser, SmallTeam smallTeam, ModelChange firstUserData, ModelChange secondUserData) {
        CorrespondenceNodePart corrNodePart = corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(firstUserData.getUserName(),
                ((ItemCreation) secondUserData).getPackageGUID(), firstUser.getModelGUID(), smallTeam.getId());
        if (corrNodePart == null) {
            return false;
        }
        CorrespondenceNode correspondenceNode = correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(corrNodePart.getId());
        if (correspondenceNode == null) {
            return false;
        }

        CorrespondenceNodePart actualCorrNodePart;
        for (String ID : correspondenceNode.getCorrespondenceNodePartIDs()) {
            if (!ID.equals(corrNodePart.getId())) {
                actualCorrNodePart = corrNodePartRepository.findById(ID);
                if (actualCorrNodePart.getElementGUID().equals(((ItemCreation) secondUserData).getPackageGUID()) && actualCorrNodePart.getUserName().equals(secondUserData.getUserName())
                        && actualCorrNodePart.getModelGUID().equals(secondUser.getModelGUID())) {
                    return true;
                }
            }
        }
        return false;
    }*/

    public boolean compareNodes(User firstUser, User secondUser, SmallTeam smallTeam, String firstUserItemGUID, String secondUserItemGUID) {
        CorrespondenceNodePart corrNodePart = corrNodePartRepository.findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(firstUser.getName(),
                firstUserItemGUID, firstUser.getModelGUID(), smallTeam.getId());
        if (corrNodePart == null) {
            return false;
        }
        CorrespondenceNode correspondenceNode = correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(corrNodePart.getId());
        if (correspondenceNode == null) {
            return false;
        }

        CorrespondenceNodePart actualCorrNodePart;
        for (String ID : correspondenceNode.getCorrespondenceNodePartIDs()) {
            if (!ID.equals(corrNodePart.getId())) {
                actualCorrNodePart = corrNodePartRepository.findById(ID);
                if (actualCorrNodePart.getElementGUID().equals(secondUserItemGUID) && actualCorrNodePart.getUserName().equals(secondUser.getName())
                        && actualCorrNodePart.getModelGUID().equals(secondUser.getModelGUID())) {
                    return true;
                }
            }
        }
        return false;
    }
    public void addDataToCorrespondenceModel(SmallTeam smallTeam, User firstUser, User secondUser){
        boolean corrModelID = smallTeam.isCorrespondenceModel();

        List<ModelChange> firstUserData = modelChangeRepository.findByUserNameAndTimestamp(firstUser.getName(), "0");
        List<ModelChange> secondUserData = modelChangeRepository.findByUserNameAndTimestamp(secondUser.getName(), "0");

        List<String> changeID = new ArrayList<>();
        List<String> changeID2 = new ArrayList<>();

        log.info(String.valueOf(firstUserData.size()) + " " + String.valueOf(secondUserData.size()));

        if (!corrModelID){                                                               //neexistuje CM

            saveNewNode(smallTeam.getId(), firstUser.getName(), firstUser.getModelGUID(), secondUser.getName(), secondUser.getModelGUID());

            for (ModelChange dataOfFirst : firstUserData){

                if (dataOfFirst instanceof ItemCreation) {
                    log.info(((ItemCreation) dataOfFirst).getName());

                    for (ModelChange dataOfSecond : secondUserData) {
                        if (dataOfSecond instanceof ItemCreation) {
                            log.info(((ItemCreation) dataOfFirst).getName() + " " + ((ItemCreation) dataOfSecond).getName());

                            if (dataOfFirst.getElementType() == dataOfSecond.getElementType() && ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())) {

                                if (dataOfFirst.getElementType() == 3) {
                                    log.info("package:");
                                    log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID() + " " + dataOfFirst.getItemGUID());
                                    log.info(smallTeam.getId() + " " + dataOfFirst.getUserName() + " " + ((ItemCreation) dataOfFirst).getPackageGUID());

                                    if (compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())) {

                                        saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }
                                } else if ((dataOfFirst.getElementType() >= 50 && dataOfSecond.getElementType() <= 59)) {
                                    log.info("diagram:");
                                    //log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID() + " " + dataOfFirst.getItemGUID());
                                    //log.info(smallTeam.getId() + " " + dataOfFirst.getUserName() + " " + ((ItemCreation) dataOfFirst).getPackageGUID());

                                    if (compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())){
                                            //|| compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())) {

                                        saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }
                                } else if (dataOfFirst.getElementType() < 50) {
                                    log.info("element:");
                                    //log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID() + " " + dataOfFirst.getItemGUID());
                                    //log.info(smallTeam.getId() + " " + dataOfFirst.getUserName() + " " + ((ItemCreation) dataOfFirst).getPackageGUID());

                                    if (//compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getDiagramGUID(), ((ItemCreation) dataOfSecond).getDiagramGUID())&&
                                            //compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())
                                            compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())){
                                        saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }

                                    if (//compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getDiagramGUID(), ((ItemCreation) dataOfSecond).getDiagramGUID())&&
                                            compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())){
                                            //|| compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())){
                                        saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }
                                } else if ((dataOfFirst.getElementType() >= 70 && dataOfSecond.getElementType() <= 79)) {
                                    log.info("connector:");
                                    //log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID() + " " + dataOfFirst.getItemGUID());
                                    //log.info(smallTeam.getId() + " " + dataOfFirst.getUserName() + " " + ((ItemCreation) dataOfFirst).getPackageGUID());

                                    if (compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getSrcGUID(), ((ItemCreation) dataOfSecond).getSrcGUID())
                                            && compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getTargetGUID(), ((ItemCreation) dataOfSecond).getTargetGUID())){
                                        saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }
                                } else if ((dataOfFirst.getElementType() == 700)) {
                                    log.info("diagram object:");
                                    //log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID() + " " + dataOfFirst.getItemGUID());
                                    //log.info(smallTeam.getId() + " " + dataOfFirst.getUserName() + " " + ((ItemCreation) dataOfFirst).getPackageGUID());

                                    if (compareNodes(firstUser, secondUser, smallTeam, dataOfFirst.getItemGUID(), dataOfSecond.getItemGUID())
                                            && compareNodes(firstUser, secondUser, smallTeam, ((ItemCreation) dataOfFirst).getDiagramGUID(), ((ItemCreation) dataOfSecond).getDiagramGUID())){
                                        //saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                        changeID.add(dataOfFirst.getId());
                                        changeID2.add(dataOfSecond.getId());

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (changesForSyncRepository.findByUserNameAndSmallTeamId(secondUser.getName(), smallTeam.getId()) == null) {
                ChangesForSynchronization changesForSynchronization = new ChangesForSynchronization();
                changesForSynchronization.setChangeIDs(new ArrayList<>());
                changesForSynchronization.setUserName(secondUser.getName());
                changesForSynchronization.setSmallTeamId(smallTeam.getId());

                for (ModelChange dataOfFirst : firstUserData) {
                    if (!changeID.contains(dataOfFirst.getId())) {
                        changesForSynchronization.getChangeIDs().add(dataOfFirst.getId());
                    }
                }

                changesForSyncRepository.save(changesForSynchronization);
            } else {
                ChangesForSynchronization changesForSynchronization = changesForSyncRepository.findByUserNameAndSmallTeamId(secondUser.getName(), smallTeam.getId());

                changesForSynchronization.setChangeIDs(new ArrayList<>());

                for (ModelChange dataOfFirst : firstUserData) {
                    if (!changeID.contains(dataOfFirst.getId())) {
                        changesForSynchronization.getChangeIDs().add(dataOfFirst.getId());
                    }
                }

                changesForSyncRepository.save(changesForSynchronization);
            }

            if (changesForSyncRepository.findByUserNameAndSmallTeamId(firstUser.getName(), smallTeam.getId()) == null) {
                ChangesForSynchronization changesForSynchronization2 = new ChangesForSynchronization();
                changesForSynchronization2.setChangeIDs(new ArrayList<>());
                changesForSynchronization2.setUserName(firstUser.getName());
                changesForSynchronization2.setSmallTeamId(smallTeam.getId());

                for (ModelChange dataOfFirst : secondUserData) {
                    if (!changeID2.contains(dataOfFirst.getId())) {
                        changesForSynchronization2.getChangeIDs().add(dataOfFirst.getId());
                    }
                }

                changesForSyncRepository.save(changesForSynchronization2);
            } else {
                ChangesForSynchronization changesForSynchronization2 = changesForSyncRepository.findByUserNameAndSmallTeamId(firstUser.getName(), smallTeam.getId());

                changesForSynchronization2.setChangeIDs(new ArrayList<>());

                for (ModelChange dataOfFirst : secondUserData) {
                    if (!changeID2.contains(dataOfFirst.getId())) {
                        changesForSynchronization2.getChangeIDs().add(dataOfFirst.getId());
                    }
                }
                changesForSyncRepository.save(changesForSynchronization2);
            }

            authorizationRepository.save(firstUser);

            smallTeam.setCorrespondenceModel(true);
            smallTeamRepository.save(smallTeam);

            int pocet = 0;

            for (String id : smallTeam.getTeamMembersId()){
                if (authorizationService.getUserById(id).isAllModelData()){
                    pocet++;
                }
            }
            if (pocet == smallTeam.getTeamMembersId().size()){
                log.info("da sa sync, nastavime syncAllowed v time");           //treba vymazat creaty
                smallTeam.setSynchronizationAllowed(true);
                smallTeamRepository.save(smallTeam);
            } else {
                log.info("neda sa sync");
            }

        } else {
            log.info("existuje corr model");
        }
    }

    public void saveNewNode(String smallTeamId, String firstUserName, String firstItemGUID, String secondUserName, String secondItemGUID){
        User firstUser = authorizationRepository.findByName(firstUserName);
        User secondUser = authorizationRepository.findByName(secondUserName);

        log.info("ulozenie noveho uzla");
        log.info(firstUserName + " " + firstItemGUID + " " + secondUserName + " " + secondItemGUID);

        CorrespondenceNodePart correspondenceNodePart1 = new CorrespondenceNodePart();
        correspondenceNodePart1.setSmallTeamID(smallTeamId);
        correspondenceNodePart1.setElementGUID(firstItemGUID);
        correspondenceNodePart1.setUserName(firstUserName);
        correspondenceNodePart1.setModelGUID(firstUser.getModelGUID());
        corrNodePartRepository.save(correspondenceNodePart1);

        CorrespondenceNodePart correspondenceNodePart2 = new CorrespondenceNodePart();
        correspondenceNodePart2.setSmallTeamID(smallTeamId);
        correspondenceNodePart2.setElementGUID(secondItemGUID);
        correspondenceNodePart2.setUserName(secondUserName);
        correspondenceNodePart2.setModelGUID(secondUser.getModelGUID());
        corrNodePartRepository.save(correspondenceNodePart2);

        CorrespondenceNode newCorrNode = new CorrespondenceNode();
        newCorrNode.setCorrespondenceNodePartIDs(new ArrayList<>());
        newCorrNode.getCorrespondenceNodePartIDs().add(correspondenceNodePart1.getId());
        newCorrNode.getCorrespondenceNodePartIDs().add(correspondenceNodePart2.getId());
        correspondenceNodeRepository.save(newCorrNode);
    }
}

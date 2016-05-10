package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;
import java.util.List;

@RestController
@RequestMapping(value = "/synchronization", produces = MediaType.APPLICATION_JSON_VALUE)
public class SynchronizationController {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private ChangesForSynchronizationService changesForSyncService;

    @Inject
    private CorrNodePartService corrNodePartService;

    @Inject
    private CorrespondenceNodeService corrNodeService;

    private final Logger log = LoggerFactory.getLogger(SynchronizationController.class);

    /**
     * method checks which fase is synchronization process in
     * @param modelInformation information about model (token of user and GUID of model)
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status BAD_REQUEST - user is not member of any small team
     *          ResponseEntity with status METHOD_NOT_ALLOWED - user wants to synchronize wrong model
     *          ResponseEntity with status FORBIDDEN - synchronization is not yet allowed
     *          ResponseEntity with status NOT_FOUND - user has not sent data about model
     *          ResponseEntity with status NOT_ACCEPTABLE - small team does not have all members
     */
    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> checkSynchronization(@RequestBody ModelInformation modelInformation) {
        User currentUser = authorizationService.getUser(modelInformation.getToken());
        if (currentUser == null){
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }
        SmallTeam smallTeam = smallTeamService.getByUserId(currentUser.getId());
        if(smallTeam == null){
            log.info(currentUser.getName() + " is not joined to any team");
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        } else {
            if (smallTeam.getTeamMembersId().size() > 1) {
                log.info(String.valueOf(currentUser.isAllModelData()));
                if (currentUser.isAllModelData()) {
                    if (smallTeam.isSynchronizationAllowed()) {
                        if (currentUser.getModelGUID().equals(modelInformation.getModelGUID())) {
                            log.info("Synchronization starts: " + smallTeam.getId() + " user " + currentUser.getName());
                            return new ResponseEntity<>(HttpStatus.OK);
                        } else {
                            log.info("Synchronization from the wrong model " + smallTeam.getId() + " user " + currentUser.getName());
                            return new ResponseEntity<>(HttpStatus.METHOD_NOT_ALLOWED);
                        }
                    } else {
                        log.info("Synchronization is not running "  + smallTeam.getId() + " user " + currentUser.getName());
                        return new ResponseEntity<>(HttpStatus.FORBIDDEN);
                    }
                } else {
                    log.info("User " + currentUser.getName() + " has not sent data of model");
                    return new ResponseEntity<>(HttpStatus.NOT_FOUND);
                }
            } else {
                log.info("Small team " + smallTeam.getId() + " not all members are in team");
                return new ResponseEntity<>(HttpStatus.NOT_ACCEPTABLE);
            }
        }
    }

    /**
     * method send change to client that has to be made in synchronization process
     * @param token token of user that wants to synchronize
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status BAD_REQUEST - user is not member of any small team
     *          ResponseEntity with status NOT_FOUND - list of changes for synchronization of this user was not found
     */
    @RequestMapping(value = "/changes", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<ModelChange> sendChangeForSynchronization(@RequestBody String token) {
        ModelChange modelChange = null;
        User user = authorizationService.getUserByToken(token);
        if (user == null){
            return new ResponseEntity<>(modelChange, HttpStatus.UNAUTHORIZED);
        }
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        if (smallTeam == null){
            return new ResponseEntity<>(modelChange, HttpStatus.BAD_REQUEST);
        }
        ChangesForSynchronization changesForSynchronization =
                changesForSyncService.findChangesForSynchronization(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null){
            return new ResponseEntity<>(modelChange, HttpStatus.NOT_FOUND);
        }
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        if (changeIDs != null) {
            if (changeIDs.size() > 0) {
                modelChange = getChangeForSynchronization(token);
                return new ResponseEntity<>(modelChange, HttpStatus.OK);
            } else {
                PropertyChange propertyChange = new PropertyChange();
                propertyChange.setTimestamp("-1");
                log.info("Synchronization finished");
                return new ResponseEntity<>(propertyChange, HttpStatus.OK);
            }
        } else {
            return new ResponseEntity<>(modelChange, HttpStatus.NOT_FOUND);
        }
    }

    /**
     * method finds number of changes that have to be made in synchronization
     * @param token token of user that wants to synchronize
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status BAD_REQUEST - user is not member of any small team
     *          ResponseEntity with status NOT_FOUND - list of changes for synchronization of this user was not found
     */
    @RequestMapping(value = "/getNumber", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> getNumberOfChangesForSynchronization(@RequestBody String token) {
        User user = authorizationService.getUserByToken(token);
        if (user == null){
            return new ResponseEntity<>("", HttpStatus.UNAUTHORIZED);
        }
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        if (smallTeam == null){
            return new ResponseEntity<>("", HttpStatus.BAD_REQUEST);
        }
        ChangesForSynchronization changesForSynchronization =
                changesForSyncService.findChangesForSynchronization(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null){
            return new ResponseEntity<>("", HttpStatus.NOT_FOUND);
        }
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        if (changeIDs != null) {
            log.info("number of changes to be synchronized for " + user.getName() + ": " + String.valueOf(changeIDs.size()));
            return new ResponseEntity<>(String.valueOf(changeIDs.size()), HttpStatus.OK);
        } else {
            return new ResponseEntity<>(String.valueOf(0), HttpStatus.OK);
        }
    }

    /**
     * method find change that has to be made in synchronization
     * @param token token of user that wants to synchronize
     * @return model change with modified GUIDs
     */
    public ModelChange getChangeForSynchronization(String token){
        String elementGUID, srcGUID, targetGUID, diagramGUID, endElementGUID, packageGUID, parentGUID, scenarioGUID;
        User user = authorizationService.getUserByToken(token);
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        ChangesForSynchronization changesForSynchronization =
                changesForSyncService.findChangesForSynchronization(user.getName(), smallTeam.getId());
        ModelChange modelChange;
        List<String> changeIDs = changesForSynchronization.getChangeIDs();
        if (changeIDs.size() > 0){
            String changeID = changeIDs.get(0);
            modelChange = modelChangeService.getChangeById(changeID);
            changesForSynchronization.getChangeIDs().remove(0);
            changesForSyncService.updateChangesForSynchronization(changesForSynchronization);
            if (modelChange instanceof ItemCreation) {

                if (modelChange.getElementType() == 3) {
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() >= 50 && modelChange.getElementType() < 70){
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() >= 0 && modelChange.getElementType() < 50){
                    return changeParentOrPackageGUID(modelChange, smallTeam, user);

                } else if (modelChange.getElementType() >= 70 && modelChange.getElementType() <= 79){
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
                } else if (modelChange.getElementType() == 90){
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
                if (modelChange.getElementDeleted() == 1 && modelChange.getElementType() == 700) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    diagramGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(diagramGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("Delete diagram object: " + elementGUID + " in " + diagramGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(diagramGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() >= 301 && ((PropertyChange) modelChange).getPropertyType() <= 302){
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    endElementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(endElementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange change target/source of connector: " + elementGUID + " and " + endElementGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(endElementGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() >= 401 && ((PropertyChange) modelChange).getPropertyType() <= 404) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    packageGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(packageGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange movement: " + elementGUID + " and " + packageGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setPropertyBody(packageGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((PropertyChange) modelChange).getPropertyType() == 405) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    diagramGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((PropertyChange) modelChange).getOldPropertyBody(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(diagramGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange movement in diagram: " + elementGUID + " and " + diagramGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((PropertyChange) modelChange).setOldPropertyBody(diagramGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("PropertyChange other change: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                }
            } else if (modelChange instanceof StepChange){
                if (((StepChange) modelChange).getStatus() == 1) {
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    if (!("").equals(elementGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("StepChange create: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                } else if (((StepChange) modelChange).getStatus() == 2 || ((StepChange) modelChange).getStatus() == 0){
                    elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), modelChange.getItemGUID(), smallTeam, user);
                    scenarioGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((StepChange) modelChange).getScenarioGUID(), smallTeam, user);
                    if (!("").equals(elementGUID) && !("").equals(scenarioGUID)) {
                        log.info("getChangeForSynchronization");
                        log.info("StepChange change or delete: " + elementGUID);
                        modelChange.setItemGUID(elementGUID);
                        ((StepChange) modelChange).setScenarioGUID(scenarioGUID);
                        return modelChange;
                    } else {
                        return null;
                    }
                }
            }
        }
        return null;
    }

    /**
     * method changes parent or package GUID of model change
     * @param modelChange model change to be synchronized
     * @param smallTeam small team where user is member
     * @param user user that wants to synchronize
     * @return modified model change
     */
    public ModelChange changeParentOrPackageGUID(ModelChange modelChange, SmallTeam smallTeam, User user){
        String elementGUID;
        elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getPackageGUID(),smallTeam, user);
        if (!("").equals(elementGUID)){
            log.info("changeParentOrPackageGUID");
            log.info("PackageGUID: " + elementGUID);
            ((ItemCreation) modelChange).setPackageGUID(elementGUID);
        } else {
            return null;
        }
        if (!("0").equals(((ItemCreation) modelChange).getParentGUID())) {
            elementGUID = getElementGUIDForSynchronization(modelChange.getUserName(), ((ItemCreation) modelChange).getParentGUID(), smallTeam, user);
            if (!("").equals(elementGUID)) {
                log.info("changeParentOrPackageGUID");
                log.info("ParentGUID: " + elementGUID);
                ((ItemCreation) modelChange).setParentGUID(elementGUID);
            } else {
                return null;
            }
        }
        return modelChange;
    }

    /**
     * method finds item GUID in model of user
     * @param userName name of user that wants to synchronize
     * @param elementGUID item GUID
     * @param smallTeam small team where user is member
     * @param user user that wants to synchronize
     * @return item GUID in model of user or empty String
     */
    public String getElementGUIDForSynchronization(String userName, String elementGUID, SmallTeam smallTeam, User user){
        User authorOfChange = authorizationService.getUserByName(userName);
        CorrespondenceNodePart currentCorrNodePart;
        CorrespondenceNodePart correspondenceNodePart = corrNodePartService.getCorrNodePart(userName, elementGUID,
                authorOfChange.getModelGUID(), smallTeam.getId());
        if (correspondenceNodePart == null) {
            return "";
        }
        CorrespondenceNode correspondenceNode = corrNodeService.getCorrNodeByCorrNodePartId(correspondenceNodePart.getId());
        if (correspondenceNode == null){
            return "";
        }
        for (String ID : correspondenceNode.getCorrespondenceNodePartIDs()){
            currentCorrNodePart = corrNodePartService.getCorrNodePartById(ID);
            if (user.getName().equals(currentCorrNodePart.getUserName())) {
                log.info("getElementGUID");
                log.info(currentCorrNodePart.getElementGUID());
                return currentCorrNodePart.getElementGUID();
            }
        }
        return "";
    }
}

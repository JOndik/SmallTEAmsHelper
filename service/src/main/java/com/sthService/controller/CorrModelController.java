package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import javax.inject.Inject;
import java.util.ArrayList;
import java.util.List;

@RestController
@RequestMapping(value = "/corrModel", produces = MediaType.APPLICATION_JSON_VALUE)
public class CorrModelController {

    private final Logger log = LoggerFactory.getLogger(CorrModelController.class);

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private CorrNodePartService corrNodePartService;

    @Inject
    private CorrespondenceNodeService correspondenceNodeService;

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private ChangesForSynchronizationService changesForSynchronizationService;

    @RequestMapping(value = "/createNode", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> postNewCorrespondenceNode(@RequestBody NewCorrespondenceNode newCorrespondenceNode) {
        String corrNodePartId;
        if (!("").equals(newCorrespondenceNode.getFirstItemGUID())) {
            log.info(newCorrespondenceNode.getSecondUsername() + " " + newCorrespondenceNode.getSecondItemGUID());
            User firstUser = authorizationService.getUserByName(newCorrespondenceNode.getFirstUsername());
            User secondUser = authorizationService.getUserByName(newCorrespondenceNode.getSecondUsername());
            SmallTeam smallTeam = smallTeamService.getByUserId(firstUser.getId());

            CorrespondenceNodePart correspondenceNodePart = corrNodePartService.getCorrNodePart(newCorrespondenceNode.getSecondUsername(),
                    newCorrespondenceNode.getSecondItemGUID(), secondUser.getModelGUID(), smallTeam.getId());
            if (correspondenceNodePart == null){
                saveNewNode(smallTeam.getId(), newCorrespondenceNode.getFirstUsername(), newCorrespondenceNode.getFirstItemGUID(),
                        newCorrespondenceNode.getSecondUsername(), newCorrespondenceNode.getSecondItemGUID());
                return new ResponseEntity<>("", HttpStatus.OK);
            }

            CorrespondenceNode correspondenceNode = correspondenceNodeService.getCorrNodeByCorrNodePartId(correspondenceNodePart.getId());
            if (correspondenceNode != null) {
                log.info("Node found, addition of new node part");

                corrNodePartId = corrNodePartService.createCorrNodePart(newCorrespondenceNode.getFirstUsername(),
                        newCorrespondenceNode.getFirstItemGUID(), firstUser.getModelGUID(), smallTeam.getId());
                correspondenceNode.getCorrespondenceNodePartIDs().add(corrNodePartId);
                correspondenceNodeService.updateCorrespondenceNode(correspondenceNode);
            }
        }
        return new ResponseEntity<>("", HttpStatus.OK);
    }

    public boolean compareNodes(User firstUser, User secondUser, SmallTeam smallTeam, String firstUserItemGUID, String secondUserItemGUID) {
        CorrespondenceNodePart corrNodePart = corrNodePartService.getCorrNodePart(firstUser.getName(),
                firstUserItemGUID, firstUser.getModelGUID(), smallTeam.getId());
        if (corrNodePart == null) {
            return false;
        }
        CorrespondenceNode correspondenceNode = correspondenceNodeService.getCorrNodeByCorrNodePartId(corrNodePart.getId());
        if (correspondenceNode == null) {
            return false;
        }

        CorrespondenceNodePart currentCorrNodePart;
        for (String ID : correspondenceNode.getCorrespondenceNodePartIDs()) {
            if (!ID.equals(corrNodePart.getId())) {
                currentCorrNodePart = corrNodePartService.getCorrNodePartById(ID);
                if (currentCorrNodePart.getElementGUID().equals(secondUserItemGUID) && currentCorrNodePart.getUserName().equals(secondUser.getName())
                        && currentCorrNodePart.getModelGUID().equals(secondUser.getModelGUID())) {
                    return true;
                }
            }
        }
        return false;
    }

    public void checkOtherTeamMembers(User currentUser){
        SmallTeam smallTeam = smallTeamService.getByUserId(currentUser.getId());
        List<String> teamMembersIds = smallTeam.getTeamMembersId();
        for(String id : teamMembersIds){
            User user = authorizationService.getUserById(id);
            if (!id.equals(currentUser.getId())) {
                if (user.isAllModelData()) {
                    log.info("connection of current user data with found user " + user.getName());
                    addDataToCorrespondenceModel(smallTeam, currentUser, user);
                } else {
                    log.info("team member " + user.getName() + " has not sent data of model");
                }
            }
        }
    }

    public void addDataToCorrespondenceModel(SmallTeam smallTeam, User currentUser, User user){
        boolean corrModelID = smallTeam.isCorrespondenceModel();
        int index;
        String constraintName, secondConstraintName;
        ModelChange modelChange;

        List<ModelChange> currentUserChanges = modelChangeService.getChangesByUsernameAndTimestamp(currentUser.getName(), currentUser.getModelGUID());
        List<ModelChange> userChanges = modelChangeService.getChangesByUsernameAndTimestamp(user.getName(), user.getModelGUID());

        List<String> changeIDsOfCurrentUser = new ArrayList<>();
        List<String> changeIDsOfUser = new ArrayList<>();

        ChangesForSynchronization changesForSynchronizationOfCurrentUser =
                changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
        for (String id : changesForSynchronizationOfCurrentUser.getChangeIDs()){
            modelChange = modelChangeService.getChangeById(id);
            userChanges.add(modelChange);
        }
        changesForSynchronizationOfCurrentUser.setChangeIDs(new ArrayList<>());
        changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronizationOfCurrentUser);

        for (ModelChange dataOfFirst : currentUserChanges) {
            if (dataOfFirst.getElementType() != -1) {
                changeIDsOfCurrentUser.add(dataOfFirst.getId());
            }
        }

        for (ModelChange dataOfSecond : userChanges){
            if (dataOfSecond.getElementType() != -1) {
                changeIDsOfUser.add(dataOfSecond.getId());
            }
        }

        log.info("AddDataToCorrModel: " + currentUser.getName() + " " + String.valueOf(currentUserChanges.size()) + " " + user.getName() + " " + String.valueOf(userChanges.size()));

        if (!corrModelID){                                                               //neexistuje CM

            saveNewNode(smallTeam.getId(), currentUser.getName(), currentUser.getModelGUID(), user.getName(), user.getModelGUID());

            // packages
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() == 3){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfSecond.getElementType() == 3 && ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())) {
                                log.info("new package node: " + currentUser.getName() + " " + ((ItemCreation)dataOfFirst).getName() + " " + user.getName() + " " + ((ItemCreation)dataOfSecond).getName());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // elements
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() < 50 && dataOfFirst.getElementType() != 3){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfFirst.getElementType() == dataOfSecond.getElementType() &&
                                ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())) {
                                log.info("new element node: "  + currentUser.getName() + " " + ((ItemCreation)dataOfFirst).getName() + " "  + user.getName() + " " + ((ItemCreation)dataOfSecond).getName());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())) {
                                log.info("new element node: "  + currentUser.getName() + " " + ((ItemCreation)dataOfFirst).getName() + " "  + user.getName() + " " + ((ItemCreation)dataOfSecond).getName());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // diagrams
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() >= 50 && dataOfFirst.getElementType() < 70){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfFirst.getElementType() == dataOfSecond.getElementType() &&
                                ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())) {
                                log.info("new diagram node: "  + currentUser.getName() + " " + ((ItemCreation)dataOfFirst).getName() + " "  + user.getName() + " " + ((ItemCreation)dataOfSecond).getName());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getPackageGUID(), ((ItemCreation) dataOfSecond).getPackageGUID())) {
                                log.info("new diagram node: "  + currentUser.getName() + " " + ((ItemCreation)dataOfFirst).getName() + " "  + user.getName() + " " + ((ItemCreation)dataOfSecond).getName());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // connectors
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() >= 70 && dataOfFirst.getElementType() <= 79){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfFirst.getElementType() == dataOfSecond.getElementType()){
                            //   ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getSrcGUID(), ((ItemCreation) dataOfSecond).getSrcGUID())
                                    && compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getTargetGUID(), ((ItemCreation) dataOfSecond).getTargetGUID())){
                                log.info("new connector node: "  + currentUser.getName() + " " + dataOfFirst.getItemGUID() + " "  + user.getName() + " " + dataOfSecond.getItemGUID());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // diagram objects
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() == 700){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfSecond.getElementType() == 700){ //&& ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, dataOfFirst.getItemGUID(), dataOfSecond.getItemGUID())
                                    && compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getDiagramGUID(), ((ItemCreation) dataOfSecond).getDiagramGUID())) {
                                log.info("same diagram objects: "  + currentUser.getName() + " " + dataOfFirst.getItemGUID() + " "  + user.getName() + " " + dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // attributes
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof ItemCreation && dataOfFirst.getElementType() == 90){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof ItemCreation && dataOfSecond.getElementType() == 90 && ((ItemCreation) dataOfFirst).getName().equals(((ItemCreation) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, ((ItemCreation) dataOfFirst).getParentGUID(), ((ItemCreation) dataOfSecond).getParentGUID())){
                                log.info("new attribute node: " + currentUser.getName() + " " + dataOfFirst.getItemGUID() + " " + user.getName() + " " + dataOfSecond.getItemGUID());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), dataOfFirst.getItemGUID(), dataOfSecond.getUserName(), dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // scenarios
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof StepChange && ((StepChange)dataOfFirst).getStatus() == 1){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof StepChange && ((StepChange)dataOfSecond).getStatus() == 1 && ((StepChange) dataOfFirst).getName().equals(((StepChange) dataOfSecond).getName())){
                            if (compareNodes(currentUser, user, smallTeam, dataOfFirst.getItemGUID(), dataOfSecond.getItemGUID())){
                                log.info("new scenario node: "  + currentUser.getName() + " " + ((StepChange)dataOfFirst).getScenarioGUID() + " " + user.getName() + " " + ((StepChange)dataOfSecond).getScenarioGUID());
                                saveNewNode(smallTeam.getId(), dataOfFirst.getUserName(), ((StepChange)dataOfFirst).getScenarioGUID(), dataOfSecond.getUserName(), ((StepChange)dataOfSecond).getScenarioGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // constraints
            for (ModelChange dataOfFirst : currentUserChanges){
                if (dataOfFirst instanceof PropertyChange && ((PropertyChange)dataOfFirst).getPropertyType() == 10){
                    for (ModelChange dataOfSecond : userChanges){
                        if (dataOfSecond instanceof PropertyChange && ((PropertyChange)dataOfSecond).getPropertyType() == ((PropertyChange)dataOfFirst).getPropertyType()){
                            index = (((PropertyChange)dataOfFirst).getPropertyBody()).indexOf(",notes:=");
                            constraintName = ((PropertyChange)dataOfFirst).getPropertyBody().substring(0, index-1);
                            index = (((PropertyChange)dataOfSecond).getPropertyBody()).indexOf(",notes:=");
                            secondConstraintName = ((PropertyChange)dataOfSecond).getPropertyBody().substring(0, index-1);
                            if (constraintName.equals(secondConstraintName)){
                                log.info("same constraint: "  + currentUser.getName() + " " + dataOfFirst.getItemGUID() + " "  + user.getName() + " " + dataOfSecond.getItemGUID());
                                changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                                changeIDsOfUser.remove(dataOfSecond.getId());
                                break;
                            }
                        }
                    }
                }
            }

            // PropertyChange
            for (ModelChange dataOfFirst : currentUserChanges) {
                if (dataOfFirst instanceof PropertyChange && ((PropertyChange)dataOfFirst).getPropertyType() != 10) {
                    for (ModelChange dataOfSecond : userChanges) {
                        if (dataOfSecond instanceof PropertyChange && ((PropertyChange)dataOfSecond).getPropertyType() == ((PropertyChange)dataOfFirst).getPropertyType() &&
                                compareNodes(currentUser, user, smallTeam, dataOfFirst.getItemGUID(), dataOfSecond.getItemGUID())) {
                            log.info("same property: " + ((PropertyChange)dataOfFirst).getPropertyType() + " "  + currentUser.getName() + " " + dataOfFirst.getItemGUID() + " "  + user.getName() + " " + dataOfSecond.getItemGUID());
                            changeIDsOfCurrentUser.remove(dataOfFirst.getId());
                            changeIDsOfUser.remove(dataOfSecond.getId());
                        }
                    }
                }
            }

            changesForSynchronizationOfCurrentUser =
                    changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
            changesForSynchronizationOfCurrentUser.setChangeIDs(changeIDsOfUser);
            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronizationOfCurrentUser);

            ChangesForSynchronization changesForSynchronizationOfUser =
                    changesForSynchronizationService.findChangesForSynchronization(user.getName(), smallTeam.getId());
            changesForSynchronizationOfUser.setChangeIDs(changeIDsOfCurrentUser);
            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronizationOfUser);

            smallTeam.setCorrespondenceModel(true);
            smallTeamService.updateTeam(smallTeam);

            int pocet = 0;

            for (String id : smallTeam.getTeamMembersId()){
                if (authorizationService.getUserById(id).isAllModelData()){
                    pocet++;
                }
            }
            if (pocet == smallTeam.getTeamMembersId().size()){
                log.info("synchronization is allowed in " + smallTeam.getId());
                smallTeam.setSynchronizationAllowed(true);
                smallTeamService.updateTeam(smallTeam);
            } else {
                log.info("synchronization is not allowed in " + smallTeam.getId());
            }

        } else {
            log.info("correspondence model of " + smallTeam.getId() + " exists");
        }
    }

    public void saveNewNode(String smallTeamId, String firstUserName, String firstItemGUID, String secondUserName, String secondItemGUID){
        User firstUser = authorizationService.getUserByName(firstUserName);
        User secondUser = authorizationService.getUserByName(secondUserName);
        String firstId, secondId;

        log.info("Saving new node: " + firstUserName + " " + firstItemGUID + " " + secondUserName + " " + secondItemGUID);

        firstId = corrNodePartService.createCorrNodePart(firstUserName, firstItemGUID, firstUser.getModelGUID(), smallTeamId);
        secondId = corrNodePartService.createCorrNodePart(secondUserName, secondItemGUID, secondUser.getModelGUID(), smallTeamId);
        correspondenceNodeService.createCorrespondenceNode(firstId, secondId);

        /*CorrespondenceNodePart correspondenceNodePart1 = new CorrespondenceNodePart();
        correspondenceNodePart1.setSmallTeamID(smallTeamId);
        correspondenceNodePart1.setElementGUID(firstItemGUID);
        correspondenceNodePart1.setUserName(firstUserName);
        correspondenceNodePart1.setModelGUID(firstUser.getModelGUID());
        corrNodePartRepository.save(correspondenceNodePart1);*/

        /*CorrespondenceNodePart correspondenceNodePart2 = new CorrespondenceNodePart();
        correspondenceNodePart2.setSmallTeamID(smallTeamId);
        correspondenceNodePart2.setElementGUID(secondItemGUID);
        correspondenceNodePart2.setUserName(secondUserName);
        correspondenceNodePart2.setModelGUID(secondUser.getModelGUID());
        corrNodePartRepository.save(correspondenceNodePart2);*/

       /* CorrespondenceNode newCorrNode = new CorrespondenceNode();
        newCorrNode.setCorrespondenceNodePartIDs(new ArrayList<>());
        newCorrNode.getCorrespondenceNodePartIDs().add(correspondenceNodePart1.getId());
        newCorrNode.getCorrespondenceNodePartIDs().add(correspondenceNodePart2.getId());
        correspondenceNodeRepository.save(newCorrNode);*/
    }
}

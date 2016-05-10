package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.MediaType;
import org.springframework.stereotype.Component;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import javax.inject.Inject;
import java.util.Iterator;
import java.util.List;

@Component
public class ProcessChangesController {

    private final Logger log = LoggerFactory.getLogger(ProcessChangesController.class);

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private ChangesForSynchronizationService changesForSynchronizationService;

    @Inject
    private CorrNodePartService corrNodePartService;

    @Inject
    private CorrespondenceNodeService corrNodeService;

    /**
     * method processes change
     * @param modelChange new model change
     * @param user user that made change
     */
    public void processChange(ModelChange modelChange, User user){
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        ChangesForSynchronization changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null){
            return;
        }
        List<String> userChangeIDs = changesForSynchronization.getChangeIDs();

        if (modelChange.getElementType() == -1){
            return;
        }

        if (modelChange instanceof ItemCreation || (modelChange instanceof StepChange && ((StepChange) modelChange).getStatus() == 1)){
            for (String id : smallTeam.getTeamMembersId()){
                User currentUser = authorizationService.getUserById(id);
                if (!currentUser.getName().equals(user.getName())){
                    changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                    changesForSynchronization.getChangeIDs().add(modelChange.getId());
                    changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
                }
            }
        }

        if (modelChange instanceof PropertyChange && modelChange.getElementDeleted() == 0){
            processPropertyChange((PropertyChange)modelChange, userChangeIDs, smallTeam, user);
        }

        else if (modelChange instanceof PropertyChange && modelChange.getElementDeleted() == 1) {
            processDelete((PropertyChange)modelChange, userChangeIDs, smallTeam, user, changesForSynchronization);
        }

        else if (modelChange instanceof StepChange && ((StepChange) modelChange).getStatus() == 2){
            processScenarioChange((StepChange)modelChange, smallTeam, user);
        }

        else if (modelChange instanceof StepChange && ((StepChange) modelChange).getStatus() == 0){
            processScenarioDelete((StepChange)modelChange, smallTeam, user);
        }
    }

    /**
     * method processes property change of model item
     * @param modelChange model change
     * @param userChangeIDs ID of synchronization changes of user
     * @param smallTeam small team where user is member
     * @param user user that made change
     */
    public void processPropertyChange(PropertyChange modelChange, List<String> userChangeIDs, SmallTeam smallTeam, User user){
        User currentUser;
        boolean found = false;
        String elementGUID;
        ChangesForSynchronization changesForSynchronization;
        List<String> currentUserChangeIDs;

        for (String id : smallTeam.getTeamMembersId()) {                                        //zmena mena pri create, kontrola listov kolegov
            currentUser = authorizationService.getUserById(id);

            if (!currentUser.getName().equals(user.getName())) {
                changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                currentUserChangeIDs = changesForSynchronization.getChangeIDs();

                for (Iterator<String> iterator = currentUserChangeIDs.iterator(); iterator.hasNext(); ) {
                    ModelChange currentModelChange = modelChangeService.getChangeById(iterator.next());

                    if (modelChange.getPropertyType() == 0 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() != 700) {

                        if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {
                            log.info("processPropertyChange: change of creation name");
                            ((ItemCreation) currentModelChange).setName(modelChange.getPropertyBody());
                            modelChangeService.updateModelChange(currentModelChange);
                            return;
                        }

                    } else if (modelChange.getPropertyType() == 1 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() != 700) {

                        if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {
                            log.info("processPropertyChange: change of creation author");
                            ((ItemCreation) currentModelChange).setAuthor(modelChange.getPropertyBody());
                            modelChangeService.updateModelChange(currentModelChange);
                            return;
                        }

                    } else if (modelChange.getPropertyType() == 405 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() == 700){

                        if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {
                            log.info("processPropertyChange: change of coordinates of diagram object creation");
                            ((ItemCreation) currentModelChange).setCoordinates(modelChange.getPropertyBody());
                            modelChangeService.updateModelChange(currentModelChange);
                            return;
                        }

                    } else if (modelChange.getPropertyType() == 301 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() >= 70
                            && currentModelChange.getElementType() <= 79){

                        if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {
                            log.info("processPropertyChange: change of target element of connector creation");
                            ((ItemCreation) currentModelChange).setTargetGUID(modelChange.getPropertyBody());
                            modelChangeService.updateModelChange(currentModelChange);
                            return;
                        }

                    } else if (modelChange.getPropertyType() == 302 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() >= 70
                            && currentModelChange.getElementType() <= 79){

                        if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {
                            log.info("processPropertyChange: change of source element of connector creation");
                            ((ItemCreation) currentModelChange).setSrcGUID(modelChange.getPropertyBody());
                            modelChangeService.updateModelChange(currentModelChange);
                            return;
                        }

                    }
                }
            }
        }

        if (modelChange.getElementDeleted() == 0){
            for (Iterator<String> iterator = userChangeIDs.iterator(); iterator.hasNext();){
                ModelChange currentModelChange = modelChangeService.getChangeById(iterator.next());
                if (currentModelChange.getElementDeleted() == 1){
                    elementGUID = getElementGUIDForSynchronization(currentModelChange.getUserName(), currentModelChange.getItemGUID(), smallTeam, user);
                    if (modelChange.getItemGUID().equals(elementGUID)){
                        log.info("processPropertyChange: no save of change in the list of " + modelChange.getUserName() + " because it has been deleted by " + currentModelChange.getUserName());
                        return;
                    }
                }
            }
            found = false;
            for (String id : smallTeam.getTeamMembersId()) {
                currentUser = authorizationService.getUserById(id);

                if (!currentUser.getName().equals(user.getName())) {
                    changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                    currentUserChangeIDs = changesForSynchronization.getChangeIDs();

                    for (Iterator<String> iterator = currentUserChangeIDs.iterator(); iterator.hasNext(); ) {
                        ModelChange currentModelChange = modelChangeService.getChangeById(iterator.next());

                        if (currentModelChange instanceof PropertyChange){

                            if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())
                                    && (modelChange.getPropertyType() == ((PropertyChange) currentModelChange).getPropertyType())){

                                ((PropertyChange) currentModelChange).setPropertyBody(((PropertyChange) modelChange).getPropertyBody());
                                modelChangeService.updateModelChange(currentModelChange);
                                log.info("processPropertyChange: change of PropertyBody in the list of " + modelChange.getUserName() + " at change " + ((PropertyChange) currentModelChange).getPropertyBody());
                                found = true;

                            }
                        }
                    }
                    if (!found){
                        changesForSynchronization.getChangeIDs().add(modelChange.getId());
                        changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
                    }
                    found = false;
                }
            }
        }

    }

    /**
     * method processes deletion of item
     * @param modelChange change of model
     * @param userChangeIDs ID of synchronization changes of user
     * @param smallTeam small team where user is member
     * @param user user that made change
     * @param myChangesForSynchronization changes for synchronization of user
     */
    public void processDelete(PropertyChange modelChange, List<String> userChangeIDs, SmallTeam smallTeam, User user, ChangesForSynchronization myChangesForSynchronization){
        boolean found = false;
        ModelChange currentModelChange;
        User currentUser;
        List<String> currentUserChangeIDs;
        String elementGUID, connectorGUID = "", srcGUID, targetGUID;
        ChangesForSynchronization changesForSynchronization;

        for (String id : smallTeam.getTeamMembersId()) {
            currentUser = authorizationService.getUserById(id);

            if (!currentUser.getName().equals(user.getName())) {
                changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                currentUserChangeIDs = changesForSynchronization.getChangeIDs();

                for (Iterator<String> iterator = currentUserChangeIDs.iterator(); iterator.hasNext();){

                    currentModelChange = modelChangeService.getChangeById(iterator.next());

                    if (modelChange.getItemGUID().equals(currentModelChange.getItemGUID())) {

                        if (modelChange.getElementType() == 700) {

                            if (currentModelChange instanceof PropertyChange && ((PropertyChange) currentModelChange).getPropertyType() == 405
                                    && modelChange.getPropertyBody().equals(((PropertyChange) currentModelChange).getOldPropertyBody())) {

                                if (modelChange.getPropertyBody().equals(((PropertyChange) currentModelChange).getOldPropertyBody())) {
                                    log.info("processDelete: Deletion of diagram object movement in the list of " + modelChange.getUserName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                                    iterator.remove();
                                    changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
                                }

                            } else if (currentModelChange instanceof ItemCreation && currentModelChange.getElementType() == 700) {
                                iterator.remove();
                                changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
                                log.info("processDelete: Creation found - do not save delete to the list of " + currentUser.getName() + " because noSync and there is creation in the list of " + modelChange.getUserName());
                                found = true;
                            }

                        } else if (modelChange.getPropertyType() != 11){

                            log.info("processDelete: Others: Deletion of change in the list of " + currentUser.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                            iterator.remove();
                            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);

                            if (currentModelChange instanceof ItemCreation){
                                log.info("processDelete: Creation found - do not save delete to the list of " + currentUser.getName() + " because noSync and there is creation in the list of " + modelChange.getUserName());
                                found = true;
                            }
                        }

                    } else if (modelChange.getElementType() >= 0 && modelChange.getElementType() < 50 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() >= 70
                            && currentModelChange.getElementType() <= 79){

                        if (((ItemCreation) currentModelChange).getSrcGUID().equals(modelChange.getItemGUID()) || ((ItemCreation) currentModelChange).getTargetGUID().equals(modelChange.getItemGUID())){

                            connectorGUID = currentModelChange.getItemGUID();
                            log.info("processDelete: Connector: Delete of creation in the list " + currentUser.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                            iterator.remove();
                            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);

                        }

                    } else if (connectorGUID.equals(currentModelChange.getItemGUID())){

                        log.info("processDelete: Connector: Delete of creation in the list " + currentUser.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                        iterator.remove();
                        changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);

                    }
                }
                if (!found){
                    changesForSynchronization.getChangeIDs().add(modelChange.getId());
                    changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
                }
                found = false;
            }
        }

        for (Iterator<String> iterator = userChangeIDs.iterator(); iterator.hasNext();){
            currentModelChange = modelChangeService.getChangeById(iterator.next());
            elementGUID = getElementGUIDForSynchronization(currentModelChange.getUserName(), currentModelChange.getItemGUID(), smallTeam, user);

            if (modelChange.getItemGUID().equals(elementGUID)) {

                if (modelChange.getElementType() == 700) {

                    if (currentModelChange instanceof PropertyChange && ((PropertyChange) currentModelChange).getPropertyType() == 405) {

                        if (modelChange.getPropertyBody().equals(((PropertyChange) currentModelChange).getOldPropertyBody())) {
                            log.info("processDelete2: Delete of diagram object movement in the list of " + modelChange.getUserName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                            iterator.remove();
                            changesForSynchronizationService.updateChangesForSynchronization(myChangesForSynchronization);
                        }

                    } else if (currentModelChange instanceof ItemCreation && currentModelChange.getElementType() == 700) {
                        iterator.remove();
                        changesForSynchronizationService.updateChangesForSynchronization(myChangesForSynchronization);
                        log.info(String.valueOf(currentModelChange.getElementType()));
                        log.info("processDelete2: Creation found - do not save delete to the list of " + user.getName() + " because noSync and there is creation in the list of " + modelChange.getUserName());
                    }

                } else if (modelChange.getPropertyType() != 11){

                    log.info("processDelete2: Others: Deletion of change in the list of " + user.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                    iterator.remove();
                    changesForSynchronizationService.updateChangesForSynchronization(myChangesForSynchronization);

                }

            } else if (modelChange.getElementType() >= 0 && modelChange.getElementType() < 50 && currentModelChange instanceof ItemCreation && currentModelChange.getElementType() >= 70
                    && currentModelChange.getElementType() <= 79){

                srcGUID = getElementGUIDForSynchronization(currentModelChange.getUserName(), ((ItemCreation) currentModelChange).getSrcGUID(), smallTeam, user);
                targetGUID = getElementGUIDForSynchronization(currentModelChange.getUserName(), ((ItemCreation) currentModelChange).getTargetGUID(), smallTeam, user);

                if (srcGUID.equals(modelChange.getItemGUID()) || targetGUID.equals(modelChange.getItemGUID())){

                    connectorGUID = currentModelChange.getItemGUID();
                    log.info("processDelete2: Connector: Deletion of create in the list of " + user.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                    iterator.remove();
                    changesForSynchronizationService.updateChangesForSynchronization(myChangesForSynchronization);

                }

            } else if (connectorGUID.equals(currentModelChange.getItemGUID())){

                log.info("processDelete2:Connector: Deletion of change in the list of " + user.getName() + " after deletion of " + modelChange.getItemGUID() + " by " + modelChange.getUserName());
                iterator.remove();
                changesForSynchronizationService.updateChangesForSynchronization(myChangesForSynchronization);

            }
        }
    }

    /**
     * method finds corresponding item GUID in correspondence node
     * @param userName name of user
     * @param elementGUID item GUID
     * @param smallTeam small team
     * @param user user
     * @return corresponding item GUID or empty String when corresponding item is not found
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
                log.info("elementGUID: " + currentCorrNodePart.getElementGUID());
                return currentCorrNodePart.getElementGUID();
            }
        }
        return "";
    }

    /**
     * method processes scenario changes
     * @param scenarioChange change of scenario
     * @param smallTeam small team where user is member
     * @param user user that made change
     */
    public void processScenarioChange(StepChange scenarioChange, SmallTeam smallTeam, User user){
        ChangesForSynchronization changesForSynchronization;
        for (String id : smallTeam.getTeamMembersId()){
            User currentUser = authorizationService.getUserById(id);
            if (!currentUser.getName().equals(user.getName())){
                changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                changesForSynchronization.getChangeIDs().add(scenarioChange.getId());
                changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
            }
        }
    }

    /**
     * method process deletion of scenario
     * @param scenarioChange deletion of scenario
     * @param smallTeam small team where user is member
     * @param user user that made change
     */
    public void processScenarioDelete(StepChange scenarioChange, SmallTeam smallTeam, User user){
        ChangesForSynchronization changesForSynchronization;
        for (String id : smallTeam.getTeamMembersId()){
            User currentUser = authorizationService.getUserById(id);
            if (!currentUser.getName().equals(user.getName())){
                changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(currentUser.getName(), smallTeam.getId());
                changesForSynchronization.getChangeIDs().add(scenarioChange.getId());
                changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
            }
        }
    }
}

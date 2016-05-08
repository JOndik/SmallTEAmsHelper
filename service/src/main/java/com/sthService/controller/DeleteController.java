package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.*;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RestController;

import javax.inject.Inject;
import java.util.List;

@RestController
@RequestMapping(value = "/delete", produces = MediaType.APPLICATION_JSON_VALUE)
public class DeleteController {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private ChangesForSynchronizationService changesForSynchronizationService;

    @Inject
    private CorrNodePartService corrNodePartService;

    @Inject
    private CorrespondenceNodeService correspondenceNodeService;

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> deleteSmallTeam(@RequestBody String token) {
        User user = authorizationService.getUserByToken(token);
        if (user == null){
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        if (smallTeam == null){
            return new ResponseEntity<>(HttpStatus.NOT_FOUND);
        }

        User currentUser;
        List<ModelChange> modelChanges;
        CorrespondenceNode correspondenceNode;
        List<ChangesForSynchronization> changesForSynchronizations
                = changesForSynchronizationService.getChangesForSynchronizationBySmallTeamId(smallTeam.getId());
        for (ChangesForSynchronization changesForSynchronization : changesForSynchronizations){
            changesForSynchronizationService.deleteChangesForSynchronization(changesForSynchronization);
        }

        List<CorrespondenceNodePart> correspondenceNodeParts
                = corrNodePartService.getCorrNodePartsBySmallTeamId(smallTeam.getId());
        for (CorrespondenceNodePart correspondenceNodePart : correspondenceNodeParts){

            correspondenceNode = correspondenceNodeService.getCorrNodeByCorrNodePartId(correspondenceNodePart.getId());
            if (correspondenceNode != null){
                correspondenceNodeService.deleteCorrespondenceNode(correspondenceNode);
            }

            corrNodePartService.deleteCorrNodePart(correspondenceNodePart);
        }

        for(String id : smallTeam.getTeamMembersId()){
            currentUser = authorizationService.getUserById(id);
            modelChanges = modelChangeService.findModelData(currentUser.getModelGUID());
            for (ModelChange modelChange : modelChanges){
                modelChangeService.deleteModelChange(modelChange);
            }
            currentUser.setModelGUID("");
            currentUser.setAllModelData(false);
            authorizationService.updateUser(currentUser);
        }

        smallTeamService.deleteSmallTeam(smallTeam);

        return new ResponseEntity<>("", HttpStatus.OK);
    }
}

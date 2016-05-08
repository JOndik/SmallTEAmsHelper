package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.SynchronizationService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;

@RestController
@RequestMapping(value = "/synchronization", produces = MediaType.APPLICATION_JSON_VALUE)
public class SynchronizationController {

    @Inject
    private SynchronizationService synchronizationService;

    private final Logger log = LoggerFactory.getLogger(SynchronizationController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> checkSynchronization(@RequestBody ModelInformation modelInformation) {
        String value = synchronizationService.checkSynchronizationInfo(modelInformation);
        return new ResponseEntity<>(value, HttpStatus.CREATED);
    }

    @RequestMapping(value = "/changes", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<ModelChange> sendChangeForSynchronization(@RequestBody String token) {
        ModelChange modelChange;
        if (synchronizationService.getSizeOfChangeIDs(token) > 0) {
            modelChange = synchronizationService.getChangeForSynchronization(token);
        } else {
            PropertyChange propertyChange = new PropertyChange();
            propertyChange.setTimestamp("-1");
            return new ResponseEntity<>(propertyChange, HttpStatus.CREATED);
        }
        return new ResponseEntity<>(modelChange, HttpStatus.CREATED);
    }

    @RequestMapping(value = "/createNode", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
     public ResponseEntity<String> postNewCorrespondenceNode(@RequestBody NewCorrespondenceNode newCorrespondenceNode) {
        if (!("").equals(newCorrespondenceNode.getFirstItemGUID())) {
            log.info(newCorrespondenceNode.getSecondUsername() + " " + newCorrespondenceNode.getSecondItemGUID());
            synchronizationService.findNodeParametres(newCorrespondenceNode);
        } /*else {
            synchronizationService.deleteFromList(newCorrespondenceNode);
        }*/
        return new ResponseEntity<>("", HttpStatus.CREATED);
    }

   /* @RequestMapping(value = "/wrongSynchronization", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> handleWrongSynchronization(@RequestBody String token) {
        synchronizationService.deleteAfterWrongSynchronization(token);
        return new ResponseEntity<>("", HttpStatus.CREATED);
    }*/

}

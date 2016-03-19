package com.sthService.controller;

import com.sthService.dataContract.DTOWrapper;
import com.sthService.dataContract.ModelChange;
import com.sthService.dataContract.User;
import com.sthService.repository.AuthorizationRepository;
import com.sthService.service.AuthorizationService;
import com.sthService.service.ModelChangeService;
import com.sthService.service.SynchronizationService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;
import java.util.List;

@RestController
@RequestMapping(value = "/changes", produces = MediaType.APPLICATION_JSON_VALUE)
public class ChangesController {

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private AuthorizationRepository authorizationRepository;

    @Inject
    private SynchronizationService synchronizationService;

    private final Logger log = LoggerFactory.getLogger(ChangesController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> logChange(@RequestBody DTOWrapper newChange) {
        log.info(newChange.getModelChange().getItemGUID());

        User user = authorizationService.getUser(newChange.getUserToken());
        if (user == null) {
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }

        if (newChange.getModelChange().getElementType() != 777) {                           //koncova sprava pri posielani celeho modelu ma elementType 777
            modelChangeService.saveChange(user.getName(), newChange.getModelChange());
        } else {
            //user.setAllModelData(true);
            user.setModelGUID(newChange.getModelChange().getModelGUID());
            authorizationRepository.save(user);
            synchronizationService.checkOtherTeamMembers(user);
        }

        return new ResponseEntity<>(HttpStatus.CREATED);
    }

    //@RequestMapping(value = "", method = RequestMethod.GET)
    /*@RequestMapping(value = "/{timestamp}", method = RequestMethod.GET)           // v browseri 8080/changes/12345
    public List<ModelChange> fetchAllChange(@PathVariable String timestamp) {
        log.warn(timestamp);                                                        //napise warn 12345
        return modelChangeService.fetchAllChanges();

    }*/

    @RequestMapping(value = "", method = RequestMethod.GET)
    public List<ModelChange> fetchAllChange() {
        return modelChangeService.fetchAllChanges();
    }
}

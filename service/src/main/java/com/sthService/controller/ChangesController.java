package com.sthService.controller;

import com.sthService.dataContract.DTOWrapper;
import com.sthService.dataContract.ModelChange;
import com.sthService.service.ModelChangeService;
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

    private final Logger log = LoggerFactory.getLogger(ChangesController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> logChange(@RequestBody DTOWrapper newChange) {
        log.info(newChange.getModelChange().getItemGUID());
        modelChangeService.saveChange(newChange.getModelChange());

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

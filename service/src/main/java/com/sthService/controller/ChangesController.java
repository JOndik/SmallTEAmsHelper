package com.sthService.controller;

import com.sthService.dataContract.DTOWrapper;
import com.sthService.dataContract.PropertyChange;
import com.sthService.service.ModelChangeService;
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

@RestController
@RequestMapping(value = "/changes", produces = MediaType.APPLICATION_JSON_VALUE)
public class ChangesController {

    @Inject
    private ModelChangeService modelChangeService;

    private final Logger log = LoggerFactory.getLogger(ChangesController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> logChange(@RequestBody DTOWrapper newChange) {
        modelChangeService.saveChange(newChange.getModelChange());

        log.info(((PropertyChange)newChange.getModelChange()).getPropertyBody());

        return new ResponseEntity<>(HttpStatus.CREATED);
    }
}

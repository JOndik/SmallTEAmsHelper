package com.sthService.controller;

import com.sthService.service.UpdateService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;

@RestController
@RequestMapping(value = "/update", produces = MediaType.APPLICATION_JSON_VALUE)
public class UpdateController {

    @Inject
    private UpdateService updateService;

    private final Logger log = LoggerFactory.getLogger(UpdateController.class);

    @RequestMapping(value = "", method = RequestMethod.GET)
    public ResponseEntity<String> getVersion() {
        Double version = updateService.findVersion();
        String value = String.valueOf(version);

        return new ResponseEntity<>(value, HttpStatus.CREATED);
    }
}

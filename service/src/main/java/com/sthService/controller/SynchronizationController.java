package com.sthService.controller;

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
    public ResponseEntity<String> checkSynchronization(@RequestBody String token) {
        String value = synchronizationService.checkSynchronizationInfo(token);
        return new ResponseEntity<>(value, HttpStatus.CREATED);
    }
}

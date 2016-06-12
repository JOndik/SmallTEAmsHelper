package com.sthService.controller;

import com.sthService.service.UpdateService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.http.HttpHeaders;
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

    /**
     * method finds version of our Add-In
     * @return ResponseEntity with status CREATED and String value of add-in version
     */
    @RequestMapping(value = "", method = RequestMethod.GET)
    public ResponseEntity<String> getVersion() {
        Double version = updateService.findVersion();
        String value = String.valueOf(version);

        return new ResponseEntity<>(value, HttpStatus.CREATED);
    }

    @RequestMapping(value = "/currentVersion", method = RequestMethod.GET)
    public ResponseEntity<?> getCurrentVersion(@Value("${addin.currentVersion}") String curVersion) {
        String fileName = "update" + curVersion + ".zip";
        return ResponseEntity.status(HttpStatus.FOUND).header(HttpHeaders.LOCATION, "/updates/" + fileName).build();
    }

    @RequestMapping(value = "/installer", method = RequestMethod.GET)
    public ResponseEntity<?> getInstaller() {
        return ResponseEntity.status(HttpStatus.FOUND).header(HttpHeaders.LOCATION, "/FSE/updates/SmallTEAmsHelper.msi").build();
    }
}

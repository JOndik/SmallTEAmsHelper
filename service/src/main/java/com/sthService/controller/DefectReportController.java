package com.sthService.controller;

import com.sthService.dataContract.DefectReport;
import com.sthService.dataContract.User;
import com.sthService.service.AuthorizationService;
import com.sthService.service.DefectReportService;
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
@RequestMapping(value = "/defectReports", produces = MediaType.APPLICATION_JSON_VALUE)
public class DefectReportController {

    @Inject
    private DefectReportService defectReportService;

    @Inject
    private AuthorizationService authorizationService;

    private final Logger log = LoggerFactory.getLogger(ChangesController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> logReport(@RequestBody DefectReport newReport) {
        log.info(newReport.getRuleGUID());

        User user = authorizationService.getUser(newReport.getUserToken());

        if (user == null) {
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }

        defectReportService.saveReport(user.getName(), newReport);

        return new ResponseEntity<>(HttpStatus.CREATED);
    }
}

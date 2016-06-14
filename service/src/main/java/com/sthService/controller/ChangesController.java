package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.AuthorizationService;
import com.sthService.service.ModelChangeService;
import com.sthService.service.SmallTeamService;
import com.sthService.utils.EncryptionUtils;
import com.sthService.utils.NumberUtils;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.boot.Banner;
import org.springframework.data.domain.PageRequest;
import org.springframework.data.domain.Pageable;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.util.AntPathMatcher;
import org.springframework.web.bind.annotation.*;
import org.springframework.web.context.request.RequestAttributes;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.servlet.HandlerMapping;

import javax.inject.Inject;
import java.security.GeneralSecurityException;
import java.util.Collections;
import java.util.List;
import java.util.Map;

@RestController
@RequestMapping(value = "/changes", produces = MediaType.APPLICATION_JSON_VALUE)
public class ChangesController {

    @Inject
    private ModelChangeService modelChangeService;

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private ProcessChangesController processChangesController;

    @Inject
    private CorrModelController corrModelController;

    private final Logger log = LoggerFactory.getLogger(ChangesController.class);

    /**
     * method saves new change of model to server database
     * @param newChange change of model
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     */
    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> logChange(@RequestBody DTOWrapper newChange) {
        log.info(newChange.getModelChange().getItemGUID());

        User user = authorizationService.getUser(newChange.getUserToken());
        if (user == null) {
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }

        if (newChange.getModelChange().getElementType() != 777) {
            modelChangeService.saveChange(user.getName(), newChange.getModelChange());
            SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
            if ((newChange.getModelChange().getModelGUID()).equals(user.getModelGUID()) && smallTeam != null) {
                processChangesController.processChange(newChange.getModelChange(), user);
            }
        } else {
            user.setAllModelData(true);
            user.setModelGUID(newChange.getModelChange().getModelGUID());
            authorizationService.updateUser(user);
            corrModelController.checkOtherTeamMembers(user);
        }

        return new ResponseEntity<>(HttpStatus.OK);
    }

    /**
     * method sets attribute of ModelGUID of user to GUID of model if last data about model is sent to server
     * @param modelInformation modelinformation that consists of user token and GUID of model
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status NOT_FOUND - last data about model has not come yet
     */
    @RequestMapping(value = "/lastCreate", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> findLastCreate(@RequestBody ModelInformation modelInformation) {
        log.info("Last create found: user with token " + modelInformation.getToken());
        User user = authorizationService.getUserByToken(modelInformation.getToken());
        if (user == null){
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        } else {
            if (user.getModelGUID().equals(modelInformation.getModelGUID())) {
                return new ResponseEntity<>(HttpStatus.OK);
            } else {
                return new ResponseEntity<>(HttpStatus.NOT_FOUND);
            }
        }
    }

    /**
     * method finds all model changes in database
     * @return list of model changes
     */
    @RequestMapping(value = "", method = RequestMethod.GET)
    public List<ModelChange> fetchAllChanges() {
        return modelChangeService.fetchAllChanges();
    }

    @RequestMapping(value = "/fse", method = RequestMethod.GET)
    public List<ModelChange> fetchAllChangesAnon() {
        List<ModelChange> modelChanges = null;

        try {
            modelChanges = modelChangeService.encryptChanges(modelChangeService.fetchAllChanges());
        } catch (GeneralSecurityException e) {
            log.error("Failed to anonymize dataset.", e);
            return Collections.emptyList();
        }

        return modelChanges;
    }

    @RequestMapping(value = "/fse/{pageNumber:\\d+}/{pageSize:\\d+}", method = RequestMethod.GET)
    public List<ModelChange> fetchAllChangesAnon(@PathVariable int pageNumber, @PathVariable int pageSize) {
        List<ModelChange> modelChanges = null;

        try {
            Pageable pageable = new PageRequest(pageNumber, pageSize);
            modelChanges = modelChangeService.encryptChanges(modelChangeService.fetchAllChanges(pageable));
        } catch (GeneralSecurityException e) {
            log.error("Failed to anonymize dataset.", e);
            return Collections.emptyList();
        }
        return modelChanges;
    }

    @RequestMapping(value = "/fse/**", method = RequestMethod.GET)
    public List<ModelChange> fetchChangesByAnonName(WebRequest request) {
        List<ModelChange> modelChanges = null;

        String fullPath = (String) request.getAttribute(
                HandlerMapping.PATH_WITHIN_HANDLER_MAPPING_ATTRIBUTE, RequestAttributes.SCOPE_REQUEST);
        String matchedPath = (String) request.getAttribute(
                HandlerMapping.BEST_MATCHING_PATTERN_ATTRIBUTE, RequestAttributes.SCOPE_REQUEST);

        AntPathMatcher antPathMatcher = new AntPathMatcher();
        String anonName = antPathMatcher.extractPathWithinPattern(matchedPath, fullPath);

        try {
            String decryptedName = EncryptionUtils.decrypt(anonName);
            String[] anonNameSplit = anonName.split("/");
            String pageSizeS = anonNameSplit[anonNameSplit.length - 1];
            String pageNumberS = anonNameSplit[anonNameSplit.length - 2];

            if (anonNameSplit.length > 2 && NumberUtils.areIntegers(pageSizeS, pageNumberS)) {
                int pageSize = Integer.parseInt(pageSizeS);
                int pageNumber = Integer.parseInt(pageNumberS);

                Pageable pageable = new PageRequest(pageNumber, pageSize);
                modelChanges = modelChangeService.encryptChanges(
                        modelChangeService.fetchChangesByUserName(decryptedName, pageable));
            } else {
                modelChanges = modelChangeService.encryptChanges(
                        modelChangeService.fetchChangesByUserName(decryptedName));
            }
        } catch (GeneralSecurityException e) {
            log.error("Failed to anonymize dataset.", e);
            return Collections.emptyList();
        }

        return modelChanges;
    }
}

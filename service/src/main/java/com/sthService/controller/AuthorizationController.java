package com.sthService.controller;

import com.sthService.dataContract.*;
import com.sthService.service.AuthorizationService;
import com.sthService.service.ChangesForSynchronizationService;
import com.sthService.service.PairRequestService;
import com.sthService.service.SmallTeamService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;
import javax.net.ssl.HttpsURLConnection;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.util.ArrayList;
import java.util.UUID;

/**
 * controller used for user authorization
 */
@RestController
@RequestMapping(value = "/auth", produces = MediaType.APPLICATION_JSON_VALUE)
public class AuthorizationController {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private PairRequestService pairRequestService;

    @Inject
    private SmallTeamService smallTeamService;

    @Inject
    private ChangesForSynchronizationService changesForSynchronizationService;

    private final Logger log = LoggerFactory.getLogger(AuthorizationController.class);

    @RequestMapping(value = "/register", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> createUser(@RequestBody User newUser) {
        User user = authorizationService.getUserByName(newUser.getName());

        if (("").equals(newUser.getPassword()) || ("").equals(newUser.getName())){
            return new ResponseEntity<>("Name or password has not been filled.", HttpStatus.UNAUTHORIZED);
        }

        if (user != null) {
            return new ResponseEntity<>("Name already in use.", HttpStatus.BAD_REQUEST);
        }

        authorizationService.createInternalUser(newUser);

        return new ResponseEntity<>(HttpStatus.CREATED);
    }

    /**
     * method checks AIS log in name and password by communication with AIS
     * @param user user which has sent AIS log in name and password
     * @return  ResponseEntity with status UNAUTHORIZED - AIS log in name or password is incorrect
     *          ResponseEntity with status OK - AIS log in name and password are correct
     *          ResponseEntity with status INTERNAL_SERVER_ERROR - problem with HTTPS connection
     */
    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> checkLogInData(@RequestBody User user) {
        String token = authorizationService.checkUserCredentials(user);

        if (token != null) {
            return new ResponseEntity<>(token, HttpStatus.OK);
        }

        log.info("checking AIS account of student");
        token = authorizationService.checkAISUser(user);

        if (token != null) {
            return new ResponseEntity<Object>(token, HttpStatus.OK);
        }

        return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
    }

    /**
     * method saves request for joining new member to small team
     * @param teamPair teamPair containing token of user and new member name
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status METHOD_NOT_ALLOWED -
     *          ResponseEntity with status FORBIDDEN - user wanted to add himself to team
     *          ResponseEntity with status NOT_ACCEPTABLE - number of team members is 2
     *          ResponseEntity with status BAD_REQUEST - new member of small team was not recognized
     *          ResponseEntity with status CONFLICT - new team member is already in team
     */
    @RequestMapping(value = "/pair", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> pairMembers(@RequestBody TeamPairDTO teamPair) {
        log.info("spajam " + teamPair.getToken());
        User requester = authorizationService.getUser(teamPair.getToken());

        if (requester == null) {
            log.info("tu");
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }

        TeamPairRequest pairRequest = pairRequestService.getPairRequestByMemberName(requester.getName());

        if (pairRequest != null) {
            return new ResponseEntity<>(HttpStatus.METHOD_NOT_ALLOWED);
        }

        if (requester.getName().equals(teamPair.getTeamMemberName())){
            log.info("self join");
            return new ResponseEntity<>(HttpStatus.FORBIDDEN);
        }

        SmallTeam smallTeam = smallTeamService.getByUserId(requester.getId());

        if (smallTeam == null) {
            smallTeamService.createSmallTeam(requester.getId());
        } else {
            if (smallTeam.getTeamMembersId().size() == 2) {
                return new ResponseEntity<>(HttpStatus.NOT_ACCEPTABLE);
            }
        }

        User newMember = authorizationService.getUserByName(teamPair.getTeamMemberName());

        if (newMember == null || newMember.getToken() == null) {
            log.info("new Member");
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        User user;
        if (smallTeam != null){
            for (String id : smallTeam.getTeamMembersId()){
                user = authorizationService.getUserById(id);
                if (user.getName().equals(teamPair.getTeamMemberName())){
                    log.info("member is in small team");
                    return new ResponseEntity<>(HttpStatus.CONFLICT);
                }
            }
        }

        //
        smallTeam = smallTeamService.getByUserId(requester.getId());
        smallTeamService.updateTeam(smallTeam);
        //

        ChangesForSynchronization changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(requester.getName(), smallTeam.getId());
        if (changesForSynchronization == null) {
            changesForSynchronizationService.createChangesForSynchronization(requester.getName(), smallTeam.getId());
        } else {
            changesForSynchronization.setChangeIDs(new ArrayList<>());
            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
        }

        ChangesForSynchronization changesForSynchronization2 = changesForSynchronizationService.findChangesForSynchronization(newMember.getName(), smallTeam.getId());
        if (changesForSynchronization2 == null) {
            changesForSynchronizationService.createChangesForSynchronization(newMember.getName(), smallTeam.getId());
        } else {
            changesForSynchronization2.setChangeIDs(new ArrayList<>());
            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization2);
        }

        pairRequest = new TeamPairRequest();
        pairRequest.setMemberName(teamPair.getTeamMemberName());

        String requestToken = authorizationService.generateToken();
        pairRequest.setToken(requestToken);
        pairRequest.setRequesterId(requester.getId());

        pairRequestService.savePairRequest(pairRequest, newMember);

        return new ResponseEntity<>(HttpStatus.OK);
    }

    /**
     * method adds new team member after his clicking on link in email
     * @param pairToken token of pair request
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status BAD_REQUEST - pair request was not found
     *          ResponseEntity with status BAD_GATEWAY - new team member was not found
     */
    @RequestMapping(value = "/pair/{pairToken}", method = RequestMethod.GET)
    public ResponseEntity<?> confirmPair(@PathVariable String pairToken) {
        log.info("confirm pair");
        TeamPairRequest pairRequest = pairRequestService.getPairRequest(pairToken);

        if (pairRequest == null) {
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        log.info("small team");
        SmallTeam smallTeam = smallTeamService.getByUserId(pairRequest.getRequesterId());
        User user = authorizationService.getUserByName(pairRequest.getMemberName());

        if (smallTeam == null) {
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        if (user == null) {
            return new ResponseEntity<>(HttpStatus.BAD_GATEWAY);
        }

        log.info("add team member");
        smallTeam.getTeamMembersId().add(user.getId());
        smallTeamService.updateTeam(smallTeam);

        log.info("set changesForSync");
        ChangesForSynchronization changesForSynchronization = changesForSynchronizationService.findChangesForSynchronization(user.getName(), smallTeam.getId());
        if (changesForSynchronization == null) {
            changesForSynchronizationService.createChangesForSynchronization(user.getName(), smallTeam.getId());
        } else {
            changesForSynchronization.setChangeIDs(new ArrayList<>());
            changesForSynchronizationService.updateChangesForSynchronization(changesForSynchronization);
        }

        log.info("delete request");
        pairRequestService.deleteRequest(pairRequest);
        return new ResponseEntity<>("Sparovanie dokoncene", HttpStatus.OK);
    }

    /**
     * method checks if user is member of a team
     * @param token token of user
     * @return  ResponseEntity with status OK
     *          ResponseEntity with status UNAUTHORIZED - user was not found
     *          ResponseEntity with status NOT_FOUND - user is not member of any small team
     */
    @RequestMapping(value = "/checkJoining", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> checkJoining(@RequestBody String token) {
        User user = authorizationService.getUserByToken(token);
        if (user == null){
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }
        SmallTeam smallTeam = smallTeamService.getByUserId(user.getId());
        if (smallTeam == null){
            return new ResponseEntity<>(HttpStatus.NOT_FOUND);
        }
        else {
            return new ResponseEntity<>(HttpStatus.OK);
        }
    }
}

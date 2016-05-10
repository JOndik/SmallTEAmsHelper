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

    /**
     * method checks AIS log in name and password by communication with AIS
     * @param user user which has sent AIS log in name and password
     * @return  ResponseEntity with status UNAUTHORIZED - AIS log in name or password is incorrect
     *          ResponseEntity with status OK - AIS log in name and password are correct
     *          ResponseEntity with status INTERNAL_SERVER_ERROR - problem with HTTPS connection
     */
    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> checkLogInData(@RequestBody User user) {
        URL url;

        try {
            url = new URL("https://maya.fiit.stuba.sk/LDAPAuth/api/auth?username=" + user.getName());

            HttpsURLConnection conn = (HttpsURLConnection) url.openConnection();

            conn.setRequestMethod("POST");
            conn.setRequestProperty("Content-Type", "application/x-www-form-urlencoded");
            conn.setRequestProperty("Content-Length", Integer.toString(user.getPassword().getBytes().length));
            conn.setRequestProperty("Host", "maya.fiit.stuba.sk");
            conn.setDoOutput(true);

            DataOutputStream writer = new DataOutputStream(conn.getOutputStream());
            writer.writeBytes("=" + user.getPassword());
            writer.flush();
            writer.close();

            int responseCode = conn.getResponseCode();

            if (responseCode == HttpsURLConnection.HTTP_OK) {
                BufferedReader in = new BufferedReader(new InputStreamReader(conn.getInputStream()));
                String inputLine;
                StringBuffer response = new StringBuffer();

                while ((inputLine = in.readLine()) != null) {
                    response.append(inputLine);
                }
                in.close();

                log.info(response.toString());
                if (("true").equals(response.toString())) {

                    log.info("AIS data of user " + user.getName() + " are correct");

                    User foundUserByName = authorizationService.getUserByName(user.getName()); //authorizationRepository.findByName(user.getName());
                    String token = generateToken();

                    log.info("Token " + token + " was generated");

                    if (foundUserByName == null){
                        log.info("User " + user.getName() + " is not in database");

                        authorizationService.createUser(user.getName(), token);

                        log.info("User " + user.getName() + " was inserted to database");

                    } else {
                        log.info("User " + user.getName() + " is in database");

                        foundUserByName.setToken(token);
                        authorizationService.updateUser(foundUserByName);

                        log.info("Token of user " + user.getName() + " was changed to " + token);
                    }

                    return new ResponseEntity<>(token, HttpStatus.OK);

                } else {
                    log.info("AIS data of user " + user.getName() + " are incorrect");
                    return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
                }
            } else {
                log.error("RespondeCode does not equal HTTP_OK, responseCode: " + responseCode);
                return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
            }
        } catch (Exception e) {
            log.error("HttpsURLConnection was not created successfully: " + e.toString());
            return new ResponseEntity<>(HttpStatus.INTERNAL_SERVER_ERROR);
        }

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

        if (newMember == null) {
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

        String requestToken = generateToken();
        pairRequest.setToken(requestToken);
        pairRequest.setRequesterId(requester.getId());

        pairRequestService.savePairRequest(pairRequest);

        return new ResponseEntity<>(HttpStatus.OK);
    }

    /**
     * method adds new team member after his clicking of link in email
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

    /**
     * method generates token for new user
     * @return generated token
     */
    public String generateToken(){
        String uuid;
        while(true) {
            uuid = UUID.randomUUID().toString();
            log.info(uuid);
            User user = authorizationService.getUserByToken(uuid);
            if(user == null){
                return uuid;
            }
        }
    }
}

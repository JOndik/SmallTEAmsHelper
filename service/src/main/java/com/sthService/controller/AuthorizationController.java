package com.sthService.controller;

import com.sthService.dataContract.SmallTeam;
import com.sthService.dataContract.TeamPairDTO;
import com.sthService.dataContract.TeamPairRequest;
import com.sthService.dataContract.User;
import com.sthService.service.AuthorizationService;
import com.sthService.service.PairRequestService;
import com.sthService.service.SmallTeamService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.inject.Inject;

@RestController
@RequestMapping(value = "/auth", produces = MediaType.APPLICATION_JSON_VALUE)
public class AuthorizationController {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private PairRequestService pairRequestService;

    @Inject
    private SmallTeamService smallTeamService;

    private final Logger log = LoggerFactory.getLogger(AuthorizationController.class);

    @RequestMapping(value = "", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<String> postLogInData(@RequestBody User user) {
        String value = authorizationService.checkLogInData(user);
        return new ResponseEntity<>(value, HttpStatus.CREATED);
    }

    @RequestMapping(value = "/pair", method = RequestMethod.POST, consumes = MediaType.APPLICATION_JSON_VALUE)
    public ResponseEntity<?> pairMembers(@RequestBody TeamPairDTO teamPair) {
        log.info("spajam " + teamPair.getToken());
        User requester = authorizationService.getUser(teamPair.getToken());
        //User newMember = authorizationService.getUserByName(teamPair.getTeamMemberName());

        if (requester == null) {
            return new ResponseEntity<>(HttpStatus.UNAUTHORIZED);
        }

        User newMember = authorizationService.getUserByName(teamPair.getTeamMemberName());

        if (newMember == null) {
            return  new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        log.warn("tu som2");
        SmallTeam smallTeam = smallTeamService.getByUserId(requester.getId());

        if (smallTeam == null) {
            smallTeamService.createSmallTeam(requester.getId());
        }

        TeamPairRequest pairRequest = new TeamPairRequest();
        pairRequest.setMemberName(teamPair.getTeamMemberName());

        String requestToken = authorizationService.generateToken();
        pairRequest.setToken(requestToken);
        pairRequest.setRequesterId(requester.getId());

        pairRequestService.savePairRequest(pairRequest);

        return new ResponseEntity<>(HttpStatus.OK);
    }

    @RequestMapping(value = "/pair/{pairToken}", method = RequestMethod.GET)
    public ResponseEntity<?> confirmPair(@PathVariable String pairToken) {
        TeamPairRequest pairRequest = pairRequestService.getPairRequest(pairToken);

        if (pairRequest == null) {
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        SmallTeam smallTeam = smallTeamService.getByUserId(pairRequest.getRequesterId());
        User user = authorizationService.getUserByName(pairRequest.getMemberName());

        if (smallTeam == null) {
            return new ResponseEntity<>(HttpStatus.BAD_REQUEST);
        }

        if (user == null) {
            return new ResponseEntity<>(HttpStatus.BAD_GATEWAY);
        }

        smallTeam.getTeamMembersId().add(user.getId());
        smallTeamService.updateTeam(smallTeam);

        pairRequestService.deleteRequest(pairRequest);

        return new ResponseEntity<>(HttpStatus.OK);
    }
}

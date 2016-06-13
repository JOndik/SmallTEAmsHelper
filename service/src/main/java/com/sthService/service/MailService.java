package com.sthService.service;

import com.sthService.dataContract.TeamPairRequest;
import com.sthService.dataContract.User;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.MailSender;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.stereotype.Service;

import javax.inject.Inject;

@Service
public class MailService {

    private final Logger log = LoggerFactory.getLogger(MailService.class);

    @Inject
    private MailSender mailSender;

    @Inject
    private PairRequestService pairRequestService;

    @Inject
    private AuthorizationService authorizationService;

    @Value("${addin.aisMail.from}")
    private String sender;

    @Value("${addin.aisMail.subject}")
    private String subject;

    @Value("${addin.aisMail.domain}")
    private String domain;

    /**
     * method sends confirmation email to AIS
     * @param username name of user
     * @param token token of user
     */
    public void sendPairRequestEmailToAIS(String username, String token) {
        log.info("Sending: " + username + "@" + domain);
        String requesterName = "";
        TeamPairRequest pairRequest = pairRequestService.getPairRequest(token);
        if (pairRequest != null){
            User user = authorizationService.getUserById(pairRequest.getRequesterId());
            if (user != null) {
                requesterName = user.getName();
            }
        }
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(username + "@" + domain);
        message.setFrom(this.sender);
        message.setSubject(this.subject);
        message.setText("Please, click on the link below for confirmation of your joining to team. User " + requesterName + " has invited you.\n\n"
                +"https://ichiban.fiit.stuba.sk:8443/auth/pair/" + token);
        mailSender.send(message);
    }

    /**
     * method sends confirmation email to user email address
     * @param email email address of user
     * @param token token of user
     */
    public void sendPairRequestEmail(String email, String token) {
        log.info("Sending email: " + email);
        String requesterName = "";
        TeamPairRequest pairRequest = pairRequestService.getPairRequest(token);
        if (pairRequest != null){
            User user = authorizationService.getUserById(pairRequest.getRequesterId());
            if (user != null) {
                requesterName = user.getName();
            }
        }
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(email);
        message.setFrom(this.sender);
        message.setSubject(this.subject);
        message.setText("Please, click on the link below for confirmation of your joining to team. User " + requesterName + " has invited you.\n\n"
                +"https://ichiban.fiit.stuba.sk:8443/auth/pair/" + token);
        mailSender.send(message);
    }
}

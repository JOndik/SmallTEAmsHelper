package com.sthService.service;

import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.MailSender;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.stereotype.Service;

import javax.inject.Inject;

@Service
public class AISMailService {

    private final Logger log = LoggerFactory.getLogger(AuthorizationService.class);

    @Inject
    private MailSender mailSender;

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
    public void sendPairRequestEmail(String username, String token) {
        log.info("Sending: " + username + "@" + domain);
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(username + "@" + domain);
        message.setFrom(this.sender);
        message.setSubject(this.subject);
        message.setText("Prosim, kliknite na odkaz dole pre dokoncenie sparovania:\n\n"
                +"https://ichiban.fiit.stuba.sk:8443/auth/pair/" + token);
        mailSender.send(message);
    }
}

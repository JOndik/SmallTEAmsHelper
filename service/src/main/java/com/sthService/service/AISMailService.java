package com.sthService.service;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.mail.MailSender;
import org.springframework.mail.SimpleMailMessage;
import org.springframework.stereotype.Service;

import javax.inject.Inject;

@Service
public class AISMailService {

    @Inject
    private MailSender mailSender;

    @Value("$(addin.aisMail.from)")
    private String sender;

    @Value("$(addin.aisMail.subject)")
    private String subject;

    public void sendPairRequestEmail(String username, String token) {
        SimpleMailMessage message = new SimpleMailMessage();
        message.setTo(username + "@is.stuba.sk");
        message.setFrom(this.sender);
        message.setSubject(this.subject);
        message.setText("https://ichiban.fiit.stuba.sk:8443/auth/pair/" + token);
        mailSender.send(message);
    }
}

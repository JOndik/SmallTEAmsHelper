package com.sthService.service;

import com.sthService.dataContract.User;
import com.sthService.repository.AuthorizationRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import javax.net.ssl.HttpsURLConnection;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;
import javax.net.ssl.X509TrustManager;
import java.io.BufferedReader;
import java.io.DataOutputStream;
import java.io.InputStreamReader;
import java.net.URL;
import java.security.GeneralSecurityException;
import java.security.cert.X509Certificate;
import java.util.UUID;

@Service
@Transactional
public class AuthorizationService {

    @Inject
    private AuthorizationRepository authorizationRepository;

    private final Logger log = LoggerFactory.getLogger(AuthorizationService.class);

    public String checkLogInData(User user){
        TrustManager[] trustAllCerts = new TrustManager[] {
                new X509TrustManager() {
                    public java.security.cert.X509Certificate[] getAcceptedIssuers() {
                        return new X509Certificate[0];
                    }
                    public void checkClientTrusted(
                            java.security.cert.X509Certificate[] certs, String authType) {
                    }
                    public void checkServerTrusted(
                            java.security.cert.X509Certificate[] certs, String authType) {
                    }
                }
        };

        // Install the all-trusting trust manager
        try {
            SSLContext sc = SSLContext.getInstance("SSL");
            sc.init(null, trustAllCerts, new java.security.SecureRandom());
            HttpsURLConnection.setDefaultSSLSocketFactory(sc.getSocketFactory());
        } catch (GeneralSecurityException e) {
            log.error("SSLContext was not created successfully: " + e.toString());
            return "error";
        }
        // Now you can access an https URL without having the certificate in the truststore

        HttpsURLConnection.setDefaultHostnameVerifier((hostname, session) -> true);
        //log.info(logIn.getName() + " " + logIn.getPassword());
        URL url = null;

        try {
            url = new URL("https://maya.fiit.stuba.sk/LDAPAuth/api/auth?username=" + user.getName());

            //String urlString = "https://maya.fiit.stuba.sk/LDAPAuth/api/auth?username=" + logIn.getName();
            //URL url = new URL(urlString);
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

                    User foundUserByName = authorizationRepository.findByName(user.getName());
                    String token = generateToken();

                    log.info("Token " + token + " was generated");

                    if (foundUserByName == null){
                        log.info("User " + user.getName() + " is not in database");

                        user.setToken(token);
                        authorizationRepository.save(user);

                        log.info("User " + user.getName() + " was inserted to database");

                    } else {
                        log.info("User " + user.getName() + " is in database");

                        foundUserByName.setToken(token);
                        authorizationRepository.save(foundUserByName);

                        log.info("Token of user " + user.getName() + " was changed to " + token);
                    }

                    return token;

                } else {
                    log.info("AIS data of user " + user.getName() + " are incorrect");
                    return "false";
                }
            } else {
                log.error("RespondeCode does not equal HTTP_OK, responseCode: " + responseCode);
                return "error";
            }
        } catch (Exception e) {
            log.error("HttpsURLConnection was not created successfully: " + e.toString());
            return "error";
        }
    }

    public String generateToken(){
        String uuid;
        while(true) {
            uuid = UUID.randomUUID().toString();
            log.info(uuid);
            User user = authorizationRepository.findByToken(uuid);
            if(user == null){
                return uuid;
            }
        }
    }

    public User getUser(String token) {
        return authorizationRepository.findByToken(token);
    }

    public User getUserByName(String name) {
        return authorizationRepository.findByName(name);
    }
}

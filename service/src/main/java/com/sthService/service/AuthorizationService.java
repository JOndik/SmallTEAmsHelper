package com.sthService.service;

import com.sthService.dataContract.User;
import com.sthService.repository.AuthorizationRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import javax.net.ssl.*;
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

    @Inject
    private PasswordEncoder passwordEncoder;

    private final Logger log = LoggerFactory.getLogger(AuthorizationService.class);

    public String checkUserCredentials(User authUser) {
        User user = getUserByName(authUser.getName());

        if (user == null) {
            return null;
        }
        log.info("Heslo: " + user.getName() + " " + user.getPassword() + " " + user.getToken());

        if (passwordEncoder.matches(authUser.getPassword(), user.getPassword())) {
            log.info("credentials if");
            String token = generateToken();

            log.info("Token " + token + " was generated");
            log.info("User " + user.getName() + " is in database");

            user.setToken(token);
            authorizationRepository.save(user);

            log.info("Token of user " + user.getName() + " was set to " + token);
            return token;
        }

        return null;
    }

    public String checkAISUser(User user) {
        URL url;

        try {
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

                // Create all-trusting host name verifier
                HostnameVerifier allHostsValid = new HostnameVerifier() {
                    public boolean verify(String hostname, SSLSession session) {
                        return true;
                    }
                };

                // Install the all-trusting host verifier
                HttpsURLConnection.setDefaultHostnameVerifier(allHostsValid);
            } catch (GeneralSecurityException e) {
                log.error("SSLContext was not created successfully: " + e.toString());
                return null;
            }

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

                    User foundUserByName = getUserByName(user.getName()); //authorizationRepository.findByName(user.getName());
                    String token = generateToken();

                    log.info("Token " + token + " was generated");

                    if (foundUserByName == null){
                        log.info("User " + user.getName() + " is not in database");

                        createUser(user.getName(), token);

                        log.info("User " + user.getName() + " was inserted to database");

                    } else {
                        log.info("User " + user.getName() + " is in database " + user.getToken() + " " + user.getId());

                        foundUserByName.setToken(token);
                        authorizationRepository.save(foundUserByName);

                        log.info("Token of user " + user.getName() + " was changed to " + token);
                    }

                    return token;

                } else {
                    log.info("AIS data of user " + user.getName() + " are incorrect");
                    return null;
                }
            } else {
                log.error("ResponseCode does not equal HTTP_OK, responseCode: " + responseCode);
                return null;
            }
        } catch (Exception e) {
            log.error("HttpsURLConnection was not created successfully: " + e.toString());
            return null;
        }
    }

    /**
     * method creates new user
     * @param name name of user
     * @param token token of user
     */
    public void createUser(String name, String token){
        User user = new User();
        user.setName(name);
        user.setToken(token);
        user.setModelGUID("");
        user.setAllModelData(false);
        authorizationRepository.save(user);
    }

    public void createInternalUser(User user){
        log.info("creating internal user");
        user.setPassword(passwordEncoder.encode(user.getPassword()));
        user.setModelGUID("");
        user.setAllModelData(false);
        authorizationRepository.save(user);
    }


    /**
     * method finds user by his token
     * @param token token of user
     * @return found user
     */
    public User getUser(String token) {
        return authorizationRepository.findByToken(token);
    }

    /**
     * method finds user by his name
     * @param name name of user
     * @return found user
     */
    public User getUserByName(String name) {
        return authorizationRepository.findByName(name);
    }

    /**
     * method finds user by his id
     * @param id id of user
     * @return found user
     */
    public User getUserById(String id){
        return authorizationRepository.findById(id);
    }

    /**
     * method finds user by his token
     * @param token token of user
     * @return found user
     */
    public User getUserByToken(String token){
        return authorizationRepository.findByToken(token);
    }

    /**
     * method updates user
     * @param user user to be updated
     */
    public void updateUser(User user){
        authorizationRepository.save(user);
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
            User user = getUserByToken(uuid);
            if(user == null){
                return uuid;
            }
        }
    }
}

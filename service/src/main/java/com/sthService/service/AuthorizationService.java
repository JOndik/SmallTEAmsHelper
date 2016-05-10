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
}

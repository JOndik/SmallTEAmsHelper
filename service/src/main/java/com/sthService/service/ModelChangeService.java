package com.sthService.service;

import com.sthService.dataContract.ItemCreation;
import com.sthService.dataContract.ModelChange;
import com.sthService.repository.ModelChangeRepository;
import org.apache.tomcat.util.codec.binary.Base64;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.crypto.Cipher;
import javax.crypto.NoSuchPaddingException;
import javax.crypto.spec.SecretKeySpec;
import javax.inject.Inject;
import java.security.GeneralSecurityException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.NoSuchProviderException;
import java.util.Collections;
import java.util.List;

@Service
@Transactional
public class ModelChangeService {

    @Inject
    private ModelChangeRepository modelChangeRepository;

    @Value("${addin.anonkey}")
    private String encryptionKey;

    /**
     * method saves new change and sets its author
     * @param username name of change author
     * @param newChange new change
     */
    public void saveChange(String username, ModelChange newChange) {
        newChange.setUserName(username);
        modelChangeRepository.save(newChange);
    }

    /**
     * method finds all model changes
     * @return list of all model changes
     */
    public List<ModelChange> fetchAllChanges() {
        return modelChangeRepository.findAll();
    }

    public List<ModelChange> fetchAllChanges(Pageable pageable) {
        return modelChangeRepository.findAll(pageable).getContent();
    }

    /**
     * method finds data about model according to parameter timestamp
     * @param timestamp time when model change was done
     * @return list of found model changes
     */
    public List<ModelChange> findModelData(String timestamp){
        return modelChangeRepository.findByTimestamp(timestamp);
    }

    /**
     * method finds model change by its ID
     * @param id ID of model change
     * @return found model change
     */
    public ModelChange getChangeById(String id){
        return modelChangeRepository.findById(id);
    }

    /**
     * method finds model changes according to parameters
     * @param userName name of change author
     * @param timestamp time when model change was done
     * @return list of found model changes
     */
    public List<ModelChange> getChangesByUsernameAndTimestamp(String userName, String timestamp){
        return modelChangeRepository.findByUserNameAndTimestamp(userName, timestamp);
    }

    /**
     * method updates model change
     * @param modelChange model change to be updated
     */
    public void updateModelChange(ModelChange modelChange){
        modelChangeRepository.save(modelChange);
    }

    /**
     * method removes model change
     * @param modelChange model change to be removed
     */
    public void deleteModelChange(ModelChange modelChange){
        modelChangeRepository.delete(modelChange);
    }

    public List<ModelChange> encryptChanges(List<ModelChange> modelChanges) throws GeneralSecurityException {
        for (ModelChange modelChange : modelChanges) {
            if (modelChange.getUserName() != null) {
                String encryptedUserName = encrypt(modelChange.getUserName());
                modelChange.setUserName(encryptedUserName);
            }

            if (modelChange instanceof ItemCreation && ((ItemCreation) modelChange).getAuthor() != null) {
                String encryptedAuthor = encrypt(((ItemCreation) modelChange).getAuthor());
                ((ItemCreation) modelChange).setAuthor(encryptedAuthor);
            }
        }

        return modelChanges;
    }

    public List<ModelChange> fetchChangesByUserName(String userName) {
        return modelChangeRepository.findByUserName(userName);
    }

    public List<ModelChange> fetchChangesByUserName(String userName, Pageable pageable) {
        return modelChangeRepository.findByUserName(userName, pageable);
    }

    public String encrypt(String data) throws GeneralSecurityException {
        SecretKeySpec key = new SecretKeySpec(encryptionKey.getBytes(), "AES");
        Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5PADDING");
        cipher.init(Cipher.ENCRYPT_MODE, key);
        byte[] encryptedBytes = cipher.doFinal(data.getBytes());

        return Base64.encodeBase64String(encryptedBytes);
    }

    public String decrypt(String data) throws GeneralSecurityException {
        SecretKeySpec key = new SecretKeySpec(encryptionKey.getBytes(), "AES");
        Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5PADDING");
        cipher.init(Cipher.DECRYPT_MODE, key);
        byte[] decryptedBytes = cipher.doFinal(Base64.decodeBase64(data));

        return new String(decryptedBytes);
    }
}

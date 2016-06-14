package com.sthService.utils;

import org.apache.tomcat.util.codec.binary.Base64;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Component;

import javax.annotation.PostConstruct;
import javax.crypto.Cipher;
import javax.crypto.spec.SecretKeySpec;
import java.security.GeneralSecurityException;

@Component
public class EncryptionUtils {

    private static String encryptionKey = null;

    @Value("${addin.anonkey}")
    private String key;

    @PostConstruct
    public void init() {
        encryptionKey = key;
    }

    public static String encrypt(String data) throws GeneralSecurityException {
        SecretKeySpec key = new SecretKeySpec(encryptionKey.getBytes(), "AES");
        Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5PADDING");
        cipher.init(Cipher.ENCRYPT_MODE, key);
        byte[] encryptedBytes = cipher.doFinal(data.getBytes());

        return Base64.encodeBase64String(encryptedBytes);
    }

    public static String decrypt(String data) throws GeneralSecurityException {
        SecretKeySpec key = new SecretKeySpec(encryptionKey.getBytes(), "AES");
        Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5PADDING");
        cipher.init(Cipher.DECRYPT_MODE, key);
        byte[] decryptedBytes = cipher.doFinal(Base64.decodeBase64(data));

        return new String(decryptedBytes);
    }
}

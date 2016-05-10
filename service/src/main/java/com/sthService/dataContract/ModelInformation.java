package com.sthService.dataContract;

/**
 * class used for transfer of model information
 */
public class ModelInformation {

    private String token;

    private String modelGUID;

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public String getModelGUID() {
        return modelGUID;
    }

    public void setModelGUID(String modelGUID) {
        this.modelGUID = modelGUID;
    }
}

package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DTOWrapper {

    private ModelChange modelChange;

    private String userToken;

    public ModelChange getModelChange() {
        return modelChange;
    }

    public void setModelChange(ModelChange modelChange) {
        this.modelChange = modelChange;
    }

    public String getUserToken() {
        return userToken;
    }

    public void setUserToken(String userToken) {
        this.userToken = userToken;
    }
}

package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

@JsonIgnoreProperties(ignoreUnknown = true)
public class DTOWrapper {

    private ModelChange modelChange;
    private String userGUID;

    public ModelChange getModelChange() {
        return modelChange;
    }

    public void setModelChange(ModelChange modelChange) {
        this.modelChange = modelChange;
    }

    public String getUserGUID() {
        return userGUID;
    }

    public void setUserGUID(String userGUID) {
        this.userGUID = userGUID;
    }
}

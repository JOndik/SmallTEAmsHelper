package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class CorrespondenceNodePart {

    @Id
    private String id;

    @NotNull
    private String userName;

    @NotNull
    private String elementGUID;

    @NotNull
    private String modelGUID;

    @NotNull
    private String smallTeamID;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getElementGUID() {
        return elementGUID;
    }

    public void setElementGUID(String elementGUID) {
        this.elementGUID = elementGUID;
    }

    public String getSmallTeamID() {
        return smallTeamID;
    }

    public void setSmallTeamID(String smallTeamID) {
        this.smallTeamID = smallTeamID;
    }

    public String getModelGUID() {
        return modelGUID;
    }

    public void setModelGUID(String modelGUID) {
        this.modelGUID = modelGUID;
    }
}

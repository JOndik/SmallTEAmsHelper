package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;

import com.fasterxml.jackson.annotation.JsonProperty;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

/**
 * class used for storing information about user
 */
@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class User {

    @Id
    private String id;

    @NotNull
    private String name;

    @NotNull
    private String password;

    @NotNull
    private String token;

    @NotNull
    private String modelGUID;

    @NotNull
    private boolean allModelData;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    @JsonIgnore
    public String getPassword() {
        return password;
    }

    @JsonProperty
    public void setPassword(String password) {
        this.password = password;
    }

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getModelGUID() {
        return modelGUID;
    }

    public void setModelGUID(String modelGUID) {
        this.modelGUID = modelGUID;
    }

    public boolean isAllModelData() {
        return allModelData;
    }

    public void setAllModelData(boolean allModelData) {
        this.allModelData = allModelData;
    }
}

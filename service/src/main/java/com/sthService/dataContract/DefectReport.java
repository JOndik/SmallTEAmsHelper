package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import org.springframework.data.annotation.Id;
import org.springframework.data.annotation.Transient;
import org.springframework.data.mongodb.core.mapping.Document;

import javax.validation.constraints.NotNull;

/**
 * class used for report defects in models
 */
@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class DefectReport {

    @Id
    @JsonIgnore
    private String id;

    @NotNull
    private String timestamp;

    @NotNull
    private String userName;

    @Transient
    private String userToken;

    @NotNull
    private String modelGUID;

    @NotNull
    private String ruleName;

    @NotNull
    private String ruleGUID;

    @NotNull
    private int actionsBeforeCorrection;

    @NotNull
    private int isHidden;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getUserToken() {
        return userToken;
    }

    public void setUserToken(String userToken) {
        this.userToken = userToken;
    }

    public String getModelGUID() {
        return modelGUID;
    }

    public void setModelGUID(String modelGUID) {
        this.modelGUID = modelGUID;
    }

    public String getRuleName() {
        return ruleName;
    }

    public void setRuleName(String ruleName) {
        this.ruleName = ruleName;
    }

    public String getRuleGUID() {
        return ruleGUID;
    }

    public void setRuleGUID(String ruleGUID) {
        this.ruleGUID = ruleGUID;
    }

    public int getActionsBeforeCorrection() {
        return actionsBeforeCorrection;
    }

    public void setActionsBeforeCorrection(int actionsBeforeCorrection) {
        this.actionsBeforeCorrection = actionsBeforeCorrection;
    }

    public int isHidden() {
        return isHidden;
    }

    public void setHidden(int hidden) {
        isHidden = hidden;
    }
}

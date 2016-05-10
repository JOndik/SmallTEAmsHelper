package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

/**
 * class used for storing changes that have to be made in next model synchronization
 */
@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class ChangesForSynchronization {

    @Id
    private String id;

    @NotNull
    private String userName;

    @NotNull
    private List<String> changeIDs;

    @NotNull
    private String smallTeamId;

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

    public List<String> getChangeIDs() {
        return changeIDs;
    }

    public void setChangeIDs(List<String> changeIDs) {
        this.changeIDs = changeIDs;
    }

    public String getSmallTeamId() {
        return smallTeamId;
    }

    public void setSmallTeamId(String smallTeamId) {
        this.smallTeamId = smallTeamId;
    }
}

package com.sthService.dataContract;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
public class SmallTeam {

    @Id
    private String id;

    private List<String> teamMembersId;

    private String correspondenceModelID;

    private boolean synchronizationAllowed;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public List<String> getTeamMembersId() {
        return teamMembersId;
    }

    public void setTeamMembersId(List<String> teamMembersId) {
        this.teamMembersId = teamMembersId;
    }

    public String getCorrespondenceModelID() {
        return correspondenceModelID;
    }

    public void setCorrespondenceModelID(String correspondenceModelID) {
        this.correspondenceModelID = correspondenceModelID;
    }

    public boolean isSynchronizationAllowed() {
        return synchronizationAllowed;
    }

    public void setSynchronizationAllowed(boolean synchronizationAllowed) {
        this.synchronizationAllowed = synchronizationAllowed;
    }
}

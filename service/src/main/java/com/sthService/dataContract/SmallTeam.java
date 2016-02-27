package com.sthService.dataContract;

import org.springframework.data.annotation.Id;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
public class SmallTeam {

    @Id
    private String id;

    private List<String> teamMembersId;

    private String projectGUID;

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

    public String getProjectGUID() {
        return projectGUID;
    }

    public void setProjectGUID(String projectGUID) {
        this.projectGUID = projectGUID;
    }
}

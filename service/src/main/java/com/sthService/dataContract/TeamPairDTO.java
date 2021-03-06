package com.sthService.dataContract;

/**
 * class used for representing node of correspondence model
 */
public class TeamPairDTO {

    private String token;
    private String teamMemberName;

    public String getToken() {
        return token;
    }

    public void setToken(String token) {
        this.token = token;
    }

    public String getTeamMemberName() {
        return teamMemberName;
    }

    public void setTeamMemberName(String teamMemberName) {
        this.teamMemberName = teamMemberName;
    }
}

package com.sthService.dataContract;

/**
 * class used for transfer of new correspondence node
 */
public class NewCorrespondenceNode {

    private String firstUsername;

    private String firstItemGUID;

    private String secondUsername;

    private String secondItemGUID;

    public String getSecondItemGUID() {
        return secondItemGUID;
    }

    public void setSecondItemGUID(String secondItemGUID) {
        this.secondItemGUID = secondItemGUID;
    }

    public String getSecondUsername() {
        return secondUsername;
    }

    public void setSecondUsername(String secondUsername) {
        this.secondUsername = secondUsername;
    }

    public String getFirstUsername() {
        return firstUsername;
    }

    public void setFirstUsername(String firstUsername) {
        this.firstUsername = firstUsername;
    }

    public String getFirstItemGUID() {
        return firstItemGUID;
    }

    public void setFirstItemGUID(String firstItemGUID) {
        this.firstItemGUID = firstItemGUID;
    }
}

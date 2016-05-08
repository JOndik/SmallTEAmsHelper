package com.sthService.dataContract;

import org.springframework.data.mongodb.core.mapping.Document;

@Document
public class ScenarioChange extends ModelChange {

    private String name;
    private String type;
    private int status;
    private String scenarioGUID;

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public int getStatus() {
        return status;
    }

    public void setStatus(int status) {
        this.status = status;
    }

    public String getScenarioGUID() {
        return scenarioGUID;
    }

    public void setScenarioGUID(String scenarioGUID) {
        this.scenarioGUID = scenarioGUID;
    }
}

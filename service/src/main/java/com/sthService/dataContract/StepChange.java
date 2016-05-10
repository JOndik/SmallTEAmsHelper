package com.sthService.dataContract;

import org.springframework.data.mongodb.core.mapping.Document;

/**
 * class used for storing addition, change and deletion of scenario step
 */
@Document
public class StepChange extends ModelChange {

    private int status;
    private String scenarioGUID;
    private String stepGUID;
    private int position;
    private String stepType;
    private String name;
    private String uses;
    private String results;
    private String state;
    private String extensionGUID;
    private String joiningStepGUID;
    private String joiningStepPosition;

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

    public int getPosition() {
        return position;
    }

    public void setPosition(int position) {
        this.position = position;
    }

    public String getStepType() {
        return stepType;
    }

    public void setStepType(String stepType) {
        this.stepType = stepType;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getUses() {
        return uses;
    }

    public void setUses(String uses) {
        this.uses = uses;
    }

    public String getResults() {
        return results;
    }

    public void setResults(String results) {
        this.results = results;
    }

    public String getState() {
        return state;
    }

    public void setState(String state) {
        this.state = state;
    }

    public String getExtensionGUID() {
        return extensionGUID;
    }

    public void setExtensionGUID(String extensionGUID) {
        this.extensionGUID = extensionGUID;
    }

    public String getJoiningStepGUID() {
        return joiningStepGUID;
    }

    public void setJoiningStepGUID(String joiningStepGUID) {
        this.joiningStepGUID = joiningStepGUID;
    }

    public String getJoiningStepPosition() {
        return joiningStepPosition;
    }

    public void setJoiningStepPosition(String joiningStepPosition) {
        this.joiningStepPosition = joiningStepPosition;
    }

    public String getStepGUID() {
        return stepGUID;
    }

    public void setStepGUID(String stepGUID) {
        this.stepGUID = stepGUID;
    }
}

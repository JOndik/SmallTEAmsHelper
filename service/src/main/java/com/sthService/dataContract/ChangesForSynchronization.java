package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class ChangesForSynchronization {

    @Id
    private String id;

    @NotNull
    private List<String> changeIDs;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public List<String> getChangeIDs() {
        return changeIDs;
    }

    public void setChangeIDs(List<String> changeIDs) {
        this.changeIDs = changeIDs;
    }
}

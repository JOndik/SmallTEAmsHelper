package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class CorrespondenceModel {

    @Id
    private String id;

    @NotNull
    private List<CorrespondenceNode> correspondenceNodes;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public List<CorrespondenceNode> getCorrespondenceNodes() {
        return correspondenceNodes;
    }

    public void setCorrespondenceNodes(List<CorrespondenceNode> correspondenceNodes) {
        this.correspondenceNodes = correspondenceNodes;
    }
}

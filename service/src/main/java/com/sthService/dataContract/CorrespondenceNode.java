package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class CorrespondenceNode {

    @Id
    private String id;

    @NotNull
    private List<CorrNodePart> corrNodePartList;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public List<CorrNodePart> getCorrNodePartList() {
        return corrNodePartList;
    }

    public void setCorrNodePartList(List<CorrNodePart> corrNodePartList) {
        this.corrNodePartList = corrNodePartList;
    }
}

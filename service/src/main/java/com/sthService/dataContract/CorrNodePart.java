package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import java.util.List;

@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class CorrNodePart {

    @Id
    private String id;

    @NotNull
    private String userName;

    @NotNull
    private String elementGUID;

    public CorrNodePart(String userName, String elementGUID){
        this.userName = userName;
        this.elementGUID = elementGUID;
    }

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public String getElementGUID() {
        return elementGUID;
    }

    public void setElementGUID(String elementGUID) {
        this.elementGUID = elementGUID;
    }
}

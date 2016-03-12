package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnore;
import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
import com.sun.mail.imap.protocol.Item;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

import javax.persistence.*;
import javax.validation.constraints.NotNull;

@Document
@JsonTypeInfo(
        use = JsonTypeInfo.Id.NAME,
        include = JsonTypeInfo.As.PROPERTY,
        property = "classType"
)
@JsonSubTypes({
        @JsonSubTypes.Type(value = PropertyChange.class, name = "PropertyChange"),
        @JsonSubTypes.Type(value = ItemCreation.class, name = "ItemCreation"),
        @JsonSubTypes.Type(value = ScenarioChange.class, name = "ScenarioChange"),
        @JsonSubTypes.Type(value = StepChange.class, name = "StepChange")
})
@JsonIgnoreProperties(ignoreUnknown = true)
public class ModelChange {

    @Id
    @JsonIgnore
    private String id;

    @NotNull
    private String timestamp;

    @NotNull
    private String itemGUID;

    @NotNull
    private String modelGUID;

    @NotNull
    private int elementType;

    @JsonIgnore
    private String username;

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(String timestamp) {
        this.timestamp = timestamp;
    }

    public String getItemGUID() {
        return itemGUID;
    }

    public void setItemGUID(String itemGUID) {
        this.itemGUID = itemGUID;
    }

    public String getModelGUID() {
        return modelGUID;
    }

    public void setModelGUID(String modelGUID) {
        this.modelGUID = modelGUID;
    }

    public String getUsername() {
        return username;
    }

    public void setUsername(String username) {
        this.username = username;
    }

    public int getElementType() {
        return elementType;
    }

    public void setElementType(int elementType) {
        this.elementType = elementType;
    }
}

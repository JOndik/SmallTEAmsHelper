package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.*;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

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

    @NotNull
    private int elementDeleted;

    @JsonIgnore
    private String userName;

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

    public String getUserName() {
        return userName;
    }

    public void setUserName(String userName) {
        this.userName = userName;
    }

    public int getElementType() {
        return elementType;
    }

    public void setElementType(int elementType) {
        this.elementType = elementType;
    }

    public int getElementDeleted() {
        return elementDeleted;
    }

    public void setElementDeleted(int elementDeleted) {
        this.elementDeleted = elementDeleted;
    }
}

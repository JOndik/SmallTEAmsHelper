package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;
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
        @JsonSubTypes.Type(value = PropertyChange.class, name = "PropertyChange")
})
@JsonIgnoreProperties(ignoreUnknown = true)
public class ModelChange {

    @Id
    //@GeneratedValue(strategy = GenerationType.AUTO)
    private String id;

    @NotNull
    private String timestamp;

    @NotNull
    private String itemGUID;

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
}

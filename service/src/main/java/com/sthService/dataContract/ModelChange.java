package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.annotation.JsonSubTypes;
import com.fasterxml.jackson.annotation.JsonTypeInfo;

import javax.persistence.*;
import javax.validation.constraints.NotNull;

@Entity
@Table(name = "model_changes")
@Inheritance(strategy = InheritanceType.JOINED)
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
    @GeneratedValue(strategy = GenerationType.AUTO)
    private Long id;

    @NotNull
    private String timestamp;

    @NotNull
    private String itemGUID;

    public Long getId() {
        return id;
    }

    public void setId(Long id) {
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

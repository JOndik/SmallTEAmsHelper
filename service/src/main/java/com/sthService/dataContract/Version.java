package com.sthService.dataContract;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;

import javax.persistence.*;
import javax.validation.constraints.NotNull;
import org.springframework.data.annotation.*;
import org.springframework.data.mongodb.core.mapping.Document;

/**
 * class used for storing version of our EA Add-In
 */
@Document
@JsonIgnoreProperties(ignoreUnknown = true)
public class Version {

    @Id
    private String id;

    @NotNull
    private Double number;

    public Double getNumber() {
        return number;
    }

    public void setNumber(Double number) {
        this.number = number;
    }
}

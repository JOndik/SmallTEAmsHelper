package com.sthService.dataContract;

import org.springframework.data.mongodb.core.mapping.Document;

import javax.validation.constraints.NotNull;

@Document
public class PropertyChange extends ModelChange {

    @NotNull
    private int propertyType;

    @NotNull
    private String propertyBody;

    public int getPropertyType() {
        return propertyType;
    }

    public void setPropertyType(int propertyType) {
        this.propertyType = propertyType;
    }

    public String getPropertyBody() {
        return propertyBody;
    }

    public void setPropertyBody(String propertyBody) {
        this.propertyBody = propertyBody;
    }
}

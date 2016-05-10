package com.sthService.dataContract;

import org.springframework.data.mongodb.core.mapping.Document;

import javax.validation.constraints.NotNull;

/**
 * class used for storing changes of item properties
 */
@Document
public class PropertyChange extends ModelChange {

    @NotNull
    private int propertyType;

    @NotNull
    private String propertyBody;

    @NotNull
    private String oldPropertyBody;

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

    public String getOldPropertyBody() {
        return oldPropertyBody;
    }

    public void setOldPropertyBody(String oldPropertyBody) {
        this.oldPropertyBody = oldPropertyBody;
    }
}

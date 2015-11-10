package com.sthService.dataContract;

import javax.persistence.Entity;
import javax.persistence.Table;
import javax.validation.constraints.NotNull;

@Entity
@Table(name="property_changes")
public class PropertyChange extends ModelChange {

    @NotNull
    private String elementGUID;

    @NotNull
    private int propertyType;

    @NotNull
    private String propertyBody;

    public String getElementGUID() {
        return elementGUID;
    }

    public void setElementGUID(String elementGUID) {
        this.elementGUID = elementGUID;
    }

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

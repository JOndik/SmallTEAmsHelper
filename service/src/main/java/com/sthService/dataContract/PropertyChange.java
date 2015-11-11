package com.sthService.dataContract;

import javax.persistence.Entity;
import javax.persistence.PrimaryKeyJoinColumn;
import javax.persistence.Table;
import javax.validation.constraints.NotNull;

@Entity
@Table(name= "property_changes")
@PrimaryKeyJoinColumn(name = "model_changes_id", referencedColumnName = "id")
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

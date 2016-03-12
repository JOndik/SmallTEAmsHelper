package com.sthService.dataContract;

import org.springframework.data.mongodb.core.mapping.Document;

import javax.validation.constraints.NotNull;

@Document
public class ItemCreation extends ModelChange {

    @NotNull
    private String parentGUID;

    @NotNull
    private String author;

    @NotNull
    private String name;

    @NotNull
    private String packageGUID;

    @NotNull
    private String diagramGUID;

    @NotNull
    private String srcGUID;

    @NotNull
    private String targetGUID;

    @NotNull
    private String coordinates;

    public String getParentGUID() {
        return parentGUID;
    }

    public void setParentGUID(String parentGUID) {
        this.parentGUID = parentGUID;
    }

    public String getAuthor() {
        return author;
    }

    public void setAuthor(String author) {
        this.author = author;
    }

    public String getName() {
        return name;
    }

    public void setName(String name) {
        this.name = name;
    }

    public String getPackageGUID() {
        return packageGUID;
    }

    public void setPackageGUID(String packageGUID) {
        this.packageGUID = packageGUID;
    }

    public String getSrcGUID() {
        return srcGUID;
    }

    public void setSrcGUID(String srcGUID) {
        this.srcGUID = srcGUID;
    }

    public String getTargetGUID() {
        return targetGUID;
    }

    public void setTargetGUID(String targetGUID) {
        this.targetGUID = targetGUID;
    }

    public String getDiagramGUID() {
        return diagramGUID;
    }

    public void setDiagramGUID(String diagramGUID) {
        this.diagramGUID = diagramGUID;
    }

    public String getCoordinates() {
        return coordinates;
    }

    public void setCoordinates(String coordinates) {
        this.coordinates = coordinates;
    }
}

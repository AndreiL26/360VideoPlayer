using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionData 
{
    private Vector2 center;
    private Vector2 size;
    private string description;
    private float startTime;
    private float endTime;
    private GameObject regionPopUp;
    
    public RegionData() {  
    }
    public RegionData(float centerX, float centerY, float sizeX, float sizeY, string description, float startTime, float endTime) {
        this.center.x = centerX;
        this.center.y = centerY;
        this.size.x = sizeX;
        this.size.y = sizeY;
        this.description = description;
        this.startTime = startTime;
        this.endTime = endTime;
        this.regionPopUp = null;
    }

    public void setRegionCenter(Vector2 center) {
        this.center = center;
    }
    
    public void setRegionSize(Vector2 size) {
        this.size = size;
    }

    public void setRegionDescription(string description) {
        this.description = description;
    }

    public void setRegionStartTime(float startTime) {
        this.startTime = startTime;
    }

    public void setRegionEndTime(float endTime) {
        this.endTime = endTime;
    }

    public void setRegionPopUp(GameObject popUp) {
        this.regionPopUp = popUp;
    }

    public Vector2 getRegionCenter() {
        return this.center;
    }

    public Vector2 getRegionSize() {
        return this.size;
    }
    
    public string getRegionDescription() {
        return this.description;
    }

    public float getRegionStartTime() {
        return this.startTime;
    }

    public float getRegionEndTime() {
        return this.endTime;
    }

    public GameObject getRegionPopUp() {
        return this.regionPopUp;
    }
    
}

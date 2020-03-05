﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightMgr : MonoBehaviour
{
    [SerializeField] private Vector3 rayDir;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Material highlightMaterial;
    [SerializeField] private CanvasGroup popUpCanvasGroup;
    [SerializeField] private GameObject regionPopUpInfoCanvasPrefab;
    private float[] shaderRegionsData = new float[80];
    private float[] previewRegionData = new float [4];
    private List<RegionData> regionsData = new List<RegionData>();
    private int existingZones = 0;

    bool previewRegionEnabled = false;

    private class RegionData {
        public RegionData() {
            this.regionPopUp = null;
            this.lastTimeClicked = -1;
        }
        public RegionData(float centerX, float centerY, float sizeX, float sizeY, string text) {
            this.center.x = centerX;
            this.center.y = centerY;
            this.size.x   = sizeX;
            this.size.y  = sizeY;
            this.text = text;
            this.regionPopUp = null;
            this.lastTimeClicked = -1;
        }

        public Vector2 center;
        public Vector2 size;
        public string text;
        public GameObject regionPopUp;
        public float lastTimeClicked;
    }
    public struct PolarAngles {
        public PolarAngles(float x, float y) {
            this.azimuth = x;
            this.zenith = y;        
        }

        public PolarAngles(Vector2 pt) {
            azimuth = pt.x;
            zenith = pt.y;
        }

        public float azimuth;
        public float zenith;
    }

    private void Awake() {
        highlightMaterial.SetFloatArray("_RegionsData", shaderRegionsData);
        highlightMaterial.SetInt("_RegionsSize", 0);
    }

    public void ModifyPreviewValues(float centerX, float centerY, float sizeX, float sizeY) {
        previewRegionData[0] = centerX;
        previewRegionData[1] = centerY;
        previewRegionData[2] = sizeX;
        previewRegionData[3] = sizeY;
    }

    public Vector4 SetupPreviewRegion() {
        Vector4 ret = new Vector4();
        previewRegionEnabled = true;
        rayDir = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f)).direction;
        PolarAngles clickPosition = ToRadialCoords(rayDir);
        previewRegionData[0] = clickPosition.azimuth;
        previewRegionData[1] = clickPosition.zenith;
        previewRegionData[2] = 0.05f;
        previewRegionData[3] = 0.05f;
        ret.x = clickPosition.azimuth;
        ret.y = clickPosition.zenith;
        ret.z = 0.05f;
        ret.w = 0.05f;

        return ret;
    }

    public void StopPreviewRegion() {
        previewRegionEnabled = false;
    }

    public void AddPreviewRegionsToRegionsData() {
        RegionData regionData = new RegionData();
        regionData.center = new Vector2(previewRegionData[0],previewRegionData[1]);
        regionData.size = new Vector2(previewRegionData[2],previewRegionData[3]);
        regionsData.Add(regionData);
    }
    
    private void Update() {
        if(!mainCam)
            return;

        if(!previewRegionEnabled) {
            highlightMaterial.SetFloatArray("_RegionsData", shaderRegionsData);
            highlightMaterial.SetInt("_RegionsSize", shaderRegionsData.Length/4);

            if (Input.GetMouseButtonDown(0)) {
                rayDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

                PolarAngles clickPosition = ToRadialCoords(rayDir);
                RegionData hitRegion = CheckRegionHit(clickPosition);
                if (hitRegion != null) {
                    hitRegion.lastTimeClicked = Time.time;
                    if (GetRegionIndexFromShader(hitRegion) == -1) {
                        AddRegionToShader(hitRegion);
                    }
                    if (hitRegion.regionPopUp == null) {
                        Quaternion rot = Quaternion.LookRotation(rayDir, Vector3.up);
                        hitRegion.regionPopUp = Instantiate(regionPopUpInfoCanvasPrefab, rayDir * 5.0f, rot);
                        hitRegion.regionPopUp.GetComponent<ZonePopUpController>().EnablePopUp();
                    }
                    else {
                        if (hitRegion.regionPopUp.GetComponent<ZonePopUpController>().IsEnabled() == false) {
                            hitRegion.regionPopUp.GetComponent<ZonePopUpController>().EnablePopUp();
                        }
                    }
                }
            }
        } else  {
            highlightMaterial.SetFloatArray("_RegionsData", previewRegionData);
            highlightMaterial.SetInt("_RegionsSize", 1);
        }

        UpdateShaderRegions(Time.time);
    }

    private RegionData CheckRegionHit(PolarAngles position) {
        foreach(RegionData region in regionsData) {
            if ((Mathf.Abs(position.azimuth - region.center.x) < Mathf.Abs(region.size.x)) &&
               (Mathf.Abs(position.zenith - region.center.y) < Mathf.Abs(region.size.y))) {
                return region;
            }
        }
        return null;
    }

    private int GetRegionIndexFromShader(RegionData region) {
        /// Might want to consider not allowing 'Overlapping regions (i.e. now u can have same center different sizes, what happens then?'
        for(int i = 0; i < existingZones; ++ i) {
            if(shaderRegionsData[i * 4] == region.center.x && shaderRegionsData[i * 4 + 1] == region.center.y && 
               shaderRegionsData[i * 4 + 2] == region.size.x && shaderRegionsData[i * 4 + 3] == region.size.y) {
                return i;
            }
        }
        return -1;
    }

    private void UpdateShaderRegions(float currentTime) {
        foreach(RegionData region in regionsData) {
            if(region.lastTimeClicked != -1 && currentTime - region.lastTimeClicked > 10.0f) {
                RemoveRegionFromShader(region);
            }
        }
    }

    private void RemoveRegionFromShader(RegionData region) {
        int ix = GetRegionIndexFromShader(region);
        if (ix == -1) {
            return;
        }
        if (existingZones == 1) {
            shaderRegionsData[0] = shaderRegionsData[1] = shaderRegionsData[2] = shaderRegionsData[3] = 0;
        }
        else {
            for (int i = ix; i < existingZones - 1; ++i) {
                shaderRegionsData[ix * 4] = shaderRegionsData[(ix + 1) * 4];
                shaderRegionsData[ix * 4 + 1] = shaderRegionsData[(ix + 1) * 4 + 1];
                shaderRegionsData[ix * 4 + 2] = shaderRegionsData[(ix + 1) * 4 + 2];
                shaderRegionsData[ix * 4 + 3] = shaderRegionsData[(ix + 1) * 4 + 3];
            }
        }
        existingZones--;
    }
    
    private void AddRegionToShader(RegionData regionData) {
        shaderRegionsData[existingZones * 4]     = (regionData.center.x);
        shaderRegionsData[existingZones * 4 + 1] = (regionData.center.y);
        shaderRegionsData[existingZones * 4 + 2] = (regionData.size.x);
        shaderRegionsData[existingZones * 4 + 3] = (regionData.size.y);
        existingZones = existingZones + 1;
        highlightMaterial.SetInt("_RegionsSize", existingZones);
    }

    public PolarAngles ToRadialCoords(Vector3 coords) {
        float pi = 3.14159265359f;
        coords.Normalize();
        float latitude = Mathf.Acos(coords.y);
        float longitude = Mathf.Atan2(coords.z, coords.x);
        return new PolarAngles(new Vector2(0.5f - longitude * (0.5f / pi), 1.0f - latitude * (1.0f / pi)));
    }
}


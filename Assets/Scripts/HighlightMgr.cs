using System;
using System.Collections;
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
    [SerializeField] private VideoController videoController;
    private float[] shaderRegionsData = new float[80];                //float array containing the details of all 'active' zones, 
                                                                      //i.e. the zones that are currently being highlighted by the shader
                                                                      //must be an array of floats because of how shaders can receive data!
    private float[] previewRegionData = new float [4];                //float array containing the details of the preview zone
    private List<RegionData> allRegionsData = new List<RegionData>(); //List of RegionData to hold the details of all regions,
                                                                      // i.e. even regions that are not yet active but will be when the time in the 
                                                                      //video is greater than the zones startTime
    private int existingZones = 0;

    bool previewRegionEnabled = false;

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
        Vector4 defaultZoneDetails = new Vector4();
        previewRegionEnabled = true;
        rayDir = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0.0f)).direction; // Perform a raycast in the camera orientation so that the preview zone is always centered to the user's view
        PolarAngles clickPosition = ToRadialCoords(rayDir); 
        previewRegionData[0] = clickPosition.azimuth;   // Assign the coordinates of the previewRegion to be the result of the raycast, i.e.
        previewRegionData[1] = clickPosition.zenith;    // asssigning these 2 variables makes the zone be in front of the camera, so that users always see the region
        previewRegionData[2] = 0.05f;                   // Default value for the X Size
        previewRegionData[3] = 0.05f;                   // Default value for the Y Size
        defaultZoneDetails.x = clickPosition.azimuth;
        defaultZoneDetails.y = clickPosition.zenith;
        defaultZoneDetails.z = 0.05f;
        defaultZoneDetails.w = 0.05f;

        return defaultZoneDetails;
    }

    public void StopPreviewRegion() {
        previewRegionEnabled = false;
    }


    public void RemoveAllRegionsFromShader() {
        foreach (RegionData region in allRegionsData) {
            RemoveRegionFromShader(region);
        }
    }

    public void AddPreviewRegionsToRegionsData(RegionData newRegion) {
        allRegionsData.Add(newRegion);
    }
    
    private void Update() {
        if(!mainCam)
            return;

        if(!previewRegionEnabled) {
            highlightMaterial.SetFloatArray("_RegionsData", shaderRegionsData);    // Every frame send the active regions data to the shader, so that the highlight is visible!
            highlightMaterial.SetInt("_RegionsSize", shaderRegionsData.Length/4);

            if (Input.GetMouseButtonDown(0)) {
                // Check if the user clicked on a zone and update the pop-up accordingly
                rayDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction; 

                PolarAngles clickPosition = ToRadialCoords(rayDir);
                RegionData hitRegion = CheckRegionHit(clickPosition);
                if (hitRegion != null) {
                    if (GetRegionIndexFromShader(hitRegion) != -1) {
                        Quaternion rot = Quaternion.LookRotation(rayDir, Vector3.up);
                        if (hitRegion.getRegionPopUp() == null) {
                            hitRegion.setRegionPopUp(Instantiate(regionPopUpInfoCanvasPrefab, rayDir * 5.0f, rot));
                            hitRegion.getRegionPopUp().GetComponentInChildren<Text>().text = hitRegion.getRegionDescription();
                            hitRegion.getRegionPopUp().GetComponent<ZonePopUpController>().EnablePopUp();
                        }
                        else {
                            if (hitRegion.getRegionPopUp().GetComponent<ZonePopUpController>().IsEnabled() == false) {
                                hitRegion.getRegionPopUp().transform.position = rayDir * 5.0f;
                                hitRegion.getRegionPopUp().transform.rotation = rot;
                                hitRegion.getRegionPopUp().GetComponent<ZonePopUpController>().EnablePopUp();
                            }
                        }
                    }
                }
            }
        } else  {
            highlightMaterial.SetFloatArray("_RegionsData", previewRegionData);
            highlightMaterial.SetInt("_RegionsSize", 1);
        }

        UpdateShaderRegions();
    }

    private RegionData CheckRegionHit(PolarAngles position) {
        foreach(RegionData region in allRegionsData) {
            if ((Mathf.Abs(position.azimuth - region.getRegionCenter().x) < Mathf.Abs(region.getRegionSize().x)) &&
               (Mathf.Abs(position.zenith - region.getRegionCenter().y) < Mathf.Abs(region.getRegionSize().y))) {
                return region;
            }
        }
        return null;
    }

    private int GetRegionIndexFromShader(RegionData region) {
        // Checks if the region is 'active' and returns the index in the float array at which the data of the region begins, or - 1 if the zone is not active
        for(int i = 0; i < existingZones; ++ i) {
            if(shaderRegionsData[i * 4] == region.getRegionCenter().x && shaderRegionsData[i * 4 + 1] == region.getRegionCenter().y && 
               shaderRegionsData[i * 4 + 2] == region.getRegionSize().x && shaderRegionsData[i * 4 + 3] == region.getRegionSize().y) {
                return i;
            }
        }
        return -1;
    }


    private void UpdateShaderRegions() {
        // Updates the active zones based on the timestamps of each region
        foreach(RegionData region in allRegionsData) {
            if(region.getRegionEndTime() <= videoController.Time) {
                RemoveRegionFromShader(region);
            }
            else if (region.getRegionStartTime() < videoController.Time && GetRegionIndexFromShader(region) == -1) {
                AddRegionToShader(region);
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
        shaderRegionsData[existingZones * 4]     = (regionData.getRegionCenter().x);
        shaderRegionsData[existingZones * 4 + 1] = (regionData.getRegionCenter().y);
        shaderRegionsData[existingZones * 4 + 2] = (regionData.getRegionSize().x);
        shaderRegionsData[existingZones * 4 + 3] = (regionData.getRegionSize().y);
        existingZones += 1;
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


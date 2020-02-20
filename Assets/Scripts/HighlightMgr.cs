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
    [SerializeField] private Text popUpText;
    [SerializeField] private Text zoneCenterXText;
    [SerializeField] private Text zoneCenterYText;

    [Range(0.0f, 1.0f)] public float zoneCenterX = 0.25f;
    [Range(0.0f, 1.0f)] public float zoneCenterY = 0.5f;
    private float[] highlightedRegionsData = new float[80];
    private List<RegionData> regionsData = new List<RegionData>();
    private int existingZones = 0;

    private class RegionData
    {
        public RegionData(float centerX, float centerY, float sizeX, float sizeY, string text) {
            this.center.x = centerX;
            this.center.y = centerY;
            this.size.x   = sizeX;
            this.size.y  = sizeY;
            this.text = text;
        }
        public Vector2 center;
        public Vector2 size;
        public string text;
        //float minTime;
        //float maxTime;
    }
    public struct PolarAngles
    {
        public PolarAngles(float x, float y)
        {
            this.azimuth = x;
            this.zenith = y;        
        }

        public PolarAngles(Vector2 pt)
        {
            azimuth = pt.x;
            zenith = pt.y;
        }

        public float azimuth;
        public float zenith;
    }

    private void Awake() {
       // regionsData.Add(new RegionData(zoneCenterX, zoneCenterY, 0.1f, 0.05f, "Hello, I am Zone Number 1!"));
        regionsData.Add(new RegionData(zoneCenterX + 0.5f, zoneCenterY, 0.1f, 0.05f, "Hello, I am Zone Number 2!"));
        highlightMaterial.SetFloatArray("_RegionsData", highlightedRegionsData);
        highlightMaterial.SetInt("_RegionsSize", 0);

        HidePopUp();
    }
    
    private void AddRegionData(float centerX, float centerY, float sizeX, float sizeY, string text) {
        RegionData newRegion = new RegionData(centerX, centerY, sizeX, sizeY, text);
        regionsData.Add(newRegion);
        AddRegionToShader(newRegion);
    }

    public void AddRegionDataFromButton() {
        float zoneX = float.Parse(zoneCenterXText.text);
        float zoneY = float.Parse(zoneCenterYText.text);
       
        AddRegionData(zoneX, zoneY, 0.1f, 0.05f, "HELLO I AM ADDED ZONE HAHAA");
    }
    
    private void AddRegionToShader(RegionData regionData) {
        highlightedRegionsData[existingZones * 4] = (regionData.center.x);
        highlightedRegionsData[existingZones * 4 + 1] = (regionData.center.y);
        highlightedRegionsData[existingZones * 4 + 2] = (regionData.size.x);
        highlightedRegionsData[existingZones * 4 + 3] = (regionData.size.y);
        existingZones = existingZones + 1;
        highlightMaterial.SetInt("_RegionsSize", existingZones);

    }


    public void ShowPopUp() {
        popUpCanvasGroup.alpha = 1;
        popUpCanvasGroup.interactable = true;
    }

    public void HidePopUp() {
        popUpCanvasGroup.interactable = false;
        popUpCanvasGroup.alpha = 0;
    }

   
    private void Update()
    {
        if(!mainCam)
            return;

        //for all curr regions 
        //create regions data
        highlightMaterial.SetFloatArray("_RegionsData", highlightedRegionsData);

        if (Input.GetMouseButtonDown(0)) {
            rayDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

            PolarAngles clickPosition = ToRadialCoords(rayDir);
            RegionData hitRegion = CheckRegionHit(clickPosition);
            if (hitRegion != null) {
               AddRegionToShader(hitRegion);
                if(popUpCanvasGroup.alpha == 0) {
                    ShowPopUp();
                }
                popUpText.text = hitRegion.text;
            }
            //GetSelectedRegions(polarAng)
            //for each region check if polarang is in region
            //if region found then add to list
        }
    }

    private RegionData CheckRegionHit(PolarAngles position) {
        foreach(RegionData region in regionsData) {
            if ((Mathf.Abs(position.azimuth - region.center.x) < Mathf.Abs(region.size.x)) &&
               (Mathf.Abs(position.zenith - region.center.y) < Mathf.Abs(region.size.y))) {
                Debug.Log("I HAVE HIT A REGION ");
                return region;
            }
        }
        return null;
    }


    public PolarAngles ToRadialCoords(Vector3 coords) {
        float pi = 3.14159265359f;
        coords.Normalize();
        float latitude = Mathf.Acos(coords.y);
        float longitude = Mathf.Atan2(coords.z, coords.x);
        return new PolarAngles(new Vector2(0.5f - longitude * (0.5f / pi), 1.0f - latitude * (1.0f / pi)));
    }
}


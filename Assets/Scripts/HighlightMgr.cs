using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightMgr : MonoBehaviour
{
    [SerializeField] private Vector3 rayDir;
    [SerializeField] private Camera mainCam;
    [SerializeField] private Material highlightMaterial;

    [Range(0.0f, 1.0f)] public float zoneCenterX = 0.25f;
    [Range(0.0f, 1.0f)] public float zoneCenterY = 0.5f;

    private struct RegionData
    {
        public RegionData(float centerX, float centerY, float sizeX, float sizeY) {
            center.x = centerX;
            center.y = centerY;
            size.x   = sizeX;
            size.y  = sizeY;
        }
        public Vector2 center;
        public Vector2 size;
        //float minTime;
        //float maxTime;
    }
    private List<float> highlightedRegionsData = new List<float>();
    private List<RegionData> regionsData = new List<RegionData>();
    //List of all regions
    //List of current regions
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
        regionsData.Add(new RegionData(zoneCenterX, zoneCenterY, 0.1f, 0.05f));
        regionsData.Add(new RegionData(zoneCenterX + 0.5f, zoneCenterY, 0.1f, 0.05f));
        highlightedRegionsData.Add(zoneCenterX);
        highlightedRegionsData.Add(zoneCenterY);
        highlightedRegionsData.Add(0.1f);
        highlightedRegionsData.Add(0.05f);

        highlightedRegionsData.Add(zoneCenterX + 0.5f);
        highlightedRegionsData.Add(zoneCenterY);
        highlightedRegionsData.Add(0.1f);
        highlightedRegionsData.Add(0.05f);

        highlightMaterial.SetInt("_RegionsSize", 2);
        highlightMaterial.SetFloatArray("_RegionsData", highlightedRegionsData);
    }
    void Update()
    {
        if(!mainCam)
            return;

        //for all curr regions 
        //create regions data


        if (Input.GetMouseButtonDown(0)) {
            //Ray Direction is Camera Forward for the moment
            //rayDir = mainCam.transform.forward;
            rayDir = Camera.main.ScreenPointToRay(Input.mousePosition).direction;

            //Convert from Cartesian Coordinates to Radial
            PolarAngles clickPosition = ToRadialCoords(rayDir);
            RegionData hitRegion = RegionHit(clickPosition);
            //GetSelectedRegions(polarAng)
            //for each region check if polarang is in region
            //if region found then add to list
        }
    }

    private RegionData RegionHit(PolarAngles position) {
        foreach(RegionData region in regionsData) {
            if ((Mathf.Abs(position.azimuth - region.center.x) < Mathf.Abs(region.size.x)) &&
               (Mathf.Abs(position.zenith - region.center.y) < Mathf.Abs(region.size.y))) {
                Debug.Log("X OF CLICK " + position.azimuth);
                Debug.Log("Y OF CLICK " + position.zenith);
                Debug.Log("Region Center X " + region.center.x);
                Debug.Log("Region Center Y " + region.center.y);
                Debug.Log("CLICKED INSIDE A ZONE!");
                return region;
            }
        }
        return regionsData[0];
    }


    public PolarAngles ToRadialCoords(Vector3 coords)
    {
        float pi = 3.14159265359f;
        coords.Normalize();
        float latitude = Mathf.Acos(coords.y);
        float longitude = Mathf.Atan2(coords.z, coords.x);
        return new PolarAngles(new Vector2(0.5f - longitude * (0.5f / pi), 1.0f - latitude * (1.0f / pi)));
    }
}

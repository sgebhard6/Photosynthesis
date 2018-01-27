﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTransmitter : MonoBehaviour {

    public GameObject lightObject;
    public LineRenderer lightRayRenderer;
    public int maxRays = 5;
    public float maxDistance = 150f;

    public LayerMask layerMask;

    int currentRayCount = 0;
    //List<Ray> rayList = new List<Ray>();
    List<LightBeam> rayList = new List<LightBeam>();

    Vector3 newDir;

    Ray lightRay;

    private void Start()
    {
        Reset();
    }

    private void Reset()
    {
        lightRay = new Ray(lightObject.transform.position, lightObject.transform.right);
        currentRayCount = 0;
        rayList.Clear();
    }

    void Update () {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Reset();
            TransmitLight(lightRay);
        }

        DrawRays();
	}

    void TransmitLight(Ray _ray)
    {
        if (currentRayCount < maxRays)
        {
            currentRayCount++;
            
            RaycastHit2D hit = Physics2D.Raycast(_ray.origin, _ray.direction, maxDistance, layerMask);
            //rayList.Add(lightRay);
            rayList.Add(new LightBeam(lightRay, hit.distance));

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Reflective"))
                {
                    newDir = Vector3.Reflect(_ray.direction, hit.normal);
                    lightRay.origin = hit.point + hit.normal * 0.01f;
                    lightRay.direction = newDir;
                    TransmitLight(lightRay);
                }
            }
        }
    }

    void DrawRays()
    {
        if (rayList.Count < 1) return;
        lightRayRenderer.positionCount = rayList.Count + 1;
        Vector3[] points = new Vector3[rayList.Count + 1];
        for(int i = 0; i < points.Length; i++)
        {
            if (i < rayList.Count)
            {
                points[i] = rayList[i].beam.origin;
                //Debug.DrawRay(rayList[i].beam.origin, rayList[i].beam.direction * 20, Color.green);
            }
            else
                points[i] = rayList[i - 1].beam.origin + (rayList[i - 1].beam.direction * rayList[i - 1].distance);
            
        }

        lightRayRenderer.SetPositions(points);
    }
}

public class LightBeam
{
    public Ray beam;
    public float distance;

    public LightBeam(Ray _beam, float _distance)
    {
        beam = _beam;
        distance = _distance;
    }
}
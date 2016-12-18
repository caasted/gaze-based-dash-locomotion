﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeGroundTargeting : MonoBehaviour {

    // This script is intended to be paired with GazeBasedDashLocomotion.cs.
    // It provides a target at the point where the user's gaze hits the terrain.
    // Made by Chris Aasted
    // Last updated 2016-12-18

    public float maxRange = 10.0f;
    public GameObject target = null;

    // Use this for initialization
    void Start () {
        if (target == null)
        {
            target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.transform.parent = this.transform;
            target.transform.position = new Vector3(target.transform.position.x, 0.2f, target.transform.position.z);
            target.transform.localPosition = Vector3.zero;
        }
    }
	
	// Update is called once per frame
	void Update () {
        Ray ray = new Ray(this.transform.position, this.transform.forward);

        bool hasGroundTarget = false;
        RaycastHit hitInfo;
        TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        hasGroundTarget = tc.Raycast(ray, out hitInfo, maxRange);

        if (hasGroundTarget)
        {
            target.transform.position = hitInfo.point;
            target.SetActive(true);
        } else
        {
            target.SetActive(false);
        }
    }
}

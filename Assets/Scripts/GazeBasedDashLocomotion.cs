using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeBasedDashLocomotion : MonoBehaviour {

    // This script is largely a modification of SteamVR_Teleporter.cs from the SteamVR Plugin by Valve Corporation.
    // Instead of teleportation, the player's position is altered at a constant speed.
    // Instead of indicating direction with a controller, the player's head orientation is used.
    // Made by Chris Aasted
    // Last updated 2016-12-18

    public float maxRange = 10.0f;
    public float movementSpeed = 10.0f;
    float speedFactor = 3.2f; // To make movementSpeed approximately meters per second

    public GameObject trackedHMD = null;
    Vector3 destination;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    void Start()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.PadClicked += new ClickedEventHandler(SetDestination);

        if (trackedHMD == null)
        {
            trackedHMD = SteamVR_Render.Top().gameObject;
        }

        // Start the player at the level of the terrain
        var player = reference;
        if (player != null)
            player.position = new Vector3(player.position.x, Terrain.activeTerrain.SampleHeight(player.position), player.position.z);
    }

    void SetDestination(object sender, ClickedEventArgs e)
    {
        Ray ray = new Ray(trackedHMD.transform.position, trackedHMD.transform.forward);

        bool hasGroundTarget = false;
        RaycastHit hitInfo;
        TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        hasGroundTarget = tc.Raycast(ray, out hitInfo, maxRange);

        if (hasGroundTarget)
        {
            Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
            destination = hitInfo.point - headPosOnGround;
        }
    }

    // Update is called once per frame
    void Update () {
        var player = reference;
        if (player == null)
            return;

        Vector3 direction = destination - player.position;
        if (direction.magnitude > 0f) // Avoid NaN errors
        {
            // If the step size is greater than the distance, teleport, otherwise step towards destination
            if (direction.magnitude > 2 * Time.fixedDeltaTime * movementSpeed * speedFactor)
                player.position = player.position + (direction / direction.magnitude) * Time.fixedDeltaTime * movementSpeed * speedFactor;
            else
                player.position = destination;
        }
    }
}

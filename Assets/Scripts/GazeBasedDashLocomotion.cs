using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeBasedDashLocomotion : MonoBehaviour {

    // This script is largely a modification of SteamVR_Teleporter.cs from the SteamVR Plugin by Valve Corporation

    public float movementSpeed = 10.0f;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

    Vector3 destination;

    void Start()
    {
        var trackedController = GetComponent<SteamVR_TrackedController>();
        if (trackedController == null)
        {
            trackedController = gameObject.AddComponent<SteamVR_TrackedController>();
        }

        trackedController.PadClicked += new ClickedEventHandler(SetDestination);

        // Start the player at the level of the terrain
        var player = reference;
        if (player != null)
            player.position = new Vector3(player.position.x, Terrain.activeTerrain.SampleHeight(player.position), player.position.z);
    }

    void SetDestination(object sender, ClickedEventArgs e)
    {
        var player = reference;
        if (player == null)
            return;

        float refY = player.position.y;

        Ray ray = new Ray(this.transform.position, transform.forward);

        bool hasGroundTarget = false;
        float dist = 0f;
        RaycastHit hitInfo;
        TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        hasGroundTarget = tc.Raycast(ray, out hitInfo, 1000f);
        dist = hitInfo.distance;

        if (hasGroundTarget)
        {
            Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
            destination = ray.origin + ray.direction * dist - new Vector3(player.GetChild(0).localPosition.x, 0f, player.GetChild(0).localPosition.z) - headPosOnGround;
        }
    }

    // Update is called once per frame
    void Update () {
        var player = reference;
        if (player == null)
            return;

        if (destination == null)
            return;

        player.position = player.position + (destination - player.position) * Time.fixedDeltaTime * movementSpeed;

	}
}

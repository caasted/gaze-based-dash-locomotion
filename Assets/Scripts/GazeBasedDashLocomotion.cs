using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeBasedDashLocomotion : MonoBehaviour {

    // This script is largely a modification of SteamVR_Teleporter.cs from the SteamVR Plugin by Valve Corporation

    public bool teleportOnClick = false;

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

        // trackedController.TriggerClicked += new ClickedEventHandler(DoClick);
        trackedController.PadClicked += new ClickedEventHandler(DoClick);

        // Start the player at the level of the terrain
        var t = reference;
        if (t != null)
            t.position = new Vector3(t.position.x, Terrain.activeTerrain.SampleHeight(t.position), t.position.z);
    }

    void DoClick(object sender, ClickedEventArgs e)
    {
        if (teleportOnClick)
        {
            var t = reference;
            if (t == null)
                return;

            float refY = t.position.y;

            Plane plane = new Plane(Vector3.up, -refY);
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
                t.position = ray.origin + ray.direction * dist - new Vector3(t.GetChild(0).localPosition.x, 0f, t.GetChild(0).localPosition.z) - headPosOnGround;
            }
        }
    }

    // Update is called once per frame
    void Update () {
		
	}
}

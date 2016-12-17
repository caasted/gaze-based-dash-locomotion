using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GazeGroundTargetting : MonoBehaviour {

    public float maxRange = 10.0f;
    public GameObject target = null;

    Transform reference
    {
        get
        {
            var top = SteamVR_Render.Top();
            return (top != null) ? top.origin : null;
        }
    }

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
        var player = reference;
        if (player == null)
            return;

        Ray ray = new Ray(this.transform.position, this.transform.forward);

        bool hasGroundTarget = false;
        float dist = 0f;
        RaycastHit hitInfo;
        TerrainCollider tc = Terrain.activeTerrain.GetComponent<TerrainCollider>();
        hasGroundTarget = tc.Raycast(ray, out hitInfo, maxRange);
        dist = hitInfo.distance;

        if (hasGroundTarget)
        {
            Vector3 headPosOnGround = new Vector3(SteamVR_Render.Top().head.localPosition.x, 0.0f, SteamVR_Render.Top().head.localPosition.z);
            target.transform.position = ray.origin + ray.direction * dist - new Vector3(player.GetChild(0).localPosition.x, 0f, player.GetChild(0).localPosition.z) - headPosOnGround;
            target.SetActive(true);
        } else
        {
            target.SetActive(false);
        }
    }
}

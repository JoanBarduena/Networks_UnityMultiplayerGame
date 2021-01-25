using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using UnityEngine;
using Photon.Pun;

public class BoxSpawner : MonoBehaviour
{
    public enum PowerUp
    {
        NONE = 0,
        SPEED,
        FIRERATE,
        HEALTH
    }

    public PowerUp powerup = PowerUp.NONE;
    
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void SpawnBox()
    {
        Vector3 pos = new Vector3(this.transform.position.x, -0.13f, this.transform.position.z);
        GameObject obj = PhotonNetwork.InstantiateRoomObject("Map/Box", pos, Quaternion.identity);
        NavMeshObstacle obs = obj.AddComponent<NavMeshObstacle>();
        obs.carving = true;
        obs.carveOnlyStationary = true;
    }
}

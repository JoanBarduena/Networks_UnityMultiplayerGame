using System.Collections;
using System.Collections.Generic;
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
        PhotonNetwork.Instantiate("Map/Box", this.transform.position, Quaternion.identity);
    }
}

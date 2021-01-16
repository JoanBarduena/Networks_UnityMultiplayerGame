using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TankSpawner : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<MeshRenderer>().enabled = false;
    }

    public void SpawnTank()
    {
        PhotonNetwork.Instantiate("PhotonPrefabs/Tank", this.transform.position, Quaternion.identity);
        Destroy(this.gameObject);
    }
}

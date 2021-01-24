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

    public void SpawnTank(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankBlue", this.transform.position, Quaternion.Euler(0, 180f, 0));
                break;
            case 2:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankRed", this.transform.position, Quaternion.identity);
                break;
            case 3:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankYellow", this.transform.position, Quaternion.identity);
                break;
            case 4:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/TankGreen", this.transform.position, Quaternion.identity);
                break;
        }
        Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManagerCS : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateController()
    {
        int i = PhotonNetwork.LocalPlayer.ActorNumber;

        GameObject spawn= GameObject.Find("TankSpawn1");
        switch (i)
        {
            case 1:
                spawn = GameObject.Find("TankSpawn1");
                break;
            case 2:
                spawn = GameObject.Find("TankSpawn2");
                break;
            case 3:
                spawn = GameObject.Find("TankSpawn3");
                break;
            case 4:
                spawn = GameObject.Find("TankSpawn4");
                break;

            default:
                break;
        }
        spawn.gameObject.GetComponent<TankSpawner>().SpawnTank(i);
    }
}

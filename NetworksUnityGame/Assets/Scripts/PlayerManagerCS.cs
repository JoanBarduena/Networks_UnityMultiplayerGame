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
        var players = PhotonNetwork.CurrentRoom.PlayerCount;

        GameObject spawn= GameObject.Find("TankSpawn1");
        switch (players)
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
        spawn.gameObject.GetComponent<TankSpawner>().SpawnTank(players);



        //Debug.Log(spawn);

        //    if (spawn)
        //        spawn.gameObject.GetComponent<TankSpawner>().SpawnTank(1);
        //    else if (PV.GetInstanceID() == 2)
        //        GameObject.Find("TankSpawn2").gameObject.GetComponent<TankSpawner>().SpawnTank(2);
        //
    }
}

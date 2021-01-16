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
        GameObject spawn = GameObject.Find("TankSpawn1");

        Debug.Log(spawn);

        if (spawn)
            spawn.gameObject.GetComponent<TankSpawner>().SpawnTank();
        else if (PV.GetInstanceID() == 2)
            GameObject.Find("TankSpawn2").gameObject.GetComponent<TankSpawner>().SpawnTank();
    }
}

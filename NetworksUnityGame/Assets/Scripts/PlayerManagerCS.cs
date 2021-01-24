using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManagerCS : MonoBehaviour
{
    PhotonView PV;

    GameManager GM;

    public bool dead = false;
    public int killer;
    int pos = -1;

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
        if (GM == null)
            GM = GameObject.Find("GameManager(Clone)").GetComponent<GameManager>();

        // Spectate
        if (dead)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                killer++;
                if (killer > 4)
                    killer = 1;

                int start = killer;
                while (GM.IsPlayerAlive(killer) == false) //alive
                {
                    killer++;
                    if (killer > 4)
                        killer = 1;

                    if (killer == start) //full cycle
                        return;
                }
                Debug.Log(start + "-" + killer);
                GameObject player = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.GetPlayer(killer).ActorNumber).TagObject as GameObject;
                Camera.main.GetComponent<FollowCamera>().target = player.transform;
            }
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                killer--;
                if (killer < 1)
                    killer = 4;

                int start = killer;
                while (GM.IsPlayerAlive(killer) == false) //alive
                {
                    killer--;
                    if (killer < 1)
                        killer = 4;

                    if (killer == start) //full cycle
                        return;
                }
                Debug.Log(start + "-" + killer);
                GameObject player = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.CurrentRoom.GetPlayer(killer).ActorNumber).TagObject as GameObject;
                Camera.main.GetComponent<FollowCamera>().target = player.transform;
            }
        }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WinScene : MonoBehaviourPun
{

    int winner = 0;
    string winner_name;
    double StartedTime = 0;

    bool loading = false;

    public Transform win_pos;

    private void Awake()
    {
        StartedTime = PhotonNetwork.Time;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject room = GameObject.Find("RoomManager");
        winner = room.GetComponent<RoomManager>().winner;
        winner_name = PhotonNetwork.CurrentRoom.GetPlayer(winner).NickName;

        GameObject tank = GetWinnerTank(winner);
        Vector3 pos = Vector3.zero;
        pos.x = win_pos.position.x;
        pos.y = 1.8f;
        pos.z = win_pos.position.z;

        tank.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        double time = PhotonNetwork.Time - StartedTime;

        Debug.Log(winner);

        if (time > 10 && PhotonNetwork.IsMasterClient && !loading/*&& all players accept rematch*/)
        {
            loading = true;
            this.photonView.RPC("Rematch", RpcTarget.All);
        }
    }

    [PunRPC]
    void Rematch()
    {
        PhotonNetwork.LoadLevel("SampleScene"); //restart the game
    }

    GameObject GetWinnerTank(int num)
    {
        GameObject tank = GameObject.Find("TankFree_Blue"); ;
        switch (num)
        {
            case 1:
                tank = GameObject.Find("TankFree_Blue");
                break;
            case 2:
                tank = GameObject.Find("TankFree_Red");
                break;
            case 3:
                tank = GameObject.Find("TankFree_Yel");
                break;
            case 4:
                tank = GameObject.Find("TankFree_Green");
                break;
        }

        return tank;
    }
}

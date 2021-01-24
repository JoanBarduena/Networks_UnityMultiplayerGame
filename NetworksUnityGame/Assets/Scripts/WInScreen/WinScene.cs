using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WinScene : MonoBehaviourPun, IPunObservable
{

    int winner = 0;
    string winner_name;
    double StartedTime = 0;
    bool loading = false;

    [SerializeField] bool[] Players_rematch = { false, false, false, false };
    public int Players_connected = 0;

    private void Awake()
    {
        StartedTime = PhotonNetwork.Time;
        
    }

    // Start is called before the first frame update
    void Start()
    {
        Players_connected = PhotonNetwork.PlayerList.Length;

        GameObject room = GameObject.Find("RoomManager");
        winner = room.GetComponent<RoomManager>().winner;
        winner_name = PhotonNetwork.CurrentRoom.GetPlayer(winner).NickName;

        GameObject text_name = GameObject.Find("Name");
        text_name.GetComponent<Text>().text = winner_name;


        GameObject tank = GetWinnerTank(winner);
        Vector3 pos = Vector3.zero;
        pos.x = 2.64f;
        pos.y = 1.8f;
        pos.z = -8.82f;

        tank.transform.position = pos;
    }

    // Update is called once per frame
    void Update()
    {
        double time = PhotonNetwork.Time - StartedTime;

        if (AllPlayersRematch() && PhotonNetwork.IsMasterClient && !loading)
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

    bool AllPlayersRematch()
    {
        bool ret = true;

        for (int i = 0; i < Players_connected; i++)
        {
            if (!Players_rematch[i])
            {
                return false;
            }
        }

        return ret;
    }

    public void OnClickRematch(int num)
    {
        this.photonView.RequestOwnership();
        Players_rematch[num - 1] = true;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Players_rematch);
        }
        else
        {
            Players_rematch = (bool[])stream.ReceiveNext();
        }

    }
}

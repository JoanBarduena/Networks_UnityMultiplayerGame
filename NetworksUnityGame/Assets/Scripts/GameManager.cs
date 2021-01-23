using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable
{
    int winner = 0;
    bool win = false;
    bool change_sceen = false;

    [SerializeField] bool[] players_alive = { false, false, false, false };
    public int PlayersRemaining = 0;

    [SerializeField] bool CountDown = true;
    double StartTime = 0;
    double WinTime = 0;

    GameObject PlayersIcon;
    Text PlayersText;
    GameObject CD_Text;



    // Start is called before the first frame update
    void Start()
    {
        StartTime = PhotonNetwork.Time;

        CD_Text = GameObject.Find("CountDownText");

        PlayersIcon = GameObject.Find("PlayersIcon");
        PlayersRemaining = PhotonNetwork.PlayerList.Length;

        for(int i = 0; i < PlayersRemaining; i++)
        {
            players_alive[i] = true;
        }

        PlayersText = PlayersIcon.GetComponentInChildren<Text>();
        PlayersText.text = PlayersRemaining.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        double time = PhotonNetwork.Time - StartTime;
        if (CountDown)
        {
            if (time > 1 && time < 2)
                CD_Text.GetComponent<Text>().text = "3";
            else if (time > 2 && time < 3)
                CD_Text.GetComponent<Text>().text = "2";
            else if (time > 3 && time < 4)
                CD_Text.GetComponent<Text>().text = "1";
            else if (time > 4 && time < 4.5)
                CD_Text.GetComponent<Text>().text = "GO!";
            else if(time > 4.5)
            {
                CountDown = false;
                CD_Text.GetComponent<Text>().enabled = false;
            }
        }

        if (PlayersText.text != PlayersRemaining.ToString())
            PlayersText.text = PlayersRemaining.ToString();

       
        if (PlayersRemaining == 1 && !win)
        {
            //Select winner 
            for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            {
                if (players_alive[i])
                {
                    winner = i + 1;
                    break;
                }
            }

            string winner_name = PhotonNetwork.CurrentRoom.GetPlayer(winner).NickName;

            //show winner screen
            GameObject WinUI = GameObject.Find("WinScreen");
            WinUI.GetComponent<Image>().enabled = true;
            WinUI.GetComponentInChildren<Text>().enabled = true;
            WinUI.GetComponentInChildren<Text>().text = winner_name + " has won the game!";

            win = true;
            WinTime = PhotonNetwork.Time;
        }

        if(win)
        {
            if (PhotonNetwork.IsMasterClient /*&& all players accept rematch*/)
            {
                if (PhotonNetwork.Time - WinTime > 2 && !change_sceen)
                {
                    change_sceen = true;
                    this.photonView.RPC("WinScreen", RpcTarget.All);
                }
                    
            }
        }
    }

    [PunRPC]
    void WinScreen()
    {
        PhotonNetwork.LoadLevel("WinScreen");
    }

    public void OnPlayerDeath(int player_num)
    {
        this.photonView.RequestOwnership();
        PlayersRemaining--;
        players_alive[player_num - 1] = false;
    }

    public int ReturnPlayersLeft()
    {
        return PlayersRemaining;
    }

    public GameObject ReturnPlayerAlive() //return alive actor number, -1 if no one alive
    {
        GameObject player;
        int actor_number = -1;

        for (int i = 0; i < PlayersRemaining; i++)
        {
            if (players_alive[i] == true)
                actor_number = i+1;
        }

        player = PhotonNetwork.CurrentRoom.GetPlayer(actor_number).TagObject as GameObject;

        return player;
    }

    public bool GameStarted()
    {
        return !CountDown;
    }

    public bool GameEnded()
    {
        return win;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(CountDown);
            stream.SendNext(players_alive);
            stream.SendNext(PlayersRemaining);
        }
        else
        {
            //CountDown = (bool)stream.ReceiveNext();
            players_alive = (bool[])stream.ReceiveNext();
            PlayersRemaining = (int)stream.ReceiveNext();
        }
            
    }
}

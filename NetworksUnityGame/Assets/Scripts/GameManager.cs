using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class GameManager : MonoBehaviourPun, IPunObservable
{
    public int PlayersRemaining = 0;
    int winner = 0;
    bool win = false;

    [SerializeField] bool[] players_alive = { false, false, false, false };

    GameObject PlayersIcon;
    Text PlayersText;

    // Start is called before the first frame update
    void Start()
    {
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

        if (PlayersText.text != PlayersRemaining.ToString())
            PlayersText.text = PlayersRemaining.ToString();

        /* //TODO: UNCOMMENT, like this for testing
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
        }*/
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(players_alive);
            stream.SendNext(PlayersRemaining);
        }
        else
        {
            players_alive = (bool[])stream.ReceiveNext();
            PlayersRemaining = (int)stream.ReceiveNext();
        }
            
    }
}

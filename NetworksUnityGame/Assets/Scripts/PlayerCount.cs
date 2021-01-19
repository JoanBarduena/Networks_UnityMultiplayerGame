using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerCount : MonoBehaviour
{
    Text text;

    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        int max_players = PhotonNetwork.CurrentRoom.MaxPlayers;
        int players = PhotonNetwork.CurrentRoom.PlayerCount;
        text.text = players + "/" + max_players;

        if (max_players == 2)
        {
            if (players == 1)
                text.color = Color.red;
            else
                text.color = Color.green;
        }
        else if (max_players == 3)
        {
            if (players == 1)
                text.color = Color.red;
            else if (players == 2)
                text.color = Color.yellow;
            else
                text.color = Color.green;
        }
        else
        {
            if (players == 1)
                text.color = Color.red;
            else if (players == 2)
                text.color = Color.yellow;
            else if (players > 2)
                text.color = Color.green;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WinScene : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient /*&& all players accept rematch*/)
        {
            this.photonView.RPC("Rematch", RpcTarget.All);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [PunRPC]
    void Rematch()
    {
        PhotonNetwork.LoadLevel("SampleScene"); //restart the game
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Exit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnExitButtonClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void ClosePopUp()
    {
        GameObject.Find("PopUpKill").SetActive(false); 
        GameObject.Find("Spectate").SetActive(false);
    }
}

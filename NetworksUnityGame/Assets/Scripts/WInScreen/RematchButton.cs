using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RematchButton : MonoBehaviour
{
    public void OnClick()
    {
        int num = PhotonNetwork.LocalPlayer.ActorNumber;
        GameObject obj = GameObject.Find("WinScreenManager(Clone)");
        obj.GetComponent<WinScene>().OnClickRematch(num);
    }
}

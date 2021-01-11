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
        Vector3 pos = new Vector3(UnityEngine.Random.Range(-10, 10), 0, UnityEngine.Random.Range(-10, 10));

        PhotonNetwork.Instantiate("PhotonPrefabs/Tank", pos, Quaternion.identity); 
    }
}

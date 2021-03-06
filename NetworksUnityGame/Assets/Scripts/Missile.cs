﻿using Photon.Pun;
using UnityEngine;

public class Missile : MonoBehaviourPun
{
    Rigidbody rb;
    GameObject explosionParticles;
    public int bounces = 2;
    public int damage = 10;

    float timeAlive = 0;

    public static float maxTimeAlive = 6;
    public GameObject spark;
    public GameObject explosion;
    public GameObject explosionSound;
    // Audio 
    private AudioSource audiosource; 
   
    void Awake()
    {
        explosionParticles = (GameObject)Resources.Load("PhotonPrefabs/Tanks/Missiles/Explosion");
        rb = GetComponent<Rigidbody>();

        audiosource = GetComponent<AudioSource>();

        int num = gameObject.GetPhotonView().OwnerActorNr;
        GameObject tank = PhotonNetwork.CurrentRoom.GetPlayer(num).TagObject as GameObject;
        tank.GetComponent<PlayerController>().ShootSound();
    }

    // Update is called once per frame
    void Update()
    {
        timeAlive += Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);

        if (timeAlive > maxTimeAlive)
            PhotonNetwork.Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bounces > 0 && collision.collider.gameObject.tag != "Tank" && collision.collider.gameObject.tag != "Box" && collision.collider.gameObject.tag != "MiniTank")
        {
            if((bounces > 1 && !photonView.IsMine) || (bounces > 0 && photonView.IsMine))
            {
                audiosource.PlayOneShot(audiosource.clip);
                GameObject obj = GameObject.Instantiate(spark, this.transform.position, Quaternion.identity);
                Destroy(obj, 2.0f);
            }

            bounces--;
            return;
        }

        if(photonView.IsMine)
        {
            if (collision.collider.gameObject.tag == "Tank")
            {
                if (collision.collider.gameObject.GetComponent<PlayerController>().health - damage <= 0)
                {
                    GameObject myTank = PhotonNetwork.CurrentRoom.GetPlayer(PhotonNetwork.LocalPlayer.ActorNumber).TagObject as GameObject;
                    myTank.GetComponent<PlayerController>().KillSound();
                }
            }
        }
        

        GameObject exp = GameObject.Instantiate(explosion, this.transform.position, Quaternion.identity);
        Destroy(exp, 2.0f);

        GameObject sound = GameObject.Instantiate(explosionSound, this.transform.position, Quaternion.identity);
        Destroy(sound, 2.0f);

        PhotonNetwork.Destroy(gameObject);   
    }
}

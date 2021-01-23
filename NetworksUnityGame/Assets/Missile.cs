﻿using Photon.Pun;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody rb;
    GameObject explosionParticles;
    public int bounces = 2;
    public int damage = 10;

    float timeAlive = 0;

    public static float maxTimeAlive = 6;

    // Audio 
    public AudioSource bounceSound; 
    void Awake()
    {
        explosionParticles = (GameObject)Resources.Load("PhotonPrefabs/Tanks/Missiles/Explosion");
        rb = GetComponent<Rigidbody>();
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
        if (bounces > 0 && collision.collider.gameObject.tag != "Tank" && collision.collider.gameObject.tag != "Box")
        {
            bounceSound.Play();
            bounces--;
            return;
        }
        // TODO: change to PhotonNetwork.Instantiate 
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        PhotonNetwork.Destroy(gameObject);
    }
}

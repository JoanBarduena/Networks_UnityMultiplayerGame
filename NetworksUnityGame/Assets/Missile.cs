using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Rigidbody rb;
    GameObject explosionParticles;
    public int bounces = 2;

    void Awake()
    {
        explosionParticles = (GameObject)Resources.Load("PhotonPrefabs/Tanks/Missiles/Explosion");
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(rb.velocity, Vector3.up);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (bounces > 0)
        {
            bounces--;
            return;
        }
        // TODO: change to PhotonNetwork.Instantiate 
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

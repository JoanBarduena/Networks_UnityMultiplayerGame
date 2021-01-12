using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    GameObject explosionParticles;
    
    void Awake()
    {
        explosionParticles = (GameObject)Resources.Load("PhotonPrefabs/Explosion");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Instantiate(explosionParticles,transform.position,Quaternion.identity);
        Destroy(gameObject);
    }
}

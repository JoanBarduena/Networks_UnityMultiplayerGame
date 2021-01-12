using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BoxScript : MonoBehaviour
{
    //PhotonView PV;

    public int health = 3;
    public bool has_powerup = false;

    private void Awake()
    {
        //PV = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Bullet Collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Missile")
        {
            health--;

            if (health <= 0)
            {
                //particles
                //sound effect

                if (has_powerup)
                    PhotonNetwork.Instantiate("Prefabs/PowerUp", this.transform.position, Quaternion.identity);

                Destroy(this.gameObject);
            }
        }
    }
}

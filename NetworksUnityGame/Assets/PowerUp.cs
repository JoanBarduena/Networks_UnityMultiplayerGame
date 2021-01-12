using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PowerUpType
{
    SPEED,
    FIRE_RATE,
    BOUNCES,
    HEALTH,

    NONE
}
public class PowerUp : MonoBehaviour
{

    public PowerUpType type;

    PlayerController player;

    bool activated = false;

    float activeTime = 0;
    public float maxActiveTime = 5;


    void Update()
    {
        if (activated)
        {
            activeTime += Time.deltaTime;

            if (activeTime >= maxActiveTime)
            {
                activated = false;
                player.ApplyPowerUp(type, false);
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided is a tank
        if (other.CompareTag("Tank"))
        {
            // Apply powerup
            player = other.GetComponent<PlayerController>();
            player.ApplyPowerUp(type, true);
            activated = true;

            // Hide the power up in the map
            GetComponent<MeshRenderer>().enabled = false;
            GetComponent<SphereCollider>().enabled = false;

        }
    }
}

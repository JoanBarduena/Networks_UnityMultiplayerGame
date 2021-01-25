using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public enum PowerUpType
{
    SPEED,
    FIRE_RATE,
    BOUNCES,
    HEALTH,
    MINI_TANKS,

    NONE
}
public class PowerUp : MonoBehaviourPun
{

    public PowerUpType type;

    PlayerController player;

    bool activated = false;

    float activeTime = 0;
    public float maxActiveTime = 5;

    ParticleSystem particles;

    private Vector3 rotation = new Vector3(0, 0.5f, 0);

    void Update()
    {
        this.transform.Rotate(rotation); 

        if (activated)
        {
            activeTime += Time.deltaTime;

            if (activeTime >= maxActiveTime)
            {
                particles.Stop();
                activated = false;
                player.ApplyPowerUp(type, false);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided is a tank
        if (other.CompareTag("Tank"))
        {
            if (other.gameObject.GetPhotonView().OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
            {
                // Apply powerup
                player = other.GetComponent<PlayerController>();
                player.PowerUpSound();
                player.ApplyPowerUp(type, true);
                activated = true;
            }

            if(gameObject.GetPhotonView().IsMine)
            {
                // Hide the power up in the map
                Vector3 pos = Vector3.zero;
                pos.y = -10.0f;
                this.transform.position = pos;
            }

            switch (type)
            {
                case PowerUpType.SPEED:
                    break;
                case PowerUpType.FIRE_RATE:
                    particles = other.gameObject.transform.Find("FireRateParticle").GetComponent<ParticleSystem>();
                    particles.Play();
                    break;
                case PowerUpType.BOUNCES:
                    particles = other.gameObject.transform.Find("BouncesParticle").GetComponent<ParticleSystem>();
                    particles.Play();
                    break;
                case PowerUpType.HEALTH:
                    particles = other.gameObject.transform.Find("Heal").GetComponent<ParticleSystem>();
                    particles.Play();
                    break;
                case PowerUpType.MINI_TANKS:
                    break;
                default:
                    break;
            }

        }
    }
}

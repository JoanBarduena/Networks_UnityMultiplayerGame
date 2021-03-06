﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BoxScript : MonoBehaviourPun, IPunObservable
{
    PhotonView PV;

    public Slider hpbar;
    Canvas canvas;

    public int max_health = 3;
    [SerializeField] public int current_health;

    public PowerUpType powerup;
    private string BoxSoundPrefabPath = "Map/BoxSound";

    bool PUSpawned = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        canvas = gameObject.GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        hpbar.maxValue = max_health;
        hpbar.value = max_health;

        powerup = (PowerUpType)Random.Range(0, 5);
    }

    // Start is called before the first frame update
    void Start()
    {
        current_health = max_health;
        canvas.enabled = false;
    }

    void Update()
    {
        hpbar.value = current_health;

        if (!canvas.enabled && current_health < max_health)
            canvas.enabled = true;

        if (current_health <= 0)
            Destroyed();
    }

    void LateUpdate()
    {
        hpbar.transform.LookAt(hpbar.transform.position + Camera.main.transform.forward);
    }

    // Bullet Collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Missile")
        {
            this.photonView.RequestOwnership();
            current_health--;
        }
    }

    private void Destroyed()
    {
        if (!PV.IsMine)
            return;
        //particles 
        Vector3 pos = gameObject.transform.position;
        pos.y = 1.0f;
        GameObject particles = PhotonNetwork.Instantiate("Map/BoxDestroyed", pos, Quaternion.identity);
        //sound effect
        GameObject boxDestoyed = PhotonNetwork.Instantiate(BoxSoundPrefabPath, gameObject.transform.position, Quaternion.identity);
        AudioSource boxSound = boxDestoyed.GetComponent<AudioSource>();
        boxSound.PlayOneShot(boxSound.clip);

        //PowerUp Drop Chance
        int r = Random.Range(0, 2); //% 0-4?

        //SpawnPowerup(powerup);
        if (r == 0 && !PUSpawned)
        {
            PUSpawned = true;
            SpawnPowerup(powerup);
        }
        PhotonNetwork.Destroy(gameObject);
    }

    void SpawnPowerup(PowerUpType type)
    {
        Vector3 pos = transform.position;
        pos.y = 0.7f;

        switch (type)
        {
            case PowerUpType.SPEED:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/PowerUps/Items_FuelCan_01", pos, Quaternion.identity);
                break;
            case PowerUpType.FIRE_RATE:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/PowerUps/FireRatePowerUp", pos, Quaternion.identity);
                break;
            case PowerUpType.BOUNCES:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/PowerUps/BouncePowerUP", pos, Quaternion.identity);
                break;
            case PowerUpType.HEALTH:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/PowerUps/HealthPowerUp", pos, Quaternion.identity);
                break;
            case PowerUpType.MINI_TANKS:
                PhotonNetwork.Instantiate("PhotonPrefabs/Tanks/PowerUps/TankMiniPowerUp", pos, Quaternion.identity);
                break;
            default:
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(current_health);
        else
            this.current_health = (int)stream.ReceiveNext();
    }
}

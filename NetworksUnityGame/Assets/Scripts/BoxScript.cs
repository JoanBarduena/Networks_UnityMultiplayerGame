using System.Collections;
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

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        canvas = gameObject.GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;

        hpbar.maxValue = max_health;
        hpbar.value = max_health;
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
        //particles
        //sound effect

        //SpawnPowerup(powerup);
        PhotonNetwork.Destroy(gameObject);
    }

    void SpawnPowerup(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.SPEED:
                PhotonNetwork.InstantiateRoomObject("", this.transform.position, Quaternion.identity);
                break;
            case PowerUpType.FIRE_RATE:
                PhotonNetwork.InstantiateRoomObject("", this.transform.position, Quaternion.identity);
                break;
            case PowerUpType.BOUNCES:
                PhotonNetwork.InstantiateRoomObject("", this.transform.position, Quaternion.identity);
                break;
            case PowerUpType.HEALTH:
                PhotonNetwork.InstantiateRoomObject("", this.transform.position, Quaternion.identity);
                break;
            case PowerUpType.MINI_TANKS:
                PhotonNetwork.InstantiateRoomObject("", this.transform.position, Quaternion.identity);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class BoxScript : MonoBehaviourPun, IPunObservable
{
    public enum PowerUp 
    {
        NONE = 0, 
        SPEED,
        FIRERATE,
        HEALTH
    }

    PhotonView PV;

    public Slider hpbar;
    Canvas canvas;

    public int max_health = 3;
    [SerializeField] public int current_health;

    public PowerUp powerup = PowerUp.NONE;

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

        switch (powerup)
        {
            case PowerUp.NONE:
                break;
            case PowerUp.SPEED:
                PhotonNetwork.Instantiate("Prefabs/PowerUp", this.transform.position, Quaternion.identity);
                break;
            case PowerUp.FIRERATE:
                PhotonNetwork.Instantiate("Prefabs/PowerUp", this.transform.position, Quaternion.identity);
                break;
            case PowerUp.HEALTH:
                PhotonNetwork.Instantiate("Prefabs/PowerUp", this.transform.position, Quaternion.identity);
                break;
        }

        PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
            stream.SendNext(current_health);
        else
            this.current_health = (int)stream.ReceiveNext();
    }
}

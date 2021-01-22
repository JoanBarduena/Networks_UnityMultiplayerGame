using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TankColor
{
    GREEN,
    RED,
    YELLOW,
    BLUE,

    NONE
}
public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
{

    PhotonView PV;
    Rigidbody rb;

    Canvas canvas;
    public Slider hpbar;
    [SerializeField] public float health = 100;
    private float maxHealth = 100;
    public float moveSpeed = 5.0f;
    public float turnSpeed = 180f;

    private string movementAxisName;
    private string turnAxisName;
    private float movementInputValue;
    private float turnInputValue;

    // Shooting
    protected bool shoot = false;
    public float fireRate = 2; // missiles per second
    private float lastShot = 0;
    private float bulletSpawnTime = 6.0f;
    private int missileBounces = 1;

    // PowerUps
    int bouncesIncrease = 1;
    float speedIncrease = 3;
    float fireRateIncrease = 2f;
    float healthIncrease = 50;
    List<GameObject> miniTanksList;

    // tank parts
    Vector3 aimPoint;
    GameObject turret;
    Transform firePoint;

    GameObject AimMark;
    GameObject GM;

    public int killer;
    public GameObject playerKiller;

    public TankColor tankColor;
    private string missileResourcePath = "PhotonPrefabs/Tanks/Missiles/Missile";

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        turret = transform.Find("Turret").gameObject;
        firePoint = turret.transform.GetChild(0).transform.Find("FirePoint");
        AimMark = GameObject.Find("AimMark");
        

        miniTanksList = new List<GameObject>();

        canvas = gameObject.GetComponentInChildren<Canvas>();
        hpbar.maxValue = (int)maxHealth;
        hpbar.value = (int)maxHealth;

        switch (tankColor)
        {
            case TankColor.BLUE:
                missileResourcePath += "Blue";
                break;
            case TankColor.GREEN:
                missileResourcePath += "Green";
                break;
            case TankColor.RED:
                missileResourcePath += "Red";
                break;
            case TankColor.YELLOW:
                missileResourcePath += "Yellow";
                break;
            default:
                Debug.LogError("Tank should be of some color");
                break;
        }

        if (PV.IsMine)
            Camera.main.GetComponent<FollowCamera>().target = gameObject.transform;
    }

    void Start()
    {
        movementAxisName = "Vertical";
        turnAxisName = "Horizontal";
        canvas.enabled = false;

        

        PV.Owner.TagObject = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(GM == null)
            GM = GameObject.Find("GameManager(Clone)");

        if(GM.GetComponent<GameManager>().GameStarted())
        {
            // Disabling inputs while starting game
            movementInputValue = Input.GetAxis(movementAxisName);
            turnInputValue = Input.GetAxis(turnAxisName);

            if (Input.GetMouseButtonDown(0))
            {
                shoot = true;
            }
        }

        // Aiming
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 50, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            aimPoint = new Vector3(hit.point.x, 0.1f, hit.point.z);
            Debug.DrawLine(turret.transform.position, aimPoint, Color.red);
            AimMark.transform.position = aimPoint;
        }

        HandleHealthBar();

        //// Follow last killer
        //if (playerKiller != null && playerKiller.GetComponent<PlayerController>().health <= 0)
        //{
        //    if (playerKiller == gameObject) //you killed your killer
        //    {
        //        killer = PhotonNetwork.CurrentRoom.GetPlayer(killer).GetNext().ActorNumber; //get killer
        //        playerKiller = PhotonNetwork.CurrentRoom.GetPlayer(killer).TagObject as GameObject; //get killer
        //    }
        //    else
        //    {
        //        killer = playerKiller.GetComponent<PlayerController>().killer;
        //        playerKiller = playerKiller.GetComponent<PlayerController>().playerKiller;
        //    }

        //    Debug.Log("following " + killer);
        //    Camera.main.GetComponent<FollowCamera>().target = playerKiller.transform;
        //}
    }

    void HandleHealthBar()
    {
        //hp bar update
        hpbar.value = (int)health;
        hpbar.transform.parent.LookAt(hpbar.transform.position + Camera.main.transform.forward);

        if (!canvas.enabled && health < maxHealth)
            canvas.enabled = true;
        else if (canvas.enabled && health == maxHealth)
            canvas.enabled = false;
    }

    void Move()
    {
        Vector3 movement = movementInputValue * transform.forward * moveSpeed * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }

    void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);
    }

    void Aim()
    {
        Vector3 direction = aimPoint - turret.transform.position;
        direction.y = 0;
        
        turret.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    protected void Shoot()
    {
        lastShot += Time.deltaTime;
        if (!shoot || lastShot < 1/fireRate) return;

        GameObject bullet = PhotonNetwork.Instantiate(missileResourcePath, firePoint.position, Quaternion.LookRotation(turret.transform.forward));
        bullet.GetComponent<Rigidbody>().AddForce(turret.transform.forward * 15.0f, ForceMode.Impulse);
        bullet.GetComponent<Missile>().bounces = missileBounces;

        lastShot = 0;
        shoot = false;
    }

    void FixedUpdate()
    {
        // TODO: uncomment

        if (!PV.IsMine)
            return;

        Move();
        Turn();
        Aim();
        Shoot();
    }

    private void InstantiateMinitanks(bool activate)
    {
        if (activate)
        {
            Vector3 pos = transform.position;
            pos.z += 5;
            miniTanksList.Add(Instantiate((GameObject)Resources.Load("PhotonPrefabs/Tanks/TankMini"), pos, Quaternion.identity));
            pos.z -= 10;
            miniTanksList.Add(Instantiate((GameObject)Resources.Load("PhotonPrefabs/Tanks/TankMini"), pos, Quaternion.identity));
        }
        else
        {
            foreach (GameObject obj in miniTanksList)
            {
                Destroy(obj);
            }
        }
    }

    public void ApplyPowerUp(PowerUpType type, bool activate)
    {
        switch (type)
        {
            case PowerUpType.SPEED:
                moveSpeed += (activate) ? speedIncrease : -speedIncrease;
                break;
            case PowerUpType.FIRE_RATE:
                fireRate += (activate) ? fireRateIncrease : -fireRateIncrease;
                break;
            case PowerUpType.BOUNCES:
                missileBounces += (activate) ? bouncesIncrease : -bouncesIncrease;
                break;
            case PowerUpType.HEALTH:
                health = (activate) ? Mathf.Min(health + healthIncrease, maxHealth) : health;
                break;
            case PowerUpType.MINI_TANKS:
                InstantiateMinitanks(activate);
                break;
            default:
                break;
        }
    }

    // Bullet Collisions
    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.tag == "Missile")
        {
            if (!PV.IsMine) //Only apply dmg if the tank is yours
                return;

            health -= collision.collider.gameObject.GetComponent<Missile>().damage;

            if (health <= 0)
            {
                killer = collision.collider.gameObject.GetComponent<PhotonView>().OwnerActorNr;
                Die();
            }
        }
    }

    void Die()
    {
        string name;
        if (killer == PhotonNetwork.LocalPlayer.ActorNumber && PhotonNetwork.PlayerList.Length > 1) //suicide
        {
            name = "yourself";
            killer = PhotonNetwork.LocalPlayer.GetNext().ActorNumber; //follow next player
        }
        else
        {
            name = PhotonNetwork.CurrentRoom.GetPlayer(killer).NickName;
        }

        if(GM.GetComponent<GameManager>().ReturnPlayersLeft() > 1) //TODO: Here goes a 2, set to 1 for tests 
        {
            //PopUp kill
            GameObject popup = GameObject.Find("PopUpKill");
            popup.GetComponent<Image>().enabled = true;
            popup.GetComponentInChildren<Text>().enabled = true;
            popup.GetComponentInChildren<Text>().text = "You were killed by " + name;

            //show spectate button
            GameObject spectate = GameObject.Find("Spectate");
            spectate.GetComponent<Image>().enabled = true;
            spectate.GetComponent<Button>().enabled = true;
            spectate.GetComponentInChildren<Text>().enabled = true;

            //show exit button
            GameObject exit = GameObject.Find("Exit");
            exit.GetComponent<Image>().enabled = true;
            exit.GetComponent<Button>().enabled = true;
            exit.GetComponentInChildren<Text>().enabled = true;
        }

        //Notify game manager you died
        GM.GetComponent<GameManager>().OnPlayerDeath(PhotonNetwork.LocalPlayer.ActorNumber);

        //Camera follow killer
        playerKiller = PhotonNetwork.CurrentRoom.GetPlayer(killer).TagObject as GameObject; //get killer
        Camera.main.GetComponent<FollowCamera>().target = playerKiller.transform; //follow killer
        Camera.main.GetComponent<FollowCamera>().distance += 10; //set new camera pos

        //hide aim mark and destroy tank -- LAST THING TO DO
        GameObject.Find("AimMark").GetComponent<MeshRenderer>().enabled = false;
        PhotonNetwork.Destroy(gameObject);

      
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Sending health to other players

        if (stream.IsWriting)
            stream.SendNext(health);
        else
            health = (float)stream.ReceiveNext();
    }
}

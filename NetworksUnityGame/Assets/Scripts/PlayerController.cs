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
    }

    // Update is called once per frame
    void Update()
    {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(turnAxisName);

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

        if (Input.GetMouseButtonDown(0))
        {
            shoot = true;
        }

        HandleHealthBar();
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
                Die();
        }
    }

    void Die()
    {
        Debug.Log("IM DEAD");
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

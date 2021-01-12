using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TankColor
{
    GREEN,
    RED,
    YELLOW,
    BLUE,

    NONE
}
public class PlayerController : MonoBehaviourPunCallbacks
{

    PhotonView PV;
    Rigidbody rb;

    public float health = 100;
    private float maxHealth = 100;
    public float moveSpeed = 5.0f;
    public float turnSpeed = 180f;

    private string movementAxisName;
    private string turnAxisName;
    private float movementInputValue;
    private float turnInputValue;

    // Shooting
    private bool shoot = false;
    public float fireRate = 2; // missiles per second
    float lastShot = 0;
    private float bulletSpawnTime = 6.0f;
    private int missileBounces = 1;

    // PowerUps
    int bouncesIncrease = 1;
    float speedIncrease = 3;
    float fireRateIncrease = 2f;
    float healthIncrease = 50;

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
    }

    void Start()
    {
        movementAxisName = "Vertical";
        turnAxisName = "Horizontal";
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

    void Shoot()
    {
        lastShot += Time.deltaTime;
        if (!shoot || lastShot < 1/fireRate) return;

        //GameObject bullet = PhotonNetwork.Instantiate(missileResourcePath, firePoint.position, Quaternion.LookRotation(turret.transform.forward));
        //bullet.GetComponent<Rigidbody>().AddForce(turret.transform.forward * 15.0f, ForceMode.Impulse);
        // TODO: delete and use line above //
        GameObject bullet = Instantiate((GameObject)Resources.Load(missileResourcePath), firePoint.position, Quaternion.LookRotation(turret.transform.forward));
        bullet.GetComponent<Rigidbody>().AddForce(turret.transform.forward * 15.0f, ForceMode.Impulse);
        bullet.GetComponent<Missile>().bounces = missileBounces;

        Destroy(bullet, bulletSpawnTime);
        lastShot = 0;
        shoot = false;
    }

    void FixedUpdate()
    {
        // TODO: uncomment
        //if (!PV.IsMine)
        //    return;

        Move();
        Turn();
        Aim();
        Shoot();
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
            default:
                break;
        }
    }
}

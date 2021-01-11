using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviourPunCallbacks
{

    PhotonView PV;
    Rigidbody rb;

    public float moveSpeed = 5.0f;
    public float turnSpeed = 180f;

    private string movementAxisName;
    private string turnAxisName;
    private float movementInputValue;
    private float turnInputValue;
    private float bulletSpawnTime = 6.0f;

    private bool shoot = false;
    private bool moving = false;

    // Canviar per una animation easy
    private float wheelAngle = 0.0f;


    // tank parts
    Vector3 aimPoint;
    GameObject turret;
    Transform firePoint;
    GameObject wheels;
    GameObject AimMark;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();

        turret = transform.Find("Turret").gameObject;
        firePoint = turret.transform.GetChild(0).transform.Find("FirePoint");
        wheels = transform.Find("Wheels").gameObject;
        AimMark = GameObject.Find("AimMark");
    }

    void Start()
    {
        movementAxisName = "Vertical";
        turnAxisName = "Horizontal";

        // aim marker (if placed)
        //Instantiate(markerobj, ray.origin, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        movementInputValue = Input.GetAxis(movementAxisName);
        turnInputValue = Input.GetAxis(turnAxisName);

        // Aiming
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //Debug.DrawLine(ray.origin, ray.origin + ray.direction * 50, Color.red);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 500))
        {
            aimPoint = new Vector3(hit.point.x, 0.1f, hit.point.z);
            //Debug.DrawLine(turret.transform.position, aimPoint, Color.red);
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

        moving = moving || Mathf.Abs(movement.magnitude) > 0.1f;
    }

    void Turn()
    {
        float turn = turnInputValue * turnSpeed * Time.deltaTime;

        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        moving = moving || Mathf.Abs(turn) > 0.1f;
    }

    void Aim()
    {
        Vector3 direction = aimPoint - turret.transform.position;
        direction.y = 0;
        
        turret.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    void Shoot()
    {
        if (!shoot) return;
        GameObject bullet = PhotonNetwork.Instantiate("PhotonPrefabs/MissileGreen", firePoint.position, Quaternion.LookRotation(turret.transform.forward));
        bullet.GetComponent<Rigidbody>().AddForce(turret.transform.forward * 15.0f, ForceMode.Impulse);

        Destroy(bullet, bulletSpawnTime);
        shoot = false;
    }

    void Animate()
    {
        if (!moving) return;
        wheelAngle += Time.deltaTime;
        if (wheelAngle > 3.14f) wheelAngle = 0;

        for (int i = 0; i < wheels.transform.childCount; i++)
        {
            wheels.transform.GetChild(i).Rotate(Vector3.right, wheelAngle);
        }
        moving = false;
    }

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        Move();
        Turn();
        Aim();
        Shoot();
        Animate();
    }
}

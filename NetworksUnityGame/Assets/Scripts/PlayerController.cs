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

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
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

    void FixedUpdate()
    {
        if (!PV.IsMine)
            return;

        Move();
        Turn();
    }
}

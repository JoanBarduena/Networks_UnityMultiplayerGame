using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class MiniTank : PlayerController
{

    public GameObject target;
    private NavMeshAgent agent;

    float timeAlive = 0;
    float maxTimeAlive = 20;


    private void Awake()
    {
        turret = transform.Find("Turret").gameObject;
        firePoint = turret.transform.GetChild(0).transform.Find("FirePoint");
        missileResourcePath = "PhotonPrefabs/Tanks/Missiles/MissileGreen";
        agent = GetComponent<NavMeshAgent>();

        health = 1;
        fireRate = 0.5f;
    }
    private void Start()
    {
       
    }

    private void PickTarget()
    {

        var tanks = GameObject.FindGameObjectsWithTag("Tank");
        float min = 99999999;

        for(int i=0; i<tanks.Length; i++)
        {
            if (!tanks[i].GetPhotonView().IsMine)
            {
                float d = Mathf.Abs(Vector3.Distance(tanks[i].transform.position, transform.position));
                if (min > d)
                {
                    min = d;
                    target = tanks[i];
                }
                
            }
        }
    }

    private void Update()
    {
        if (!photonView.IsMine)
            return;

        Aim();
        if (target)
        {
            agent.SetDestination(target.transform.position);
        }
        else
        {
            PickTarget();
        }

        timeAlive += Time.deltaTime;
        if (timeAlive > maxTimeAlive)
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }

    void Aim()
    {
        if (!target) return;
        Vector3 direction = target.transform.position - turret.transform.position;
        direction.y = 0;

        turret.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        Shoot();
        shoot = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Missile"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class MiniTank : PlayerController
{

    public GameObject target;
    private NavMeshAgent agent;

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
        for(int i=0; i<tanks.Length; i++)
        {
            if (!tanks[i].GetPhotonView().IsMine)
            {
                target = tanks[i];
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
        //if (collision.gameObject.CompareTag("Tank"))
        //{
        //    Destroy(gameObject);
        //}
    }
}

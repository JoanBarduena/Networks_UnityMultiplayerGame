using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MiniTank : PlayerController
{
    public GameObject target;
    private NavMeshAgent agent;
    
    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        // To change to a network player
        target = GameObject.Find("Target");
        agent.SetDestination(target.transform.position);
        fireRate = 0.5f;
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Shoot();
        shoot = true;
    }
}

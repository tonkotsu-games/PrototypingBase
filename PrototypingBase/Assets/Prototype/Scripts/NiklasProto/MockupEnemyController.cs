﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MockupEnemyController : MonoBehaviour
{
    GameObject player;
    Vector3 target;
    NavMeshAgent agent;
    Animator anim;

    [SerializeField]
    private float minSpd;
    [SerializeField]
    private float maxSpd;

    [SerializeField]
    private float distance;

    [SerializeField]
    private EnemyType.EnemyTypes enemyType;

    private void Start()
    {
        player = Locator.instance.GetPlayerGameObject();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        agent.speed = Random.Range(minSpd, maxSpd);
        WaveManager.instance.AddToCurrentEnemies(gameObject);
    }
    void Update()
    {
        target = player.transform.position;

        agent.SetDestination(target);
        if (Vector3.Distance(transform.position, target) <= distance)
        {
            agent.isStopped = true;
            anim.SetBool("rangedAttk", true);

        }
        else
        {
            agent.isStopped = false;
            anim.SetBool("rangedAttk", false);
        }
    }

    private void OnDestroy()
    {
        WaveManager.instance.RemoveFromCurrentEnemies(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.GetComponent<PlayerController>()!= null)
        {
            Destroy(gameObject);
        }
    }
}


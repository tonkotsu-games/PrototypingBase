using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MockupEnemyController : MonoBehaviour , IDamageAble
{
    [SerializeField]
    private EnemyType.EnemyTypes enemyType;
    private GameObject player;
    private Vector3 target;
    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField]
    private float minSpd;
    [SerializeField]
    private float maxSpd;
    [SerializeField]
    private float distance;

    [SerializeField]
    private int health = 0;

    private void Awake()
    {
        WaveManager.instance.AddToCurrentEnemies(gameObject);
    }

    private void Start()
    {
        player = Locator.instance.GetPlayerGameObject();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        agent.speed = Random.Range(minSpd, maxSpd);
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

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        WaveManager.instance.RemoveFromCurrentEnemies(gameObject);
    }
}
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

    [SerializeField]
    private List<GameObject> bloodSpawns;

    bool starting = false;

    private void Awake()
    {
        WaveManager.instance.AddToCurrentEnemies(gameObject);
        if(enemyType == EnemyType.EnemyTypes.Boss)
        {
            starting = false;
            StartCoroutine(Delay());
            
        }
        else
        {
            starting = true;
        }
    }

    private void Start()
    {
        player = Locator.instance.GetPlayerGameObject();
        agent = gameObject.GetComponent<NavMeshAgent>();
        anim = gameObject.GetComponent<Animator>();
        agent.speed = Random.Range(minSpd, maxSpd);
        //player.GetComponent<PlayerController>().EnemyAdd(gameObject);
    }

    void Update()
    {
        if (starting)
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
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;
        SpawnBlood();
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        WaveManager.instance.RemoveFromCurrentEnemies(gameObject);
       // player.GetComponent<PlayerController>().EnemyRemove(gameObject);
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
        starting = false;
    }

    private void SpawnBlood()
    {
        GameObject tempSpawn = bloodSpawns[Random.Range(0, bloodSpawns.Count)];
        ParticleSystem part = tempSpawn.GetComponentInChildren<ParticleSystem>();
        part.Play();
        if(part == null)
        {
            Debug.Log("oh no");
        }
        
    }
}
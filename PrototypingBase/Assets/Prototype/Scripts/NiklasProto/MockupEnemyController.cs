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
    private Rigidbody rb;
    private bool gotPushed = false;
    private int currentStaggerCount = 0; 

    [SerializeField]
    private float minSpd;
    [SerializeField]
    private float maxSpd;
    [SerializeField]
    private float distance;

    [SerializeField]
    private int damageDealt = 1;

    [SerializeField]
    private float maxKnockbackRange = 10f;

    [SerializeField]
    private int maxStaggerAmount = 3;

    [SerializeField]
    private int health = 0;

    [SerializeField]
    private List<GameObject> bloodSpawns;

    [SerializeField]
    BoxCollider meleeHitbox;

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
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void Update()
    {
        anim.ResetTrigger("attacking");

        if (starting)
        {
            target = player.transform.position;

            agent.SetDestination(target);
            if (Vector3.Distance(transform.position, target) <= distance)
            {
                agent.isStopped = true;
                if (enemyType == EnemyType.EnemyTypes.Ranged)
                {
                    anim.SetBool("rangedAttk", true);
                }
                else
                {
                    anim.SetTrigger("attacking");
                }
            }
            else
            {
                agent.isStopped = false;
                anim.SetBool("rangedAttk", false);
            }

            if (gotPushed)
            {
                if(Vector3.Distance(target,transform.position)>= maxKnockbackRange)
                {
                    rb.velocity = Vector3.zero;
                    gotPushed = false;
                    rb.isKinematic = true;
                }
            }
        }
    }
    public void Damage(int damageAmount) { }
    public void DamageAndPush(int damageAmount,Vector3 playerPos = default(Vector3),float pushStrength = default(float))
    {
        health -= damageAmount;
        SpawnBlood();
        if(playerPos != Vector3.zero)
        {
            Vector3 pushDirection = transform.position - playerPos;
            rb.isKinematic = false;
            rb.AddForce((pushDirection * pushStrength), ForceMode.Impulse);
            gotPushed = true;
            if (currentStaggerCount <= maxStaggerAmount)
            {
                currentStaggerCount++;
                anim.SetTrigger("staggered");
            }
        }
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
        
    }

    public void DamagePlayer(PlayerNiklas playerScript)
    {
        playerScript.Damage(damageDealt);
        currentStaggerCount = 0;

    }

    public void EnableMeleeHit()
    {
        meleeHitbox.enabled = true;
    }
    public void DisableMeleeHit()
    {
        meleeHitbox.enabled = false;
    }
}
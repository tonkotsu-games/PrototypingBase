using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MockupEnemyController : MonoBehaviour
{
    GameObject player;
    Vector3 target;
    NavMeshAgent agent;

    [SerializeField]
    private float minSpd;
    [SerializeField]
    private float maxSpd;

    private void Start()
    {
        player = Locator.instance.GetPlayerGameObject();
        agent = gameObject.GetComponent<NavMeshAgent>();
        agent.speed = Random.Range(minSpd, maxSpd);
        WaveManager.instance.AddToCurrentEnemies(gameObject);
    }
    void Update()
    {
        target = player.transform.position;
        agent.SetDestination(target);
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


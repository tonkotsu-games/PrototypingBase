using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField][Tooltip("The Wave this Spawner activates")]
    public Wave.Waves Wave;
    [SerializeField]
    public int amountOfEnemiesToSpawn;
    [SerializeField]
    GameObject mockupEnemyPrefab;

    private bool doneSpawning = false;

    private void Start()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    private void Update()
    {
        if(WaveManager.instance.currentWaveState == Wave && !doneSpawning)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < amountOfEnemiesToSpawn; i++)
        {
            Instantiate(mockupEnemyPrefab, transform.position, Quaternion.identity);
        }
        doneSpawning = true;
    }
}

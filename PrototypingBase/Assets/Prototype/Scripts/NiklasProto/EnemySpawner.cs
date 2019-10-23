using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] [Tooltip("The Wave this Spawner activates")]
    public Wave.Waves Wave;
    [SerializeField]
    [Header("Quick Enemies")]
    private int amountOfQuickEnemiesToSpawn;

    [SerializeField]
    [Header("HeavyEnemies")]
    private int amountOfHeavyEnemiesToSpawn;

    [SerializeField]
    [Header("RangedEnemies")]
    private int amountOfRangedEnemiesToSpawn;

    [HideInInspector]
    public int amountOfEnemiesToSpawn;

    [SerializeField]
    List<GameObject> mockupEnemyPrefabs;

    private bool doneSpawning = false;

    private void Start()
    {
        
        gameObject.GetComponent<BoxCollider>().enabled = false;
        gameObject.GetComponent<MeshRenderer>().enabled = false;
    }
    private void Update()
    {
        if (WaveManager.instance != null)
        {
            if (WaveManager.instance.currentWaveState == Wave && !doneSpawning)
            {
                SpawnEnemy();
            }
        }
    }

    private void SpawnEnemy()
    {
        for (int i = 0; i < amountOfQuickEnemiesToSpawn; i++)
        {
            Instantiate(mockupEnemyPrefabs[0], transform.position, Quaternion.identity);
        }
        for(int i = 0; i < amountOfHeavyEnemiesToSpawn; i++)
        {
            Instantiate(mockupEnemyPrefabs[1], transform.position, Quaternion.identity);
        }
        for(int i = 0; i < amountOfRangedEnemiesToSpawn; i++)
        {
            Instantiate(mockupEnemyPrefabs[2], transform.position, Quaternion.identity);
        }

        doneSpawning = true;
    }

    private void OnGUI()
    {
        if(WaveManager.instance == null)
        {
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.red;
            style.fontSize = 40;
            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box("WAVE MANAGER NOT IN SCENE \n U DUMMIE DUM DUM",style);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
            
        }
    }
}

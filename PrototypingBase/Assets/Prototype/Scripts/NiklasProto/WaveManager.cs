using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [SerializeField]
    float timeBetweenWaveSpawns = 2;

    private HashSet<GameObject> currentEnemies;
    [HideInInspector]
    public Wave.Waves currentWaveState;
    private Wave.Waves nextWaveState;
    private int waveNumber = 1;
    private bool sceneChanging = false;
    private Timer waveTimer = new Timer();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        currentEnemies = new HashSet<GameObject>();
    }
    // Start is called before the first frame update
    void Start()
    {
        waveTimer.Set(timeBetweenWaveSpawns);
        currentWaveState = Wave.Waves.Init;
        nextWaveState = (Wave.Waves)waveNumber;
    }

    private void LateUpdate()
    {
        CheckForEnemiesLeftAndChangeWaveState();
    }

    private void CheckForEnemiesLeftAndChangeWaveState()
    {
        if (waveNumber != System.Enum.GetValues(typeof(Wave.Waves)).Length)
        {
            if (currentEnemies.Count == 0)
            {
                Debug.Log("number of enemies: " + currentEnemies.Count);
                sceneChanging = true;
                ChangeWaveState(nextWaveState);
            }
        }
    }

    private void ChangeWaveState(Wave.Waves nextWave)
    {
        if (nextWave != currentWaveState)
        {
            if (waveTimer.timeCurrent <= 0)
            {
                currentWaveState = nextWave;
                waveNumber++;
                nextWaveState = (Wave.Waves)waveNumber;
                waveTimer.ResetTimer();
            }
            else
            {
                waveTimer.Tick();
            }
        }
            
    }

    public void AddToCurrentEnemies(GameObject enemy)
    {
        currentEnemies.Add(enemy);
    }

    public void RemoveFromCurrentEnemies(GameObject enemy)
    {
        currentEnemies.Remove(enemy);
    }

    private void OnGUI()
    {
        GUILayout.Box("Current Wave: " + currentWaveState);
        GUILayout.Box("Enemies Left: " + currentEnemies.Count);
    }
}

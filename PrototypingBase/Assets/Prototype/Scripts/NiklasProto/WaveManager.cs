using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    private HashSet<GameObject> currentEnemies;
    [HideInInspector]
    public Wave.Waves currentWaveState;
    private Wave.Waves nextWaveState;
    private int waveNumber = 1;

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
                ChangeWaveState(nextWaveState);
                waveNumber++;
                nextWaveState = (Wave.Waves)waveNumber;
            }
        }
    }

    private void ChangeWaveState(Wave.Waves nextWave)
    {
        if (nextWave != currentWaveState)
        {
            currentWaveState = nextWave;
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

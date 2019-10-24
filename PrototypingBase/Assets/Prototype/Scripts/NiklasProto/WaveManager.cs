using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance;
    [SerializeField]
    public float timeBetweenWaveSpawns = 2;

    private HashSet<GameObject> currentEnemies;
    [HideInInspector]
    public Wave.Waves currentWaveState;
    private Wave.Waves nextWaveState;
    private int waveNumber = 1;
    public Timer waveTimer = new Timer();

    [HideInInspector]
    public bool waveSpawnPaused = false;
    public bool modSelected = false;

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
        waveSpawnPaused = true;
        UIManager.instance.ToggleAugmentSelection(true);
    }

    private void LateUpdate()
    {
        if (!waveSpawnPaused)
        {
            CheckForEnemiesLeftAndChangeWaveState();
        }
    }

    private void CheckForEnemiesLeftAndChangeWaveState()
    {

        if (waveNumber != System.Enum.GetValues(typeof(Wave.Waves)).Length)
        {
            if (currentEnemies.Count == 0)
            {
                // end of a bosswave
                if (waveNumber!=1 && (waveNumber - 1) % 5 == 0)
                {
                    if (!modSelected)
                    {
                        Debug.Log("Boss Stage");
                        UIManager.instance.ToggleAugmentSelection(true);
                        waveSpawnPaused = true;
                    }
                    if (modSelected)
                    {
                        ChangeWaveState(nextWaveState);
                    }

                }
                // normal waves
                else
                {
                    ChangeWaveState(nextWaveState);
                }
            }
        }
    }

    private void ChangeWaveState(Wave.Waves nextWave)
    {
        if (nextWave != currentWaveState)
        {
            UIManager.instance.ToggleIngameUI(true);       
            if (waveTimer.timeCurrent <= 0)
            {
                currentWaveState = nextWave;
                waveNumber++;
                nextWaveState = (Wave.Waves)waveNumber;
                UIManager.instance.ToggleIngameUI(false);
                waveTimer.ResetTimer();
                modSelected = false;
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

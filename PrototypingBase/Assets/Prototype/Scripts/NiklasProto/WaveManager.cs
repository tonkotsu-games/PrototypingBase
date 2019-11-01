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

    public GameObject introCam; //Im sorry luca sam forced me to do this please hate him not me (or benni)

    private PlayerController playerScript;
    bool introPlaying = false;

    public static event System.Action OnGameEnd = delegate { };
    public static event System.Action OnFirstModStage = delegate { };

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
        //UIManager.instance.ToggleAugmentSelection(true);
        playerScript = Locator.instance.GetPlayerGameObject().GetComponent<PlayerController>();
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

        if (waveNumber <= System.Enum.GetValues(typeof(Wave.Waves)).Length)
        {
            if (currentEnemies.Count == 0)
            {
                // end of a bosswave
                if (waveNumber == 2 || waveNumber!=2 && (waveNumber - 2) % 5 == 0)
                {
                    if (!modSelected)
                    {
                        Debug.Log("Boss Stage");
                        introPlaying = false;
                        UIManager.instance.ToggleAugmentSelection(true);
                        waveSpawnPaused = true;
                        playerScript.enabled = false;
                        playerScript.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    }
                    else if (modSelected)
                    {
                        playerScript.enabled = true;
                        ChangeWaveState(nextWaveState);
                    }

                    if(waveNumber == 2)
                    {
                        OnFirstModStage();
                    }

                }
                // normal waves
                else
                {
                    ChangeWaveState(nextWaveState);
                }
            }
        }
        else
        {
            OnGameEnd();
        }
    }

    private void ChangeWaveState(Wave.Waves nextWave)
    {
        if (nextWave != currentWaveState)
        {
            UIManager.instance.ToggleIngameUI(true);       
            if (waveTimer.timeCurrent <= 0 || waveNumber == 1 )
            {
                if ((!introPlaying && waveNumber == 6) || (!introPlaying && waveNumber == 11))
                {
                    introPlaying = true;
                    playerScript.enabled = false;
                    playerScript.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    introCam.SetActive(true);
                    Camera.main.gameObject.SetActive(false);
                }
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

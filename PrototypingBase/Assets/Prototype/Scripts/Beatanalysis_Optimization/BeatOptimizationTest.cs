using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatOptimizationTest : MonoBehaviour
{
    public bool useOptimized = false;

    public List<float> beatList;

    public List<float> beatListCopy;

    private Timer timeTracker = new Timer();

    [SerializeField] bool checkInUpdate = true;

    private int beatsFound = 0;
    private int callsPerMinute = 0;


    private void Start()
    {
        beatListCopy = new List<float>(60);

        timeTracker.Set(0f);
        timeTracker.CountingDown = false;
        timeTracker.paused = false;

        //Define List      
        for (float i = 0.5f; i <= 60; i+=0.5f)
        {
            beatList.Add(i);
        }

        beatListCopy = new List<float>(beatList);
    }

    private void Update()
    {
        if(timeTracker.timeCurrent >60)
        {
            return;
        }

        timeTracker.Tick();

        if(checkInUpdate && !useOptimized)
        {
            if (CheckBeatUnoptimized(timeTracker.timeCurrent))
            {
                beatsFound++;
            }
        }
        else if(checkInUpdate && useOptimized)
        {
            if(CheckBeatOptimized(timeTracker.timeCurrent))
            {
                beatsFound++;
            }
        }
        
    }

    private void FixedUpdate()
    {
        if (timeTracker.timeCurrent > 60f)
        {
            Debug.LogError("Beats Found: " + beatsFound + "\n" + "Calls per Minute: " + callsPerMinute);
            return;
        }

        if (!checkInUpdate && !useOptimized)
        {
            if (CheckBeatUnoptimized(timeTracker.timeCurrent))
            {
                beatsFound++;
            }
        }
        else if (!checkInUpdate && useOptimized)
        {
            if (CheckBeatOptimized(timeTracker.timeCurrent))
            {
                beatsFound++;
            }
        }
    }

    private bool CheckBeatOptimized(float beatToCheck)
    {
        Debug.Log(beatListCopy.Count);
        bool found = false;

        float timeToCheck = beatToCheck * 10;
        timeToCheck = Mathf.Round(timeToCheck);
        timeToCheck = timeToCheck / 10;

        for (int i = 0; i < beatListCopy.Count - 1; i++)
        {
            callsPerMinute++;

            if (timeToCheck > beatListCopy[i])
            {
                beatListCopy.RemoveAt(i);
                i -= 1;
            }
            else if(timeToCheck == beatListCopy[i])
            {
                found = true;
                break;
            } 
        }

        return found;
    }

    private bool CheckBeatUnoptimized(float beatToCheck)
    {
        bool found = false;

        float timeToCheck = beatToCheck * 10;
        timeToCheck = Mathf.Round(timeToCheck);
        timeToCheck = timeToCheck / 10;

        Debug.Log("Checking for : " + timeToCheck + " beatToCheck: " + beatToCheck + " current Time: " + timeTracker.timeCurrent);

        for (int i = 0; i < beatListCopy.Count - 1; i++)
        {
            callsPerMinute++;

            if (timeToCheck == beatListCopy[i])
            {
                found = true;
            }
        }

        return found;
    }
}

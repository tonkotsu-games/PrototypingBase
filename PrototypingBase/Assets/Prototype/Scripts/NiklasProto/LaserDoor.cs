using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDoor : MonoBehaviour
{

    private Timer doorTimer = new Timer();

    [SerializeField]
    private float intervalTime = 2;

    [SerializeField]
    private GameObject laserWall;

    private bool isOpen;
    // Start is called before the first frame update
    void Start()
    {
        doorTimer.timeMax = intervalTime;
    }

    // Update is called once per frame
    void Update()
    {
        CheckDoorInterval();
    }

    private void CheckDoorInterval()
    {
        if (doorTimer.timeCurrent <= 0)
        {
            doorTimer.ResetTimer();

            bool state = isOpen ? false : true;
            ChangeDoorState(state);
        }
        else
        {
            doorTimer.Tick();
        }
    }

    private void ChangeDoorState(bool spawned)
    {
        laserWall.SetActive(spawned);
        isOpen = spawned;
    }
}

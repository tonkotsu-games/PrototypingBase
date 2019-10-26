using UnityEngine;

public class Timer
{
    public float timeCurrent = 0;
    public float timeMax = 0;
    public bool paused = false;
    private bool countingDown = true;

    public bool CountingDown { get => countingDown; set => countingDown = value; }

    public void Set(float max)
    {
        timeMax = max;
        timeCurrent = timeMax;
    }

    public void Tick()
    {
        if (!paused)
        {
            if (CountingDown)
            {
                timeCurrent -= Time.deltaTime;
            }
            else
            {
                timeCurrent += Time.deltaTime;
            }
        }
    }

    public void ResetTimer()
    {
        timeCurrent = timeMax;
    }

    public void TogglePause()
    {
        paused = !paused;
    }
}
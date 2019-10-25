using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStartAnimationEvent : MonoBehaviour
{
    public void AnimationOver()
    {
        WaveManager.instance.waveSpawnPaused = false;
    }
}

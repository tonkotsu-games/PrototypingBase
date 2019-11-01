using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStartAnimationEvent : MonoBehaviour
{
    private PlayerController playerScript;

    private void Start()
    {
        playerScript = Locator.instance.GetPlayerGameObject().GetComponent<PlayerController>();
    }
    public void AnimationOver()
    {
        WaveManager.instance.waveSpawnPaused = false;
    }

    public void LockPlayer()
    {
        playerScript.enabled = false;
    }

    public void UnlockPlayer()
    {
        playerScript.enabled = true;
    }
}

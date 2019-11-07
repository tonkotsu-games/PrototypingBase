using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndOfStartAnimationEvent : MonoBehaviour
{
    private PlayerNiklas playerScript;

    private void Start()
    {
        playerScript = Locator.instance.GetPlayerGameObject().GetComponent<PlayerNiklas>();
    }
    public void AnimationOver()
    {
        WaveManager.instance.waveSpawnPaused = false;
    }

    public void LockPlayer()
    {
        playerScript.enabled = false;
        playerScript.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public void UnlockPlayer()
    {
        playerScript.enabled = true;
    }
}

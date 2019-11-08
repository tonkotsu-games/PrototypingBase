using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashAnimEvents : MonoBehaviour
{
    PlayerNiklas playerScript;

    private void Start()
    {
        playerScript = gameObject.GetComponentInParent<PlayerNiklas>();

    }

    public void SlashRightNext()
    {
        playerScript.slashRight = true;
        playerScript.DisableSlashHitbox();
    }

    public void SlashLeftNext()
    {
        playerScript.slashRight = false;
        playerScript.DisableSlashHitbox();
    }

    public void EndAttackMove()
    {
        playerScript.inAttackMove = false;
    }
}

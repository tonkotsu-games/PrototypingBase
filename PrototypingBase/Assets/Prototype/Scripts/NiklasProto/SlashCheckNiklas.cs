using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCheckNiklas : MonoBehaviour
{
    PlayerNiklas playerScript;

    private void Start()
    {
        playerScript = GetComponentInParent<PlayerNiklas>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<MockupEnemyController>() != null)
        {
            playerScript.EnemyHit(other.gameObject);
        }
    }
}

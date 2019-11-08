using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCheckNiklas : MonoBehaviour
{
    [SerializeField]
    PlayerNiklas playerScript;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<MockupEnemyController>() != null)
        {
            playerScript.EnemyHit(other.gameObject);
        }
    }
}

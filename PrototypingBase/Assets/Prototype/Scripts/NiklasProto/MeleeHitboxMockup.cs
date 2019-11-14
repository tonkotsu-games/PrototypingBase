using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeHitboxMockup : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<PlayerNiklas>() != null)
        {
            GetComponentInParent<MockupEnemyController>().DamagePlayer(other.gameObject.GetComponent<PlayerNiklas>());
        }
    }
}

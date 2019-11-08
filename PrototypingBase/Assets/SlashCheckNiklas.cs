using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlashCheckNiklas : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<MockupEnemyController>() != null)
        {
            other.gameObject.GetComponent<MockupEnemyController>().Damage(2);
        }
    }
}

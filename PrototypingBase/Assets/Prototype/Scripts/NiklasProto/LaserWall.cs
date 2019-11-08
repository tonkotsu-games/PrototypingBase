using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWall : MonoBehaviour
{
    [SerializeField]
    private int damage = 5;
    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<PlayerController>() != null)
        {
            other.GetComponent<PlayerController>().Damage(damage);
        }
        else if(other.GetComponent<MockupEnemyController>() != null)
        {
            other.GetComponent<MockupEnemyController>().Damage(damage);
        }
    }
}

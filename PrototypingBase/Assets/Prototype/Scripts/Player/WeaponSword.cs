using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSword : MonoBehaviour
{
    [SerializeField]
    private int damageAmount = 0;

    private void Start()
    {
        Physics.IgnoreLayerCollision(9, 9, true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MockupEnemyController>().Damage(damageAmount);
        }
    }
}
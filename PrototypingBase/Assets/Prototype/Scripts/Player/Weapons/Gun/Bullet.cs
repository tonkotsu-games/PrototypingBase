using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField]
    private float bulletLifeTime;

    private int damageAmount = 0;

    public int DamageAmount { set => damageAmount = value; }

    void Update()
    {
        bulletLifeTime -= Time.deltaTime;
        if (bulletLifeTime <= 0)
        {
            Destroy(this.gameObject);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player" || collision.gameObject.tag != "Enemy")
        {
            Destroy(this.gameObject);
        }
        else if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<MockupEnemyController>().Damage(damageAmount);
            Destroy(this.gameObject);
        }
    }
}

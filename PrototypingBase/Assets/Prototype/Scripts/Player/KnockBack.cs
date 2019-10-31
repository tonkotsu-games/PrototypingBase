using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBack
{
    public void EnemyKnockback(List<GameObject> currentEnemy, float knockbackRange, float enemyKnockbackRange, Transform player)
    {
        for (int i = 0; i < currentEnemy.Count; i++)
        {
            if (Vector3.Distance(currentEnemy[i].GetComponent<Transform>().position, player.position) <= knockbackRange)
            {
                Vector3 direction = currentEnemy[i].GetComponent<Transform>().position - player.position;
                direction.y = 0;
                currentEnemy[i].GetComponent<Transform>().transform.position += direction.normalized * enemyKnockbackRange;
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAttackCheck : MonoBehaviour
{
    private bool hit = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hit)
        {
            Debug.Log("Boss Hit");
            //other.GetComponent<LifeCounter>().LifeChange();
            hit = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        hit = false;
    }
}
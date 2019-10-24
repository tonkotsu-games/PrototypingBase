using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake2 : MonoBehaviour
{
    
    [SerializeField] private Animator tutelev;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            tutelev.SetBool("TUTELE", true);
        }

    }
}

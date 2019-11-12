using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmyani1 : MonoBehaviour
{
    
    [SerializeField] private Animator Bluev4_8;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            Bluev4_8.SetBool("Bossplat", true);
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmyani : MonoBehaviour
{
    [SerializeField] private Animator elevators;
    

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            elevators.SetBool("elevator", true);
        
        }

    }
}

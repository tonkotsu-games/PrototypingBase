using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial_Elevator : MonoBehaviour
{
    
    [SerializeField] private Animator Arena_Platform;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            Arena_Platform.SetBool("Tutorial_Trigger", true);
        }

    }
}

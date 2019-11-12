using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCodersScript : MonoBehaviour
{
    
    [SerializeField] private Animator ArenaTransform;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            ArenaTransform.SetBool("TrigArenaChange", true);
        }

    }
}

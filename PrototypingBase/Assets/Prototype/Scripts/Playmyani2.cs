using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmyani2 : MonoBehaviour
{
    [SerializeField] private Animator NewArena;
    

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            NewArena.SetBool("ArenaChange", true);
            SkyboxChanger.instance.ChangeTexture(1);
        }

    }
}

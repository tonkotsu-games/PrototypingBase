using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmyani2 : MonoBehaviour
{
    [SerializeField] private Animator NewArena;

    bool playedArenaShake = false;
    

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
<<<<<<< HEAD
            NewArena.SetTrigger("ArenaChange");
=======
            NewArena.SetBool("ArenaChange", true);
            SkyboxChanger.instance.ChangeTexture(1);
        }
>>>>>>> Niklas

            if(!playedArenaShake)
            {
                Camera.main.GetComponent<Animator>().SetTrigger("ArenaShake");
                playedArenaShake = true;
            }        
        }
    }
}

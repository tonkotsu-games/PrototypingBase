using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playmyani2 : MonoBehaviour
{
    [SerializeField] private Animator NewArena;

    bool playedArenaShake = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            NewArena.SetTrigger("ArenaChange");
        }

        if (!playedArenaShake)
        {
            Camera.main.GetComponent<Animator>().SetTrigger("ArenaShake");
            playedArenaShake = true;
        }
    }
}


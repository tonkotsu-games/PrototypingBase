using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Playmyani : MonoBehaviour
{
    
    [SerializeField] private Animator Camera_Level_Animation;
    [SerializeField] GameObject Camera2;
    [SerializeField] GameObject Camera;
    GameObject player;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            Camera_Level_Animation.SetBool("Camera_Level_Cinematic", true);
            player.GetComponent<PlayerNiklas>().enabled = false;
            Camera2.enabled = true;
            Camera.enabled = false;
        }
        else
        {
            player.GetComponent<PlayerNiklas>().enabled = true;
            Camera2.enabled = false;
            Camera.enabled = true;
        }

    }
}

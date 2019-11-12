using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Playmyani : MonoBehaviour
{
    
    [SerializeField]
    private GameObject camera2;
    [SerializeField]
    private GameObject camera;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private Animator cameraLevelAnimation;

    private void OnTriggerEnter(Collider other)
   {
        if(other.CompareTag("Player"))
        {
            player.GetComponent<PlayerNiklas>().enabled = false;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
            camera2.SetActive(true);
            camera.SetActive(false);
            cameraLevelAnimation.SetBool("Camera_Level_Cinematic", true);
        }
    }

    public void EndOfAnimation()
    {
        player.GetComponent<PlayerNiklas>().enabled = true;
        camera2.SetActive(false);
        camera.SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Level_Cinematic : MonoBehaviour
{
    [SerializeField]
    GameObject player;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            player.GetComponent<PlayerNiklas>().enabled = false;
        }
        else
        {
            player.GetComponent<PlayerNiklas>().enabled = true;           
        }
    }
}

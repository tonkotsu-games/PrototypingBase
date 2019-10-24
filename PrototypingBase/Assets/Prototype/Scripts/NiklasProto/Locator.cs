using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator instance;
    [HideInInspector]
    public GameObject playerGO;


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        playerGO = GameObject.FindGameObjectWithTag("Player");
    }

    public GameObject GetPlayerGameObject()
    {
        return playerGO;
    }
}

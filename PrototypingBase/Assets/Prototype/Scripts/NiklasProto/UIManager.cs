using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    private GameObject ingameUI;

    [SerializeField]
    private GameObject augmentSelection;

    [SerializeField]
    private Button firstSelectedAugment;

    [SerializeField]
    private GameObject styleGrade;

    private PlayerController playerScript;

    private bool endscreen = false;

    private void Awake()
    {
        if(instance== null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        WaveManager.OnGameEnd += EnableStyleGrade;

        // i know this is horrible but we do a new project tomrrow so dont judge me
        playerScript = Locator.instance.GetPlayerGameObject().GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (endscreen)
        {
            if (Input.GetButton("Slide"))
            {
                SceneManager.LoadScene(0);
            }
        }
    }

    public void ToggleIngameUI(bool state)
    {
        ingameUI.SetActive(state);
    }
    public void ToggleAugmentSelection(bool state)
    {
        augmentSelection.SetActive(state);
        firstSelectedAugment.Select();
    }

   public void EnableStyleGrade()
    {
        ToggleIngameUI(false);
        styleGrade.SetActive(true);
        playerScript.enabled = false;
        endscreen = true;
    }

}

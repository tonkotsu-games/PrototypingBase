using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField]
    GameObject ingameUI;

    [SerializeField]
    GameObject augmentSelection;

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
    }

    public void ToggleIngameUI(bool state)
    {
        ingameUI.SetActive(state);
    }
    public void ToggleAugmentSelection(bool state)
    {
        augmentSelection.SetActive(state);
    }

}

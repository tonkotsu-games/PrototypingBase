using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private GameObject previousPanel;
    private GameObject currentPanel;

    [SerializeField]
    private GameObject mainPanel;

    [SerializeField]
    private GameObject characterPanel;

    [SerializeField]
    private GameObject optionsPanel;


    public void OnStartClick()
    {
        SceneManager.LoadScene(2);      
    }
    public void OnQuitClick()
    {
        Application.Quit();
    }

    public void OnCharacterClick()
    {
        previousPanel = mainPanel;
        mainPanel.SetActive(false);
        characterPanel.SetActive(true);
        currentPanel = characterPanel;
    }

    public void OnOptionsClick()
    {
        previousPanel = mainPanel;
        mainPanel.SetActive(false);
        optionsPanel.SetActive(true);
        currentPanel = optionsPanel;

    }
    
    public void OnBackClick()
    {
        previousPanel.SetActive(true);
        currentPanel.SetActive(false);
        GameObject temp = previousPanel;
        previousPanel = currentPanel;
        currentPanel = temp;        
    }
}

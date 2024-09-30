using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject creditsMenu;

    private bool showCredits = false;

    public void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");
    }
    public void ToggleCredits()
    {
        showCredits = !showCredits;
        mainMenu.SetActive(!showCredits);
        creditsMenu.SetActive(showCredits);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}

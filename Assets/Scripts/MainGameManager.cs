using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public enum PanelImage
{
    Background,
    CharacterLeft,
    CharacterRight
}
public enum PanelText
{
    MainPanelText,
    NamePanelText
}

public class MainGameManager : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void ContinueStory()
    {

    }

    public void SetImage(PanelImage targetImage, string newTexture)
    {
        Texture loadedTexture = Resources.Load<Texture>(newTexture);

        switch(targetImage)
        {
            case PanelImage.Background:
                GameObject.Find("MainPanel").GetComponent<UnityEngine.UIElements.Image>().image = loadedTexture;
                break;

            case PanelImage.CharacterLeft:
                GameObject.Find("CharacterPanelLeft").GetComponent<UnityEngine.UIElements.Image>().image = loadedTexture;
                break;

            case PanelImage.CharacterRight:
                GameObject.Find("CharacterPanelRight").GetComponent<UnityEngine.UIElements.Image>().image = loadedTexture;
                break;
        }
    }

    public void SetText(PanelText targetText, string newText)
    {
        switch (targetText)
        {
            case PanelText.MainPanelText:
                GameObject.Find("MainTextBox").GetComponent<Text>().text = newText;
                break;

            case PanelText.NamePanelText:
                GameObject.Find("NameTextBox").GetComponent<Text>().text = newText;
                break;
        }
    }
}

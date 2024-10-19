using LLMUnity;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
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
    public StoryManager storyManager;
    public TextInputManager textInputManager;
    public DrawingManager drawingManager;
    public GameObject mainCanvas;
    public CanvasGroup mainCanvasGroup;

    public UnityEngine.UI.Image mainPanelImage;
    public UnityEngine.UI.Image characterPanelLeftImage;
    public UnityEngine.UI.Image characterPanelRightImage;

    //public void Start()
    //{
    //    storyManager.Continue();
    //}

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }
    
    public void ContinueStory()
    {
        Debug.LogError("Continuing za story");
        storyManager.Continue();
    }

    public void SubmitStoryNode(StoryNode storyNode)
    {
        switch(storyNode.storyNodeType)
        {
            case StoryNodeType.OutputComplete:
                mainCanvas.SetActive(true);
                UnpackOutputStoryNode(storyNode);
                break;
            case StoryNodeType.OutputIncomplete:
                mainCanvas.SetActive(true);
                UnpackOutputStoryNode(storyNode);
                break;
            case StoryNodeType.TextInput:
                mainCanvas.SetActive(true);
                textInputManager.EnableTextInput();
                break;
            case StoryNodeType.DrawInput:
                drawingManager.EnableDrawing();
                mainCanvas.SetActive(true);
                mainCanvasGroup.interactable = false;
                mainCanvasGroup.blocksRaycasts = false;
                break;
        }
    }

    private void UnpackOutputStoryNode(StoryNode storyNode)
    {
        SetText(PanelText.NamePanelText, storyNode.activeCharacterName);
        SetText(PanelText.MainPanelText, storyNode.dialogueBoxText);

        if (storyNode.background != null)
            SetImage(PanelImage.Background, storyNode.background);

        SetImage(PanelImage.CharacterLeft, storyNode.characterLeft.sprite);
        SetImage(PanelImage.CharacterRight, storyNode.characterRight.sprite);
    }

    public void SetImage(PanelImage targetImage, Sprite sprite)
    {
        switch (targetImage)
        {
            case PanelImage.Background:
                mainPanelImage.sprite = sprite;
                break;

            case PanelImage.CharacterLeft:
                characterPanelLeftImage.sprite = sprite;
                break;

            case PanelImage.CharacterRight:
                characterPanelRightImage.sprite = sprite;
                break;
        }
    }

    public void SetText(PanelText targetText, string newText)
    {
        switch (targetText)
        {
            case PanelText.MainPanelText:
                GameObject.Find("MainTextBox").GetComponent<TMP_Text>().text = newText;
                break;

            case PanelText.NamePanelText:
                GameObject.Find("NameTextBox").GetComponent<TMP_Text>().text = newText;
                break;
        }
    }

    internal void SubmitDrawingRecognizedObject(string recognizedObject)
    {
        storyManager.SetRecognizedDrawingObjectString(recognizedObject);
    }
}

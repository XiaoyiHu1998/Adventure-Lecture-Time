using LLMUnity;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using System;
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
    public ScrollRect scrollRect;

    public UnityEngine.UI.Image mainPanelImage;
    public UnityEngine.UI.Image characterPanelLeftImage;
    public UnityEngine.UI.Image characterPanelRightImage;

    private bool canClickContinueButton = false;
    private Coroutine textRevealCoroutine;
    private string fullMainPanelText;
    private string currentMainPanelText = "";

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    private void ContinueStory()
    {
        Debug.Log("Continuing za story");
        storyManager.Continue();
    }

    public void ContinueStoryButton()
    {
        if (textRevealCoroutine != null)
        {
            StopCoroutine(textRevealCoroutine);
            textRevealCoroutine = null;
            currentMainPanelText = fullMainPanelText;
            GameObject.Find("MainTextScrollViewContent").GetComponent<TMP_Text>().text = fullMainPanelText;
            Canvas.ForceUpdateCanvases();
            StartCoroutine(SetScrollPositionToBottom());
        }
        else if(canClickContinueButton)
        {
            ContinueStory();
        }
    }

    // Set the scroll view to the bottom, need to wait until the text has been properly updated
    private IEnumerator SetScrollPositionToBottom()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return null; // Wait for the next frame
        }
        scrollRect.verticalNormalizedPosition = 0f; // Set the scroll view to the bottom
    }

    public void ContinueInputComplete()
    {
        canClickContinueButton = true;
        ContinueStoryButton();
    }

    public void SubmitStoryNode(StoryNode storyNode)
    {
        switch(storyNode.storyNodeType)
        {
            case StoryNodeType.OutputComplete:
                mainCanvasGroup.interactable = true;
                mainCanvasGroup.blocksRaycasts = true;
                canClickContinueButton = true;
                UnpackOutputStoryNode(storyNode);
                break;
            case StoryNodeType.OutputIncomplete:
                mainCanvasGroup.interactable = true;
                mainCanvasGroup.blocksRaycasts = true;
                canClickContinueButton = false;
                UnpackOutputStoryNode(storyNode);
                break;
            case StoryNodeType.TextInput:
                mainCanvasGroup.interactable = true;
                mainCanvasGroup.blocksRaycasts = true;
                canClickContinueButton = false;
                textInputManager.EnableTextInput();
                break;
            case StoryNodeType.DrawInput:
                drawingManager.EnableDrawing();
                mainCanvasGroup.interactable = false;
                mainCanvasGroup.blocksRaycasts = false;
                canClickContinueButton = false;
                break;
        }
    }

    private void UnpackOutputStoryNode(StoryNode storyNode)
    {
        SetText(PanelText.NamePanelText, storyNode.activeCharacterName);
        SetText(PanelText.MainPanelText, storyNode.dialogueBoxText);

        if (storyNode.background != null)
        {
            Debug.Log("Setting background image");
            Debug.Log(storyNode.background);
            SetImage(PanelImage.Background, storyNode.background);
        }
            

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
                TMP_Text mainText = GameObject.Find("MainTextScrollViewContent").GetComponent<TMP_Text>();
                if (textRevealCoroutine != null)
                {
                    StopCoroutine(textRevealCoroutine);
                }
                textRevealCoroutine = StartCoroutine(RevealText(mainText, currentMainPanelText, newText));
                fullMainPanelText = newText;
                break;

            case PanelText.NamePanelText:
                GameObject.Find("NameTextBox").GetComponent<TMP_Text>().text = newText;
                break;
        }
    }

    private IEnumerator RevealText(TMP_Text textComponent, string prevText, string fullText)
    {
        int i;
        if (fullText.StartsWith(prevText))
        {
            i = prevText.Length;
        }
        else
        {
            currentMainPanelText = "";
            textComponent.text = "";
            i = 0;
        }

        for (; i < fullText.Length; i++)
        {
            currentMainPanelText += fullText[i];
            textComponent.text = currentMainPanelText;
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f; // Set the scroll view to the bottom
            yield return new WaitForSeconds(0.05f);
        }

        textRevealCoroutine = null;
    }

    public void ToggleLeftCharacter(bool active)
    {
        characterPanelLeftImage.gameObject.SetActive(active);
    }

    public void ToggleRightCharacter(bool active)
    {
        characterPanelRightImage.gameObject.SetActive(active);
    }
    public void ScrollToTop()
    {
        scrollRect.verticalNormalizedPosition = 1f; // 1 means top
    }

    internal void SubmitDrawingRecognizedObject(string recognizedObject)
    {
        storyManager.SetRecognizedDrawingObjectString(recognizedObject);
    }
}

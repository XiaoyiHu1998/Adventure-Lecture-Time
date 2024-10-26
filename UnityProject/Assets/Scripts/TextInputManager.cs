using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputManager : MonoBehaviour
{
    public GameObject textInput;
    private bool textInputEnabled;
    public StoryManager storyManager;

    public TMPro.TMP_InputField textInputField;

    private void Start()
    {
        textInputEnabled = textInput.activeInHierarchy;
    }

    public void EnableTextInput()
    {
        Debug.Log("enabling text input");
        textInputEnabled = true;
        textInput.SetActive(textInputEnabled);
        FocusTextInput();
    }

    public void ToggleTextInput()
    {
        textInputEnabled = !textInputEnabled;
        textInput.SetActive(textInputEnabled);
        if (textInputEnabled)
        {
            FocusTextInput();
        }
    }

    public void HandleTextInputSubmitted()
    {
        string inputText = textInputField.text;
        if (inputText == "")
        {
            FocusTextInput();
            return;
        }
        storyManager.SubmitInputText(inputText);

        Debug.Log(inputText);
        textInputField.text = "";
        ToggleTextInput();
        storyManager.mainGameManager.ContinueInputComplete();
    }

    private void FocusTextInput()
    {
        textInputField.Select(); // Set the input field as the selected UI element
        textInputField.ActivateInputField(); // Activate the input field to start receiving input
    }
}

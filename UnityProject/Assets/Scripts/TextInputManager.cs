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
    }

    public void ToggleTextInput()
    {
        textInputEnabled = !textInputEnabled;
        textInput.SetActive(textInputEnabled);
    }

    public void HandleTextInputSubmitted()
    {
        string inputText = textInputField.text;
        storyManager.SubmitInputText(inputText);

        Debug.Log(inputText);
        textInputField.text = "";
        ToggleTextInput();
    }


}

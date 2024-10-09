using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextInputManager : MonoBehaviour
{
    public GameObject textInput;
    private bool textInputEnabled;

    public TMPro.TMP_Text textInputText;

    private void Start()
    {
        textInputEnabled = textInput.activeInHierarchy;
    }

    public void ToggleTextInput()
    {
        textInputEnabled = !textInputEnabled;
        textInput.SetActive(textInputEnabled);
    }

    public void HandleTextInputSubmitted()
    {
        string inputText = textInputText.text;

        Debug.LogError(inputText);
        ToggleTextInput();
    }


}

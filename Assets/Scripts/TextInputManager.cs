using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInputManager : MonoBehaviour
{
    public GameObject textInput;
    //public Text textInputText;
    public TMPro.TMP_Text textInputText;
    private bool textInputEnabled = false;

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

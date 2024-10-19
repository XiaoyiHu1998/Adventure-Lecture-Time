using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public MainGameManager mainGameManager;
    private string m_ObjectString;

    private void Start()
    {
        DisableDrawing();
    }

    public void EnableDrawing()
    {
        gameObject.SetActive(true);
    }

    public void DisableDrawing()
    {
        gameObject.SetActive(false);
    }

    public void SetRecognizedObjectString(string objectString)
    {
        m_ObjectString = objectString;
        mainGameManager.SubmitDrawingRecognizedObject(m_ObjectString);
        DisableDrawing();
        mainGameManager.ContinueStory();
    }

    public string GetRecognizedObjectString()
    {
        return m_ObjectString;
    }
}

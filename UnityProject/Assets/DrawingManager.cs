using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public MainGameManager mainGameManager;
    public GameObject drawingCanvas;

    [System.NonSerialized]
    public string targetObject = null; // The object that the player should draw
    [System.NonSerialized]
    public int topNTargetObjects = 3; // How high the target object should be in the list of recognized objects
    [System.NonSerialized]
    public bool allowNonTargetObjects = true; // Whether the player can submit a recognized object that is not the target object
    private string m_ObjectString;

    private void Start()
    {
        DisableDrawing();
    }

    public void EnableDrawing()
    {
        gameObject.SetActive(true);
        drawingCanvas.SetActive(true);
    }

    public void DisableDrawing()
    {
        gameObject.SetActive(false);
        drawingCanvas.SetActive(false);
    }

    public void SetRecognizedObjectString(string objectString)
    {
        m_ObjectString = objectString;
        mainGameManager.SubmitDrawingRecognizedObject(m_ObjectString);
        DisableDrawing();
        mainGameManager.ContinueInputComplete();
    }

    public string GetRecognizedObjectString()
    {
        return m_ObjectString;
    }
}

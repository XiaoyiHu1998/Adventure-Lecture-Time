using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingManager : MonoBehaviour
{
    public GameObject DrawingWriteEnabledImageToDrawOn;
    private bool drawingEnabled = false;
    public string m_ObjectString { get; private set; }

    private void Start()
    {
        drawingEnabled = false;
        SetActiveDrawingWriteEnabledImageToDrawOn(drawingEnabled);
    }

    private void SetActiveDrawingWriteEnabledImageToDrawOn(bool enabled)
    {
        DrawingWriteEnabledImageToDrawOn.SetActive(enabled);
    }

    public void SetDrawingActive()
    {
        DrawingWriteEnabledImageToDrawOn.SetActive(true);
    }

    public void EnableDrawing()
    {
        DrawingWriteEnabledImageToDrawOn.SetActive(false);
    }

    public void SetRecognizedObjectString(string objectString)
    {
        m_ObjectString = objectString;
    }
}

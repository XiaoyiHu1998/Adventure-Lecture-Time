using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ControlNet : MonoBehaviour
{
    public Texture2D inputTexture;
    public Texture2D outputTexture;
    public GameObject drawingCanvas;
    
    public void Start()
    {
        DrawControlNet("cat");
    }

    public void DrawControlNet(string className)
    {
        StartCoroutine(GetControlNet(className));
    }

    private IEnumerator GetControlNet(string className)
    {
        Debug.Log("Getting control net for " + className);
        Debug.Log("Input texture size: " + inputTexture.width + "x" + inputTexture.height);
        string url = "http://localhost:7860/sdapi/v1/txt2img";
        Dictionary<string, object> json = new Dictionary<string, object>()
        {
            { "prompt", "a " + className + " in anime style" },
            { "steps", 20 },
            { "width", inputTexture.width },
            { "height", inputTexture.height }
        }; 
        string jsonStr = JsonConvert.SerializeObject(json);
        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);
        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            Debug.Log("Sending request to " + url);
            yield return request.SendWebRequest();
            Debug.Log("Request sent");
            if (request.isNetworkError || request.isHttpError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                string response = request.downloadHandler.text;
                JObject jsonResponse = JObject.Parse(response);
                Debug.Log(jsonResponse);
                var image = jsonResponse["images"][0].ToString();
                outputTexture.LoadImage(System.Convert.FromBase64String(image));
                Sprite sprite = Sprite.Create(outputTexture, new Rect(0, 0, outputTexture.width, outputTexture.height), new Vector2(0.5f, 0.5f));
                drawingCanvas.GetComponent<SpriteRenderer>().sprite = sprite;
            }
        }
    }
}

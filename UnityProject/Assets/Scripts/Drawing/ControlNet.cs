using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Drawing 
{
    public class ControlNet : MonoBehaviour
    {
        public Texture2D inputTexture;
        public Texture2D outputTexture;
        public GameObject drawingCanvas;
        public GameObject outputImage;
        public GameObject continueButton;
        public GameObject loadingPanel;
        
        private float fadeDuration = 1.0f;

        public void DrawControlNet(string className)
        {
            StartCoroutine(GetControlNet(className));
        }

        private IEnumerator GetControlNet(string className)
        {
            string url = "http://localhost:7860/sdapi/v1/txt2img";
            byte[] inputBytes = inputTexture.EncodeToPNG();
            string inputBase64 = System.Convert.ToBase64String(inputBytes);
            Dictionary<string, object> json = new Dictionary<string, object>()
            {
                { "prompt", "a " + className + " in anime style" },
                { "negative_prompt", ""},
                { "steps", 20 },
                { "batch_size", 1 },
                { "cfg_scale", 7},
                { "width", inputTexture.width / 2 },
                { "height", inputTexture.height / 2 },
                { "override_settings", new Dictionary<string, object>()
                    {
                        { "sd_model_checkpoint", "v1-5-pruned-emaonly" }
                    }
                },
                { "sampler_name", "Euler" },
                { "alwayson_scripts", new Dictionary<string, object>()
                    {
                        { "controlnet", new Dictionary<string, object>()
                            {
                                { "args", new List<object>()
                                    {
                                        new Dictionary<string, object>()
                                        {
                                            { "enabled", true },
                                            { "module", "invert (from white bg & black line)" },
                                            { "model",  "control_lora_rank128_v11p_sd15_scribble_fp16" },
                                            { "image", inputBase64 },
                                            { "resize_mode", "Crop and Resize" },
                                            { "low_vram", false },
                                            { "guidance_start", 0.0f },
                                            { "guidance_end", 1.0f },
                                            { "control_mode", "ControlNet is more important" },
                                            { "pixel_perfect", true }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }; 
            string jsonStr = JsonConvert.SerializeObject(json);
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(jsonStr);
            using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
            {
                request.uploadHandler = new UploadHandlerRaw(bytes);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError(request.error);
                }
                else
                {
                    string response = request.downloadHandler.text;
                    JObject jsonResponse = JObject.Parse(response);
                    var image = jsonResponse["images"][0].ToString();
                    outputTexture.LoadImage(System.Convert.FromBase64String(image));
                    Sprite sprite = Sprite.Create(outputTexture, new Rect(0, 0, outputTexture.width, outputTexture.height), new Vector2(0.5f, 0.5f));
                    SpriteRenderer inputSpriteRenderer = drawingCanvas.GetComponent<SpriteRenderer>();
                    SpriteRenderer outputSpriteRenderer = outputImage.GetComponent<SpriteRenderer>();
                    StretchSprite(outputSpriteRenderer);
                    StartCoroutine(FadeSprites(inputSpriteRenderer, outputSpriteRenderer, sprite, fadeDuration));
                }
                loadingPanel.SetActive(false);
                continueButton.SetActive(true);
            }
        }

        private IEnumerator FadeSprites(SpriteRenderer spriteRenderer1, SpriteRenderer spriteRenderer2, Sprite newSprite, float duration)
        {
            spriteRenderer2.sprite = newSprite;
            spriteRenderer2.color = new Color(spriteRenderer2.color.r, spriteRenderer2.color.g, spriteRenderer2.color.b, 0f);

            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float alpha1 = Mathf.Lerp(1f, 0f, elapsedTime / duration);
                float alpha2 = Mathf.Lerp(0f, 1f, elapsedTime / duration);
                spriteRenderer1.color = new Color(spriteRenderer1.color.r, spriteRenderer1.color.g, spriteRenderer1.color.b, alpha1);
                spriteRenderer2.color = new Color(spriteRenderer2.color.r, spriteRenderer2.color.g, spriteRenderer2.color.b, alpha2);
                yield return null;
            }

            spriteRenderer1.enabled = false;
            spriteRenderer2.color = new Color(spriteRenderer2.color.r, spriteRenderer2.color.g, spriteRenderer2.color.b, 1f);
        }

        public void StretchSprite(SpriteRenderer spriteRenderer)
        {
            if (spriteRenderer != null)
            {
                float xScale = (float)inputTexture.width / (float)outputTexture.width;
                float yScale = (float)inputTexture.height / (float)outputTexture.height;
                spriteRenderer.transform.localScale = new Vector3(xScale, yScale, 1.0f);
            }
        }
    }
}
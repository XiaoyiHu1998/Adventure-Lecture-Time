using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Debug = UnityEngine.Debug;
using System.Linq;
using UnityEngine.UI;
using TMPro;

namespace FreeDraw
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(Collider2D))]  // REQUIRES A COLLIDER2D to function
    // 1. Attach this to a read/write enabled sprite image
    // 2. Set the drawing_layers  to use in the raycast
    // 3. Attach a 2D collider (like a Box Collider 2D) to this sprite
    // 4. Hold down left mouse to draw on this texture!
    public class Drawable : MonoBehaviour
    {
        public DrawingManager drawingManager;
        // PEN COLOUR
        public static Color Pen_Colour = Color.black;     // Change these to change the default drawing settings
        // PEN WIDTH (actually, it's a radius, in pixels)
        public static int Pen_Width = 5;


        public delegate void Brush_Function(Vector2 world_position);
        // This is the function called when a left click happens
        // Pass in your own custom one to change the brush type
        // Set the default function in the Awake method
        public Brush_Function current_brush;

        public LayerMask Drawing_Layers;

        public bool Reset_Canvas_On_Play = true;
        // The colour the canvas is reset to each time
        public Color Reset_Colour = new Color(0, 0, 0, 0);  // By default, reset the canvas to be transparent
		
		public bool Reset_To_This_Texture_On_Play = false;	// If true, will reset the image back to whatever reset texture is
		public Texture2D reset_texture;

        // Used to reference THIS specific file without making all methods static
        public static Drawable drawable;
        // MUST HAVE READ/WRITE enabled set in the file editor of Unity
        Sprite drawable_sprite;
        Texture2D drawable_texture;

        Vector2 previous_drag_position;
        Color[] clean_colours_array;
        Color transparent;
        Color32[] cur_colors;
        List<Color32[]> prev_colors = new List<Color32[]>();
        bool mouse_was_previously_held_down = false;
        bool no_drawing_on_current_drag = false;
        List<List<List<int>>> strokes = new List<List<List<int>>>();
        List<string> predictions = new List<string>();
        int strokes_undone = 0; // strokes that have been undone but have not yet been predicted
        Button submitButton;
        TMP_Text predictionText;




//////////////////////////////////////////////////////////////////////////////
// BRUSH TYPES. Implement your own here
// How to write your own brush method:
// 1. Copy and rename the BrushTemplate() method below with your own brush
// 2. Write your own code inside of this method
// 3. Assign this method to the current_brush variable (see how PenBrush does this)


        // When you want to make your own type of brush effects,
        // Copy, paste and rename this function.
        // Go through each step
        public void BrushTemplate(Vector2 world_position)
        {
            // 1. Change world position to pixel coordinates
            Vector2 pixel_pos = WorldToPixelCoordinates(world_position);

            // 2. Make sure our variable for pixel array is updated in this frame
            cur_colors = drawable_texture.GetPixels32();

            ////////////////////////////////////////////////////////////////
            // FILL IN CODE BELOW HERE

            // Do we care about the user left clicking and dragging?
            // If you don't, simply set the below if statement to be:
            //if (true)

            // If you do care about dragging, use the below if/else structure
            if (previous_drag_position == Vector2.zero)
            {
                // THIS IS THE FIRST CLICK
                // FILL IN WHATEVER YOU WANT TO DO HERE
                // Maybe mark multiple pixels to colour?
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // THE USER IS DRAGGING
                // Should we do stuff between the previous mouse position and the current one?
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ////////////////////////////////////////////////////////////////

            // 3. Actually apply the changes we marked earlier
            // Done here to be more efficient
            ApplyMarkedPixelChanges();
            
            // 4. If dragging, update where we were previously
            previous_drag_position = pixel_pos;
        }


        private void AddStroke(Vector2 pixel_pos)
        {            
            if (previous_drag_position == Vector2.zero)
            {
                submitButton.interactable = false;
                predictionText.text = "..............";
                // Start a new stroke if we're not dragging already
                strokes.Add(new List<List<int>>() {
                    new List<int>(),
                    new List<int>()
                });
                strokes[^1][0].Add((int)pixel_pos.x);
                strokes[^1][1].Add((int)(drawable_texture.height - pixel_pos.y));
            }
            else if (previous_drag_position != pixel_pos)
            {
                strokes[^1][0].Add((int)pixel_pos.x);
                strokes[^1][1].Add((int)(drawable_texture.height - pixel_pos.y));
            }
        }
        
        // Default brush type. Has width and colour.
        // Pass in a point in WORLD coordinates
        // Changes the surrounding pixels of the world_point to the static pen_colour
        public void PenBrush(Vector2 pixel_pos)
        {
            // if (previous_drag_position.x - pixel_pos.x >= 1 || previous_drag_position.y - pixel_pos.y >= 1)
            // {
            //     Debug.Log("PenBrush at " + pixel_pos_topleft);
            // }

            cur_colors = drawable_texture.GetPixels32();

            if (previous_drag_position == Vector2.zero)
            {
                // If this is the first time we've ever dragged on this image, simply colour the pixels at our mouse position
                MarkPixelsToColour(pixel_pos, Pen_Width, Pen_Colour);
            }
            else
            {
                // Colour in a line from where we were on the last update call
                ColourBetween(previous_drag_position, pixel_pos, Pen_Width, Pen_Colour);
            }
            ApplyMarkedPixelChanges();

            //Debug.Log("Dimensions: " + pixelWidth + "," + pixelHeight + ". Units to pixels: " + unitsToPixels + ". Pixel pos: " + pixel_pos);
        }


        // Helper method used by UI to set what brush the user wants
        // Create a new one for any new brushes you implement
        public void SetPenBrush()
        {
            // PenBrush is the NAME of the method we want to set as our current brush
            current_brush = PenBrush;
        }

        //////////////////////////////////////////////////////////////////////////////


        private List<List<List<int>>> ScaleStrokes(List<List<List<int>>> strokes) 
        {   
            // Get the maximum and minimum x and y values
            int min_x = int.MaxValue;
            int max_x = int.MinValue;
            int min_y = int.MaxValue;
            int max_y = int.MinValue;
        
            foreach (List<List<int>> stroke in strokes)
            {
                foreach (int point in stroke[0])
                {
                    min_x = Mathf.Min(min_x, point);
                    max_x = Mathf.Max(max_x, point);
                }
                foreach (int point in stroke[1])
                {
                    min_y = Mathf.Min(min_y, point);
                    max_y = Mathf.Max(max_y, point);
                }
            }
        
            List<List<List<int>>> scaled_strokes = new List<List<List<int>>>();
            // Align and scale the strokes
            float scale_factor = 255f / Mathf.Max(max_x - min_x, max_y - min_y);
            foreach (var stroke in strokes)
            {
                List<List<int>> scaledStroke = new List<List<int>> { new List<int>(), new List<int>() };
                for (int j = 0; j < stroke[0].Count; j++)
                {
                    scaledStroke[0].Add((int)((stroke[0][j] - min_x) * scale_factor));
                    scaledStroke[1].Add((int)((stroke[1][j] - min_y) * scale_factor));
                }
                scaled_strokes.Add(scaledStroke);
            }

            // Remove duplicate points and interpolate between them
            for (int i = 0; i < scaled_strokes.Count; i++)
            {
                List<List<int>> newStroke = new List<List<int>> { new List<int>() {scaled_strokes[i][0][0]}, new List<int>() {scaled_strokes[i][1][0]} };
                for (int j = 1; j < scaled_strokes[i][0].Count; j++)
                {
                    if (scaled_strokes[i][0][j] == scaled_strokes[i][0][j - 1] && scaled_strokes[i][1][j] == scaled_strokes[i][1][j - 1])
                    {
                        continue;
                    }
                    Vector2 start_point = new Vector2(scaled_strokes[i][0][j - 1], scaled_strokes[i][1][j - 1]);
                    Vector2 end_point = new Vector2(scaled_strokes[i][0][j], scaled_strokes[i][1][j]);
                    StrokeBetween(start_point, end_point, newStroke);
                }
                scaled_strokes[i] = newStroke;
            }
            return scaled_strokes;
        }


        public void SubmitDrawing()
        {
            if (strokes.Count == predictions.Count)
            {
                drawingManager.SetRecognizedObjectString(predictions[^1]);               
            }
        }

        // This is where the magic happens.
        // Detects when user is left clicking, which then call the appropriate function
        void Update()
        {
            // Is the user holding down the left mouse button?
            bool mouse_held_down = Input.GetMouseButton(0);
            
            // Convert mouse coordinates to world coordinates
            Vector2 mouse_world_position = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Check if the current mouse position overlaps our image
            Collider2D hit = Physics2D.OverlapPoint(mouse_world_position, Drawing_Layers.value);
            
            if (mouse_held_down && !no_drawing_on_current_drag)
            {
                if (hit != null && hit.transform != null)
                {
                    // We're over the texture we're drawing on!
                    // Use whatever function the current brush is
                    if (previous_drag_position == Vector2.zero)
                    {
                        prev_colors.Add(drawable_texture.GetPixels32());
                    }
                    Vector2 pixel_pos = WorldToPixelCoordinates(mouse_world_position);
                    current_brush(pixel_pos);
                    AddStroke(pixel_pos);
                    previous_drag_position = pixel_pos;
                }

                else
                {
                    if (previous_drag_position != Vector2.zero)
                    {
                        // We were drawing, but now we're not over the canvas
                        // This means we've stopped drawing
                        PredictStrokes(strokes);
                    }
                    // We're not over our destination texture
                    previous_drag_position = Vector2.zero;
                    if (!mouse_was_previously_held_down)
                    {
                        // This is a new drag where the user is left clicking off the canvas
                        // Ensure no drawing happens until a new drag is started
                        no_drawing_on_current_drag = true;
                    } 
                }
            }
            // Mouse is released
            else if (!mouse_held_down)
            {
                previous_drag_position = Vector2.zero;
                no_drawing_on_current_drag = false;
                if (mouse_was_previously_held_down && hit != null && hit.transform != null)
                {
                    // This means the user has released the mouse button
                    // We can consider this the end of a drawing stroke
                    PredictStrokes(strokes);
                }
            }
            mouse_was_previously_held_down = mouse_held_down;
        }

        public void PredictStrokes(List<List<List<int>>> strokes) 
        {
            if (strokes.Count == 0)
            {
                return;
            }
            List<List<List<int>>> scaled_strokes = ScaleStrokes(strokes);
            string json = JsonConvert.SerializeObject(scaled_strokes);
            for (int i = 0; i < scaled_strokes.Count; i++)
            {
                scaled_strokes[i] = RamerDouglasPeucker(scaled_strokes[i]);
            }
            json = JsonConvert.SerializeObject(new { drawing = scaled_strokes });
            StartCoroutine(SendJsonRequest("http://127.0.0.1:5000/predict", json));

        }

        private IEnumerator SendJsonRequest(string url, string json)
        {
            using UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else if (request.result == UnityWebRequest.Result.Success)
            {
                string response = request.downloadHandler.text;
                JObject jsonResponse = JObject.Parse(response);
                string classValue = jsonResponse["class"]?.ToString();
                if(strokes_undone > 0)
                {
                    strokes_undone--;
                }
                else
                {
                    predictions.Add(classValue);
                }
                if (predictions.Count == strokes.Count && strokes.Count > 0)
                {
                    submitButton.interactable = true;
                    predictionText.text = classValue;
                }
            }
        }

        private List<List<int>> RamerDouglasPeucker(List<List<int>> stroke, float epsilon = 2.0f)
        {
            // Find the point with the maximum distance
            float dmax = 0;
            int index = 0;
            for (int j = 1; j < stroke[0].Count - 1; j++)
            {
                float d = PerpendicularDistance(stroke[0][j], stroke[1][j], stroke[0][0], stroke[1][0], stroke[0][^1], stroke[1][^1]);
                if (d > dmax)
                {
                    index = j;
                    dmax = d;
                }
            }

            List<List<int>> resultStroke = new List<List<int>>
            {
                new List<int>(),
                new List<int>()
            };
            if (dmax > epsilon)
            {
                List<List<int>> left = new List<List<int>> { stroke[0].GetRange(0, index + 1), stroke[1].GetRange(0, index + 1) };
                List<List<int>> right = new List<List<int>> { stroke[0].GetRange(index, stroke[0].Count - index), stroke[1].GetRange(index, stroke[1].Count - index) };
                // Recursive call
                List<List<int>> recResults1 = RamerDouglasPeucker(left, epsilon);
                List<List<int>> recResults2 = RamerDouglasPeucker(right, epsilon);

                // Build the result list
                resultStroke[0].AddRange(recResults1[0].GetRange(0, recResults1[0].Count - 1));
                resultStroke[0].AddRange(recResults2[0]);
                resultStroke[1].AddRange(recResults1[1].GetRange(0, recResults1[1].Count - 1));
                resultStroke[1].AddRange(recResults2[1]);
            }
            else
            {
                resultStroke[0].Add(stroke[0][0]);
                resultStroke[1].Add(stroke[1][0]);
                resultStroke[0].Add(stroke[0][^1]);
                resultStroke[1].Add(stroke[1][^1]);
            }
            return resultStroke;
        }

        private float PerpendicularDistance(int x, int y, int x1, int y1, int x2, int y2)
        {
            float A = x - x1;
            float B = y - y1;
            float C = x2 - x1;
            float D = y2 - y1;

            float dot = A * C + B * D;
            float len_sq = C * C + D * D;
            float param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                // Point is before the line from x1, y1 to x2, y2
                // Closest point is the start point
                xx = x1;
                yy = y1;
            }
            else if (param > 1)
            {
                // Point is after the line from x1, y1 to x2, y2
                // Closest point is the end point
                xx = x2;
                yy = y2;
            }
            else
            {
                xx = x1 + param * C;
                yy = y1 + param * D;
            }

            return Mathf.Sqrt((x - xx) * (x - xx) + (y - yy) * (y - yy));
        }

        public void StrokeBetween(Vector2 start_point, Vector2 end_point, List<List<int>> stroke)
        {
            float distance = Vector2.Distance(start_point, end_point);

            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                Vector2 cur_position = Vector2.Lerp(start_point, end_point, lerp);
                stroke[0].Add((int)cur_position.x);
                stroke[1].Add((int)cur_position.y);            
            }
        }

        // Set the colour of pixels in a straight line from start_point all the way to end_point, to ensure everything inbetween is coloured
        public void ColourBetween(Vector2 start_point, Vector2 end_point, int width, Color color)
        {
            // Get the distance from start to finish
            float distance = Vector2.Distance(start_point, end_point);

            // Calculate how many times we should interpolate between start_point and end_point based on the amount of time that has passed since the last update
            float lerp_steps = 1 / distance;

            for (float lerp = 0; lerp <= 1; lerp += lerp_steps)
            {
                Vector2 cur_position = Vector2.Lerp(start_point, end_point, lerp);
                MarkPixelsToColour(cur_position, width, color);
            }
        }





        public void MarkPixelsToColour(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                // Check if the X wraps around the image, so we don't draw pixels on the other side of the image
                if (x >= (int)drawable_sprite.rect.width || x < 0)
                    continue;

                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    MarkPixelToChange(x, y, color_of_pen);
                }
            }
        }
        public void MarkPixelToChange(int x, int y, Color color)
        {
            // Need to transform x and y coordinates to flat coordinates of array
            int array_pos = y * (int)drawable_sprite.rect.width + x;

            // Check if this is a valid position
            if (array_pos > cur_colors.Length || array_pos < 0)
                return;

            cur_colors[array_pos] = color;
        }
        public void ApplyMarkedPixelChanges()
        {
            drawable_texture.SetPixels32(cur_colors);
            drawable_texture.Apply();
        }

        public void UndoStroke() {
            if (strokes.Count > 0 && prev_colors.Count > 0)
            {
                cur_colors = prev_colors[^1];
                ApplyMarkedPixelChanges();
                prev_colors.RemoveAt(prev_colors.Count - 1);
                strokes.RemoveAt(strokes.Count - 1);
                // If we have predictions that haven't been undone yet, remove the last one
                // Otherwise, increment strokes_undone to remove it later
                if (strokes.Count >= predictions.Count)
                {
                    // Prediction has not finished yet, so we need to remove it later
                    strokes_undone++;
                }
                else {
                    predictions.RemoveAt(predictions.Count - 1);
                }
                if (strokes.Count == predictions.Count && strokes.Count > 0)
                {
                    submitButton.interactable = true;
                    predictionText.text = predictions[^1];
                }
                else if (strokes.Count == 0)
                {
                    submitButton.interactable = false;
                    predictionText.text = "..............";
                }
            }
        }


        // Directly colours pixels. This method is slower than using MarkPixelsToColour then using ApplyMarkedPixelChanges
        // SetPixels32 is far faster than SetPixel
        // Colours both the center pixel, and a number of pixels around the center pixel based on pen_thickness (pen radius)
        public void ColourPixels(Vector2 center_pixel, int pen_thickness, Color color_of_pen)
        {
            // Figure out how many pixels we need to colour in each direction (x and y)
            int center_x = (int)center_pixel.x;
            int center_y = (int)center_pixel.y;
            //int extra_radius = Mathf.Min(0, pen_thickness - 2);

            for (int x = center_x - pen_thickness; x <= center_x + pen_thickness; x++)
            {
                for (int y = center_y - pen_thickness; y <= center_y + pen_thickness; y++)
                {
                    drawable_texture.SetPixel(x, y, color_of_pen);
                }
            }

            drawable_texture.Apply();
        }


        public Vector2 WorldToPixelCoordinates(Vector2 world_position)
        {
            // Change coordinates to local coordinates of this image
            Vector3 local_pos = transform.InverseTransformPoint(world_position);

            // Change these to coordinates of pixels
            float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;

            // Need to center our coordinates
            float centered_x = local_pos.x * unitsToPixels + pixelWidth / 2;
            float centered_y = local_pos.y * unitsToPixels + pixelHeight / 2;

            // Round current mouse position to nearest pixel
            Vector2 pixel_pos = new Vector2(Mathf.RoundToInt(centered_x), Mathf.RoundToInt(centered_y));

            return pixel_pos;
        }
		// Some guy requested this - it might be wrong
        public Vector3 PixelToWorldCoordinates(Vector2 pixel_pos)
        {
			float pixelWidth = drawable_sprite.rect.width;
            float pixelHeight = drawable_sprite.rect.height;
            float unitsToPixels = pixelWidth / drawable_sprite.bounds.size.x * transform.localScale.x;
			
			// Need to uncenter our coordinates
			float uncentered_x = pixel_pos.x / unitsToPixels - pixelWidth / 2;
			float uncentered_y = pixel_pos.y / unitsToPixels - pixelHeight / 2;
			
			// Convert point to world space
			Vector3 world_pos = transform.TransformPoint(new Vector3(uncentered_x, uncentered_y, 0f));
            return world_pos;
        }

        // Changes every pixel to be the reset colour
        public void ResetCanvas()
        {
            drawable_texture.SetPixels(clean_colours_array);
            drawable_texture.Apply();
        }


        
        void Awake()
        {
            drawable = this;
            // DEFAULT BRUSH SET HERE
            current_brush = PenBrush;

            submitButton = GameObject.Find("CheckMarkButton").GetComponent<Button>();
            predictionText = GameObject.Find("PredictText").GetComponent<TMP_Text>();


            drawable_sprite = this.GetComponent<SpriteRenderer>().sprite;
            drawable_texture = drawable_sprite.texture;

            // Initialize clean pixels to use
            clean_colours_array = new Color[(int)drawable_sprite.rect.width * (int)drawable_sprite.rect.height];
            for (int x = 0; x < clean_colours_array.Length; x++)
                clean_colours_array[x] = Reset_Colour;

            // Should we reset our canvas image when we hit play in the editor?
            if (Reset_Canvas_On_Play)
                ResetCanvas();
			else if (Reset_To_This_Texture_On_Play)
			{
				Graphics.CopyTexture(reset_texture, drawable_texture);
				//drawable_texture = reset_texture;
				Debug.Log("Reset texture");
			}
        }
    }
}
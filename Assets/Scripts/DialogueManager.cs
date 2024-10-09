using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;

public class DialogueManager : MonoBehaviour
{
    public LLMCharacter llmCharacter;

    // Do something with the reply from the llmCharacter
    void HandleReply(string reply)
    {
        Debug.Log(reply);
    }

    void ReplyCompleted()
    {
        // do something when the reply from the model is complete
        Debug.Log("The AI replied");
    }


    // Update is called once per frame
    void Update()
    {

        // Beyond shitty, i know
        // inderdaad
        if (Input.GetKeyDown(KeyCode.Alpha1)) // For key 1
        {
            GenerateMesage(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2)) // For key 2
        {
            GenerateMesage(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3)) // For key 3
        {
            GenerateMesage(3);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4)) // For key 4
        {
            GenerateMesage(4);
        }
    }

    void GenerateMesage(int i)
    {
        string message;
        switch (i)
        {
            case 1:
                message = ("Hi, I'm Shrimp nice to meet you!");
                break;
            case 2:
                message = ("What did you do today?");
                break;
            case 3:
                message = ("What's your favourite thing about your job?");
                break;
            default:
                message = ("Skooboodoobob, I am a shrimp, and am shrimpin around");
                break;
        }
        Debug.Log("Asking to Gnorp: " + message);
        _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted);
    }
}

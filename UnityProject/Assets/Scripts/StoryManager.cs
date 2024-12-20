using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using System;
using UnityEngine.Timeline;

public enum StoryNodeType
{
    OutputComplete,
    OutputIncomplete,
    TextInput,
    DrawInput
};

public enum StoryPoint
{
    Introduction,
    Computer,
    Coffee,
    Gnoblin
}

public struct StoryNode
{
    public StoryNodeType storyNodeType;
    public string activeCharacterName;
    public string dialogueBoxText;
 
    public Sprite background;
    public CharacterStruct characterLeft;
    public CharacterStruct characterRight;
}

public partial class StoryManager : MonoBehaviour
{
    //public bool firstload = true;
    public Sprite BackgroundSprite;

    public MainGameManager mainGameManager;
    public CharacterDatabase characterDatabase;
    public AudioSource bgmPlayer;

    private StoryNode initialStoryNode;
    private StoryNode currentStoryNode;
    private string LastInputText;
    private string LastRecognizedObjectString;
    private string LastLLMOutputText;
    private int drawAttempt = 0;
    
    private CharacterStruct activeCharacter;
    private CharacterStruct sideCharacter;

    private StoryPoint storyPoint = StoryPoint.Introduction;
    private int progress = 0;

    public void Awake()
    {
        PlayNewBGMTrack((AudioClip)Resources.Load("Audio\\Paper Dreams"));
    }

    public void Start()
    {
        activeCharacter = characterDatabase.Get(CharacterEnum.Character2);
        sideCharacter = characterDatabase.Get(CharacterEnum.Character3);
        mainGameManager.ToggleLeftCharacter(false);
        mainGameManager.ToggleRightCharacter(false);

        initialStoryNode = new StoryNode();
        initialStoryNode.storyNodeType = StoryNodeType.OutputComplete;
        initialStoryNode.activeCharacterName = "Me";
        initialStoryNode.dialogueBoxText = "ZZzzzz.... mimimimimimi.... *Sleeping noises*";

        initialStoryNode.background = null;
        initialStoryNode.characterLeft = activeCharacter;
        initialStoryNode.characterRight = sideCharacter;

        mainGameManager.SubmitStoryNode(initialStoryNode);
    }

    public void Continue()
    {
        //actual story - temp
        //CharacterStruct activeCharacter = characterGenerator.Generate(CharacterEnum.Character0);
        //CharacterStruct sideCharacter = characterGenerator.Generate(CharacterEnum.Character1);

        StoryNode newStoryNode = new StoryNode();

        switch (storyPoint)
        {
            case StoryPoint.Introduction:
                newStoryNode = IntroductionStory();
                break;
            case StoryPoint.Computer:
                newStoryNode = ComputerStory();
                break;
            case StoryPoint.Coffee:
                newStoryNode = CoffeeStory();
                break;
            case StoryPoint.Gnoblin:
                newStoryNode = GnoblinStory();
                break;
        }
        
        progress++;
        //mainGameManager.ScrollToTop();
        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    public void SubmitInputText(string inputText)
    {
        LastInputText = inputText;
    }

    // should this be async?
    async void GenerateMessage(LLMCharacter llmCharacter, string message)
    {
        Debug.Log("Asking to AI: " + message);
        //////////////////////////////
        /////////////////////////////////
        ///////////////////////////
        
        _ = llmCharacter.Chat(message, HandleReply, ReplyCompleted); //TODO: FIX THIS SHIT SHO HAARD
        /////////////////////
        /////////////////////
        ///////////////////////////
        /////////////////////////////////
    }


    // Do something with the reply from the llmCharacter
    void HandleReply(string reply)
    {
        LastLLMOutputText = reply;

        StoryNode newStoryNode = GenerateGenericNode(reply, StoryNodeType.OutputIncomplete);
        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    void ReplyCompleted()
    {
        Debug.Log(LastLLMOutputText);
        StoryNode newStoryNode = GenerateGenericNode(LastLLMOutputText, StoryNodeType.OutputComplete);
        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    StoryNode GenerateGenericNode(string text, StoryNodeType storyNodeType, Sprite background = null)
    {
        StoryNode newStoryNode = new StoryNode();
        newStoryNode.storyNodeType = storyNodeType;
        newStoryNode.activeCharacterName = activeCharacter.name;
        newStoryNode.dialogueBoxText = text;

        newStoryNode.background = background;
        newStoryNode.characterLeft = activeCharacter;
        newStoryNode.characterRight = sideCharacter;

        return newStoryNode;
    }

    // Utility functions
    public void SwapActiveCharacter()
    {
        CharacterStruct temp = activeCharacter;
        activeCharacter = sideCharacter;
        sideCharacter = temp;
    }

    public void SetRecognizedDrawingObjectString(string recognizedObject)
    {
        LastRecognizedObjectString = recognizedObject;
    }

    private void PlayNewBGMTrack(AudioClip audioClip)
    {
        bgmPlayer.clip = audioClip;
        bgmPlayer.Play();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LLMUnity;
using System;

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
    Part2
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

    private StoryNode initialStoryNode;
    private StoryNode currentStoryNode;
    private string LastInputText;
    private string LastLLMOutputText;

    private CharacterStruct activeCharacter;
    private CharacterStruct sideCharacter;

    private StoryPoint storyPoint = StoryPoint.Introduction;
    private int progress = 0;

    public void Start()
    {
        activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
        sideCharacter = characterDatabase.Get(CharacterEnum.Character1);

        initialStoryNode = new StoryNode();
        initialStoryNode.storyNodeType = StoryNodeType.OutputComplete;
        initialStoryNode.activeCharacterName = "???";
        initialStoryNode.dialogueBoxText = "...";

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
            case StoryPoint.Part2:
                newStoryNode = Part2Story();
                break;
        }
        
        progress++;

        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    public void SubmitInputText(string inputText)
    {
        LastInputText = inputText;
    }

    void GenerateMessage(LLMCharacter llmCharacter, string message)
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
        Debug.Log(reply);
        LastLLMOutputText = reply;

        StoryNode newStoryNode = GenerateReplyNode(reply, StoryNodeType.OutputIncomplete);
        Debug.Log(newStoryNode.dialogueBoxText);
        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    void ReplyCompleted()
    {
        StoryNode newStoryNode = GenerateReplyNode(LastLLMOutputText, StoryNodeType.OutputComplete);
        mainGameManager.SubmitStoryNode(newStoryNode);
    }

    StoryNode GenerateReplyNode(string reply, StoryNodeType storyNodeType)
    {
        CharacterStruct activeCharacter = characterDatabase.Get(CharacterEnum.Character0);
        CharacterStruct sideCharacter = characterDatabase.Get(CharacterEnum.Character1);

        StoryNode newStoryNode = new StoryNode();
        newStoryNode.storyNodeType = StoryNodeType.OutputIncomplete;
        newStoryNode.activeCharacterName = activeCharacter.name;
        newStoryNode.dialogueBoxText = reply;

        newStoryNode.background = null;
        newStoryNode.characterLeft = activeCharacter;
        newStoryNode.characterRight = sideCharacter;

        return newStoryNode;
    }

    // Utility functions
    public void swapActiveCharacter()
    {
        CharacterStruct temp = activeCharacter;
        activeCharacter = sideCharacter;
        sideCharacter = temp;
    }
}
